using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public class CardDealer
{
    public List<CardValue> PlayedCards { get; private set; } = new List<CardValue>();
    
    public bool IsDealingCards { get; private set; }
    
    private List<Card> shuffledDeck;
    
    private Coroutine dealRoutine;
    
    private bool isDeckOver;
    private bool isGameFresh = true;
    
    private float lastThrowPointYPos;
    
}
