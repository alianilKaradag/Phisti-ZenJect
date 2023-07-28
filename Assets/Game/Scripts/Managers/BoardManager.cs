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

[RequireComponent(typeof(CardDealer))]
public class BoardManager : MonoBehaviour
{

    public static UnityAction<Card, CharacterController> OnACardPlayed;
    public List<CardValue> PlayedCards { get; private set; } = new List<CardValue>();

    public bool IsGameCompleted { get; private set; }

    [SerializeField, Foldout("Setup")] private TextMeshProUGUI remainingCardText;
    [SerializeField, Foldout("Setup")] private ParticleSystem confettiParticle;

    private List<CharacterController> users = new List<CharacterController>();
    private List<Card> cardsOnTheTable = new List<Card>();
    private Coroutine dealRoutine;

    private CharacterController currentUser => users[turnIndex];
    private CharacterController lastCardGainedUser;
    private TableData tableData;
    private CharacterManager characterManager;
    private ScoreManager scoreManager;
    private GameCompletePopUp gameCompletePopUp;
    private OptionsMenu optionsMenu;
    private CardDealer cardDealer;
    
    private int turnIndex;


    [Inject]
    public void Construct(CharacterManager characterManager, ScoreManager scoreManager, GameCompletePopUp gameCompletePopUp, OptionsMenu optionsMenu, CardDealer cardDealer)
    {
        this.characterManager = characterManager;
        this.scoreManager = scoreManager;
        this.gameCompletePopUp = gameCompletePopUp;
        this.optionsMenu = optionsMenu;
        this.cardDealer = cardDealer;
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
        cardDealer.CreateDeck(users);
    }

    public void UpdateRemainingText(int count)
    {
        remainingCardText.text = count.ToString();
    }

    private void ACardPlayed(Card card, CharacterController user)
    {
        cardDealer.IncreaseLastCardYPos();

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
        cardDealer.CheckForNewTurnDealing();
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
        cardDealer.ResetThrowPos();
        lastCardGainedUser = user;
    }

    public void SetUserTurn()
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

    public Transform GetThrowPoint(out float yPos) => cardDealer.GetThrowPoint(out yPos);

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
            user.GameCompleted(winner, tableData.BetAmount, tableData.SaloonSize);
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
        IsGameCompleted = false;
        turnIndex = 0;
        PlayedCards.Clear();
        DestroyCardsOnTheTable();
        remainingCardText.text = "52";
        users.ForEach(x => x.Reset());
        cardDealer.Reset();
    }

    private void BackToLobby()
    {
        if (!IsGameCompleted)
        {
            characterManager.Player.GameCompleted(null, tableData.BetAmount, tableData.SaloonSize);
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
        cardDealer.CreateDeck(users);
    }

    public void AddCardToTable(Card card)
    {
        cardsOnTheTable.Add(card);
    }

}