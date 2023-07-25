using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using NaughtyAttributes;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class BotBrain : MonoBehaviour
{
    [SerializeField] private CharacterController user;

    private Coroutine decisionRoutine;
    private BoardManager boardManager;

    [Inject]
    public void Construct(BoardManager boardManager)
    {
        this.boardManager = boardManager;
    }
    private void Start()
    {
        user.OnMyTurn += MakeADecision;
        OptionsMenu.OnBackToLobbyChoosed += Reset;
        OptionsMenu.OnNewGameChoosed += Reset;
    }

    private void OnDestroy()
    {
        user.OnMyTurn -= MakeADecision;
        OptionsMenu.OnBackToLobbyChoosed -= Reset;
        OptionsMenu.OnNewGameChoosed -= Reset;
    }

    private void MakeADecision(Card lastCardOnTheTable)
    {
        decisionRoutine = StartCoroutine(DecisionRoutine(lastCardOnTheTable));
    }

    private IEnumerator DecisionRoutine([CanBeNull] Card lastCardOnTheTable)
    {
        yield return new WaitForSeconds(0.8f);
        
        Card willPlayCard;

        if (lastCardOnTheTable == null)
        {
            willPlayCard = CountPlayedCards();
            user.PlayCard(willPlayCard);
            yield break;
        }

        willPlayCard = user.CardsOnHand.Find(x => x.CardValue == lastCardOnTheTable.CardValue);

        if (willPlayCard != null)
        {
            user.PlayCard(willPlayCard);
            yield break;
        }

        willPlayCard = user.CardsOnHand.Find(x => x.CardValue == CardValue.Joker);

        if (willPlayCard != null && Random.Range(0, 10) >= 5)
        {
            user.PlayCard(willPlayCard);
            yield break;
        }

        willPlayCard = CountPlayedCards();

        user.PlayCard(willPlayCard);
    }

    private Card CountPlayedCards() // Makes decisions based on previously played cards"
    {
        Card willPlayCard;
        var cardValues = new List<CardValue>();

        foreach (var item in user.CardsOnHand)
        {
            if (item.CardValue != CardValue.Joker)
            {
                cardValues.Add(item.CardValue);
            }
        }

        if (cardValues.Count == 0)
        {
            return user.CardsOnHand[0];
        }

        var intersectedCards = boardManager.PlayedCards.Intersect(cardValues).ToList();

        if (intersectedCards.Count > 0)
        {
            var mostPlayedCard = CardValue.One;
            var maxCount = 0;

            foreach (var item in intersectedCards)
            {
                var count = boardManager.PlayedCards.Count(x => x == item);

                if (count >= maxCount)
                {
                    maxCount = count;
                    mostPlayedCard = item;
                }
            }

            if (Random.Range(0, 10) >= 6)
            {
                willPlayCard = user.CardsOnHand.First(x => x.CardValue == mostPlayedCard);
                return willPlayCard;
            }
        }

        var rndValue = cardValues[Random.Range(0, cardValues.Count)];
        willPlayCard = user.CardsOnHand.FirstOrDefault(x => x.CardValue == rndValue); // There is nothing to do. Make it randomly :)

        return willPlayCard;
    }

    private void Reset()
    {
        if (decisionRoutine != null)
        {
            StopCoroutine(decisionRoutine);
            decisionRoutine = null;
        }
    }
}