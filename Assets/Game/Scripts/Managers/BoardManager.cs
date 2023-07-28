using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class BoardManager : MonoBehaviour
{
    public UnityAction<GameState> OnGameStateChanged { get; set; }

    public static UnityAction<Card, CharacterController> OnACardPlayed;
    public List<CardValue> PlayedCards { get; private set; } = new List<CardValue>();

    public bool IsDealingCards { get; private set; }
    public bool IsGameCompleted { get; private set; }

    [SerializeField, Foldout("Setup")] private List<Card> cards;
    [SerializeField, Foldout("Setup")] private Transform deckPoint;
    [SerializeField, Foldout("Setup")] private Transform cardThrowPoint;
    [SerializeField, Foldout("Setup")] private TextMeshProUGUI remainingCardText;
    [SerializeField, Foldout("Setup")] private ParticleSystem confettiParticle;

    private List<CharacterController> users = new List<CharacterController>();
    private List<Card> shuffledDeck;
    private List<Card> cardsOnTheTable = new List<Card>();
    
    private CharacterController currentUser => users[turnIndex];
    private CharacterController lastCardGainedUser;
    private TableData tableData;
    private CharacterManager characterManager;
    private ScoreManager scoreManager;
    private GameCompletePopUp gameCompletePopUp;
    private OptionsMenu optionsMenu;
    private Card.Factory cardFactory;

    private bool isDeckOver;
    private bool isPlayerOnTheBoard;
    private bool isGameFresh = true;

    private int turnIndex;
    private float lastThrowPointYPos;

    private Coroutine dealRoutine;

    [Inject]
    public void Construct(CharacterManager characterManager, ScoreManager scoreManager, GameCompletePopUp gameCompletePopUp, OptionsMenu optionsMenu, Card.Factory cardFactory)
    {
        this.characterManager = characterManager;
        this.scoreManager = scoreManager;
        this.gameCompletePopUp = gameCompletePopUp;
        this.optionsMenu = optionsMenu;
        this.cardFactory = cardFactory;
    }
    
    private void Start()
    {
        SaloonManager.OnJoinedTable += PlayerJoinedTable;
        OnACardPlayed += ACardPlayed;
        OptionsMenu.OnBackToLobbyChoosed += BackToLobby;
        OptionsMenu.OnNewGameChoosed += StartNewGame;
    }

    private void OnDestroy()
    {
        SaloonManager.OnJoinedTable -= PlayerJoinedTable;
        OnACardPlayed -= ACardPlayed;
        OptionsMenu.OnBackToLobbyChoosed -= BackToLobby;
        OptionsMenu.OnNewGameChoosed -= StartNewGame;
    }
    
    public void AddUser(CharacterController character)
    {
        users.Add(character);
    }

    private void PlayerJoinedTable(TableData tableData)
    {
        this.tableData = tableData;
        StartCoroutine(CreateDeck());
    }

    private IEnumerator CreateDeck()
    {
        if (!isPlayerOnTheBoard) // for camera transition (Intro cam -> Gameplay cam)
        {
            yield return new WaitForSeconds(1.5f);
        }

        isPlayerOnTheBoard = true;
        shuffledDeck = new List<Card>();
        shuffledDeck = cards.OrderBy(a => Guid.NewGuid()).ToList();
        dealRoutine = StartCoroutine(DealCardsRoutine());
    }

    private IEnumerator DealCardsRoutine()
    {
        if (isDeckOver) yield break;

        IsDealingCards = true;

        foreach (var user in users)
        {
            for (var i = 0; i < 4; i++)
            {
                var canKeepGoing = false;
                var card = InitiateCard();

                var destinations = new List<Transform>() {user.CardFirstDestination, user.CardPoints[i]};
                user.AddCard(card);
                card.FlyToCharacterHand(destinations, () => canKeepGoing = true);
                UpdateRemainingText();

                if (shuffledDeck.Count == 0) isDeckOver = true;

                yield return new WaitUntil(() => canKeepGoing);
            }
        }

        if (isGameFresh)
        {
            isGameFresh = false;
            dealRoutine = null;
            dealRoutine = StartCoroutine(DealGroundCards());
        }

        IsDealingCards = false;
    }

    private Card InitiateCard()
    {
        var lastCard = shuffledDeck.Last();
        var card = cardFactory.Create(lastCard);
        card.SetParent(deckPoint);
        //var card = Instantiate(lastCard, deckPoint);
        shuffledDeck.Remove(lastCard);
        card.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        return card;
    }

    private IEnumerator DealGroundCards() // Starting cards creation on the table
    {
        lastThrowPointYPos = cardThrowPoint.position.y;

        for (var i = 0; i < 4; i++)
        {
            var canKeepGoing = false;
            var card = InitiateCard();

            cardsOnTheTable.Add(card);
            card.Play(cardThrowPoint, lastThrowPointYPos, i == 3, () => canKeepGoing = true);
            UpdateRemainingText();
            IncreaseLastCardYPos();

            yield return new WaitUntil(() => canKeepGoing);
        }

        dealRoutine = null;
        PlayedCards.Add(cardsOnTheTable.Last().CardValue);
        SetUserTurn();
    }

    private void UpdateRemainingText()
    {
        remainingCardText.text = shuffledDeck.Count.ToString();
    }

    private void ACardPlayed(Card card, CharacterController user)
    {
        IncreaseLastCardYPos();

        if (cardsOnTheTable.Count == 0)
        {
            cardsOnTheTable.Add(card);
        }
        else
        {
            CheckForGainOnTheTables(card, user);
        }

        if (CheckForGameComplete()) return;

        turnIndex = (turnIndex + 1) % tableData.SaloonSize;
        SetUserTurn();
        CheckForNewTurnDealing();
    }

    private bool CheckForGameComplete()
    {
        if (PlayedCards.Count == 49)
        {
            IsGameCompleted = true;

            if (cardsOnTheTable.Count > 0)
            {
                GiveTheLastCardsToLastGainedOne();
            }

            DetectWinner();
            return true;
        }

        return false;
    }

    private void CheckForGainOnTheTables(Card card, CharacterController user)
    {
        var lastCard = cardsOnTheTable.Last();

        if (lastCard != null)
        {
            if (lastCard.CardValue == card.CardValue)
            {
                bool isPhisti = cardsOnTheTable.Count == 1;
                GainOnTheTableCards(card, isPhisti, user);
            }
            else if (card.CardValue == CardValue.Joker)
            {
                GainOnTheTableCards(card, false, user);
            }
            else
            {
                cardsOnTheTable.Add(card);
            }
        }
    }

    private void CheckForNewTurnDealing()
    {
        if (!isDeckOver)
        {
            if (users.All(x => x.CardsOnHand.Count == 0))
            {
                dealRoutine = StartCoroutine(DealCardsRoutine());
            }
        }
    }

    private void GainOnTheTableCards(Card card, bool isPhisti, CharacterController user)
    {
        cardsOnTheTable.Add(card);

        if (isPhisti)
        {
            currentUser.CardsGainedWithPhisti(cardsOnTheTable);
        }
        else
        {
            currentUser.CardsGainedNormally(cardsOnTheTable);
        }

        HideCardsOnTheTable();
        lastThrowPointYPos = cardThrowPoint.position.y;
        lastCardGainedUser = user;
    }

    private void IncreaseLastCardYPos()
    {
        lastThrowPointYPos += 0.001f;
    }

    private void SetUserTurn()
    {
        var user = users[turnIndex];
        user.MyTurn(cardsOnTheTable.LastOrDefault());
    }

    private void HideCardsOnTheTable()
    {
        for (var i = 0; i < cardsOnTheTable.Count; i++)
        {
            cardsOnTheTable[i].HideCard();
        }

        cardsOnTheTable.Clear();
    }

    private void DestroyCardsOnTheTable()
    {
        for (var i = 0; i < cardsOnTheTable.Count; i++)
        {
            cardsOnTheTable[i].DestroyCard();
        }

        cardsOnTheTable.Clear();
    }

    public Transform GetThrowPoint(out float yPos)
    {
        yPos = lastThrowPointYPos;
        return cardThrowPoint;
    }

    public void AddPlayedCard(CardValue card)
    {
        PlayedCards.Add(card);
    }

    private void GiveTheLastCardsToLastGainedOne()
    {
        currentUser.CardsGainedNormally(cardsOnTheTable);
        HideCardsOnTheTable();
    }

    private void DetectWinner()
    {
        var winner = scoreManager.GetWinner(users);

        foreach (var user in users)
        {
            user.GameCompleted(winner, tableData.BetAmount , tableData.SaloonSize);
        }

        StartCoroutine(CallGameCompletePopUp(winner));
    }

    private IEnumerator CallGameCompletePopUp(CharacterController winner)
    {
        var didPlayerWin = winner == characterManager.Player;
        
        if (didPlayerWin)
        {
            confettiParticle.Play();
        }
        
        yield return new WaitForSeconds(1f);

        gameCompletePopUp.SetValues(winner.UserData.UserName, tableData.BetAmount, didPlayerWin);
        gameCompletePopUp.Open();

        yield return new WaitForSeconds(3f);

        if (!optionsMenu.IsOpen)
        {
            optionsMenu.Show();
        }
    }

    private void ResetBoard()
    {
        isDeckOver = false;
        IsGameCompleted = false;
        isPlayerOnTheBoard = false;
        isGameFresh = true;
        turnIndex = 0;
        lastThrowPointYPos = cardThrowPoint.position.y;
        DestroyCardsOnTheTable();
        PlayedCards.Clear();
        remainingCardText.text = cards.Count.ToString();
        users.ForEach(x => x.Reset());

        if (dealRoutine != null)
        {
            StopCoroutine(dealRoutine);
            dealRoutine = null;
        }
    }

    private void BackToLobby()
    {
        if (!IsGameCompleted)
        {
            characterManager.Player.GameCompleted(null, tableData.BetAmount,  tableData.SaloonSize);
        }
        
        ResetBoard();
        users.Clear();
    }

    private void StartNewGame()
    {
        if (!IsGameCompleted)
        {
            characterManager.Player.GameCompleted(null, tableData.BetAmount, tableData.SaloonSize);
        }
        
        ResetBoard();
        StartCoroutine(CreateDeck());
    }
}