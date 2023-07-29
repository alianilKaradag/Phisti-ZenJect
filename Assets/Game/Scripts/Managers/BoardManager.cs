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

    //private List<CharacterController> users = new List<CharacterController>();
    private List<Card> cardsOnTheTable = new List<Card>();
    //private Coroutine dealRoutine;

    //private CharacterController currentUser => users[turnIndex];
    //private CharacterController lastCardGainedUser;
    private TableData tableData;
    [Inject] private CharacterManager characterManager;
    //[Inject] private ScoreManager scoreManager;
    [Inject] private GameCompletePopUp gameCompletePopUp;
    [Inject] private OptionsMenu optionsMenu;
    [Inject] private CardDealer cardDealer;
    [Inject] private UserHandler userHandler;
    
   // private int turnIndex;


    // [Inject]
    // public void Construct(CharacterManager characterManager, ScoreManager scoreManager, GameCompletePopUp gameCompletePopUp, OptionsMenu optionsMenu, CardDealer cardDealer)
    // {
    //     this.characterManager = characterManager;
    //     this.scoreManager = scoreManager;
    //     this.gameCompletePopUp = gameCompletePopUp;
    //     this.optionsMenu = optionsMenu;
    //     this.cardDealer = cardDealer;
    // }

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

    private void PlayerJoinedTable(TableData tableData)
    {
        this.tableData = tableData;
        userHandler.SetTableData(tableData);
        cardDealer.CreateDeck(userHandler.Users);
    }

    public void AddUser(CharacterController character)
    {
        userHandler.AddUser(character);
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
        
        userHandler.IncreaseTurnIndex();
        userHandler.SetTurn(cardsOnTheTable.LastOrDefault());
        cardDealer.CheckForNewTurnDealing();
    }

    private bool CheckForGameComplete()
    {
        if (PlayedCards.Count == 49)
        {
            IsGameCompleted = true;

            if (cardsOnTheTable.Count > 0)
            {
                userHandler.GainLevelEndTableCards(cardsOnTheTable);
                HideCardsOnTheTable();
            }

            userHandler.DetectWinner();
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

    public void SetUserTurn()
    {
        userHandler.SetTurn(cardsOnTheTable.LastOrDefault());
    }

    private void GainOnTheTableCards(Card card, bool isPhisti, CharacterController user)
    {
        cardsOnTheTable.Add(card);
        userHandler.GainCards(cardsOnTheTable, isPhisti);
        HideCardsOnTheTable();
        cardDealer.ResetThrowPos();
        userHandler.ChangeLastGainUser(user);
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

    public TableData GetTableData() => tableData;

    public void CallGameCompletePopUp(CharacterController winner)
    {
        StartCoroutine(CallGameCompletePopUpRoutine(winner));
    }
    private IEnumerator CallGameCompletePopUpRoutine(CharacterController winner)
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
        //turnIndex = 0;
        PlayedCards.Clear();
        DestroyCardsOnTheTable();
        remainingCardText.text = "52";
        //users.ForEach(x => x.Reset());
        userHandler.Reset();
        cardDealer.Reset();
    }

    private void BackToLobby()
    {
        if (!IsGameCompleted)
        {
            characterManager.Player.GameCompleted(null, tableData.BetAmount, tableData.SaloonSize);
        }

        ResetBoard();
        userHandler.ClearUsers();
    }

    private void StartNewGame()
    {
        if (!IsGameCompleted)
        {
            characterManager.Player.GameCompleted(null, tableData.BetAmount, tableData.SaloonSize);
        }

        ResetBoard();
        cardDealer.CreateDeck(userHandler.Users);
    }

    public void AddCardToTable(Card card)
    {
        cardsOnTheTable.Add(card);
    }

}