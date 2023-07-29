using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Zenject;

public class CharacterController : MonoBehaviour
{
    public UnityAction<Card> OnMyTurn;

    public List<Card> GainedCards { get; private set; } = new List<Card>();
    public List<Card> Phistis { get; private set; } = new List<Card>();

    public List<Transform> CardPoints => cardPoints;

    public List<Card> CardsOnHand;

    public Transform CardFirstDestination => cardFirstDestination;
    public bool IsMyTurn { get; private set; }
    public bool IsPlayer => isPlayer;
    public UserInfoData UserData => userData;

    [SerializeField, Foldout("Settings")] private bool isPlayer;

    [SerializeField, Foldout("Setup")] private Transform visual;
    [SerializeField, Foldout("Setup")] private Transform cardFirstDestination;
    [SerializeField, Foldout("Setup"), ReorderableList] private List<Transform> cardPoints;
    [SerializeField, Foldout("Setup")] private UserInfo userInfo;

    private UserInfoData userData;
    
    private BoardManager boardManager;
    private CharacterManager characterManager;
    private DataManager dataManager;
    private PlayerInfoArea playerInfoArea;

    [Inject]
    public void Construct(BoardManager boardManager, CharacterManager characterManager, DataManager dataManager, PlayerInfoArea playerInfoArea)
    {
        this.boardManager = boardManager;
        this.characterManager = characterManager;
        this.dataManager = dataManager;
        this.playerInfoArea = playerInfoArea;
    }

    public void SetAsUser(bool isOpen)
    {
        visual.gameObject.SetActive(isOpen);
        SetUserData();
    }

    private void SetUserData()
    {
        if (isPlayer)
        {
            var playerData = dataManager.PlayerData;
            userData = new UserInfoData("Player", playerData.WinAmount, playerData.LostAmount, playerData.PlayerTotalMoney);
        }
        else
        {
            SetRandomData();
        }

        userInfo.SetData(userData);
    }

    private void SetRandomData()
    {
        userData = characterManager.GetRandomData();
    }

    public void AddCard(Card card)
    {
        CardsOnHand.Add(card);
    }

    public void CardsGainedNormally(List<Card> cards)
    {
        GainedCards.AddRange(cards);
    }

    public void CardsGainedWithPhisti(List<Card> cards)
    {
        Phistis.AddRange(cards);
    }

    public void MyTurn([CanBeNull] Card lastCard)
    {
        IsMyTurn = true;
        OnMyTurn?.Invoke(lastCard);
    }

    public void PlayCard(Card card)
    {
        boardManager.AddPlayedCard(card.CardValue);
        var yPos = 0f;
        var destinationPoint = boardManager.GetThrowPoint(out yPos);
        card.Play(destinationPoint, yPos, true, () => BoardManager.OnACardPlayed?.Invoke(card, this));
        CardsOnHand.Remove(card);
        IsMyTurn = false;
    }

    public void GameCompleted([CanBeNull] CharacterController winner, int betAmount, int saloonSize)
    {
        var winAmount = userData.WinAmount;
        var lostAmount = userData.LostAmount;
        var money = userData.MoneyAmount;
        
        if (winner == this)
        {
            if (isPlayer)
            {
                dataManager.IncreaseWinAmount();
                dataManager.UpdateMoney(betAmount * (saloonSize -1));
                //SaveData.WinAmount++;
                //SaveData.PlayerTotalMoney += betAmount;
            }

            winAmount++;
            money += betAmount;
        }
        else
        {
            if (isPlayer)
            {
                dataManager.IncreaseLostAmount();
                dataManager.UpdateMoney(-betAmount);
                // SaveData.LostAmount++;
                // SaveData.PlayerTotalMoney -= betAmount;
            }

            lostAmount++;
            money -= betAmount;
        }

        userData = new UserInfoData(userData.UserName, winAmount, lostAmount, money);
        userInfo.SetData(userData);
        playerInfoArea.UpdateData();
    }

    public void Reset()
    {
        DestroyCardsOnHand();
        DestroyGainedCards();
        DestroyPhistis();
    }

    private void DestroyCardsOnHand()
    {
        foreach (var card in CardsOnHand)
        {
            card.DestroyCard();
        }

        CardsOnHand.Clear();
    }
    
    private void DestroyGainedCards()
    {
        foreach (var card in GainedCards)
        {
            card.DestroyCard();
        }

        GainedCards.Clear();
    }
    
    private void DestroyPhistis()
    {
        foreach (var card in Phistis)
        {
            card.DestroyCard();
        }

        Phistis.Clear();
    }
}