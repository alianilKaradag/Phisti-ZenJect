using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class CardDealer : MonoBehaviour
{
    public bool IsDealingCards { get; private set; }

    [SerializeField, Foldout("Setup")] private List<Card> cards;
    [SerializeField, Foldout("Setup")] private Transform deckPoint;
    [SerializeField, Foldout("Setup")] private Transform cardThrowPoint;

    private List<CharacterController> users;
    private List<Card> shuffledDeck;
    private Coroutine dealRoutine;
    private BoardManager boardManager;
    private Card.Factory cardFactory;

    private bool isDeckOver;
    private bool isGameFresh = true;

    private float lastThrowPointYPos;

    [Inject]
    public void Construct(BoardManager boardManager, Card.Factory cardFactory)
    {
        this.boardManager = boardManager;
        this.cardFactory = cardFactory;
    }
    
    public void CreateDeck(List<CharacterController> users)
    {
        this.users = users;
        StartCoroutine(CreateDeckRoutine());
    }

    private IEnumerator CreateDeckRoutine()
    {
        yield return new WaitForSeconds(1.5f);

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
                boardManager.UpdateRemainingText(shuffledDeck.Count);

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
        shuffledDeck.Remove(lastCard);
        card.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        return card;
    }

    private IEnumerator DealGroundCards() // Starting cards creation on the table
    {
        lastThrowPointYPos = cardThrowPoint.position.y;
        Card lastCardOnTheTable = null;

        for (var i = 0; i < 4; i++)
        {
            var canKeepGoing = false;
            var card = InitiateCard();
            boardManager.AddCardToTable(card);
            card.Play(cardThrowPoint, lastThrowPointYPos, i == 3, () => canKeepGoing = true);
            boardManager.UpdateRemainingText(shuffledDeck.Count);
            IncreaseLastCardYPos();
            lastCardOnTheTable = card;

            yield return new WaitUntil(() => canKeepGoing);
        }

        dealRoutine = null;
        boardManager.PlayedCards.Add(lastCardOnTheTable.CardValue);
        boardManager.SetUserTurn();
    }

    public void IncreaseLastCardYPos()
    {
        lastThrowPointYPos += 0.001f;
    }

    public int GetCurrentDeckCardAmount() => shuffledDeck != null ? shuffledDeck.Count : 0;

    public void CheckForNewTurnDealing()
    {
        if (!isDeckOver)
        {
            if (users.All(x => x.CardsOnHand.Count == 0))
            {
                dealRoutine = StartCoroutine(DealCardsRoutine());
            }
        }
    }

    public void ResetThrowPos()
    {
        lastThrowPointYPos = cardThrowPoint.position.y;
    }

    public Transform GetThrowPoint(out float yPos)
    {
        yPos = lastThrowPointYPos;
        return cardThrowPoint;
    }

    public void Reset()
    {
        isDeckOver = false;
        isGameFresh = true;
        lastThrowPointYPos = cardThrowPoint.position.y;

        if (dealRoutine != null)
        {
            StopCoroutine(dealRoutine);
            dealRoutine = null;
        }
    }
}