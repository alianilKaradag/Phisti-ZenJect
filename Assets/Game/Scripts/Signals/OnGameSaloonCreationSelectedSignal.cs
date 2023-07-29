using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnGameSaloonCreationSelectedSignal
{
    public SaloonType SaloonType;
    public int MinBet;
    public int MaxBet;
    
    public OnGameSaloonCreationSelectedSignal(SaloonType saloonType, int minBet, int maxBet)
    {
        SaloonType = saloonType;
        MinBet = minBet;
        MaxBet = maxBet;
    }
}
