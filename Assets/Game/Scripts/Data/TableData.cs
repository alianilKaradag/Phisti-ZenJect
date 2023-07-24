using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableData : MonoBehaviour
{
    public SaloonType SaloonType { get; private set; }
    public int SaloonSize { get; private set; }
    public int BetAmount { get; private set; }

    public TableData(SaloonType saloonType, int saloonSize, int betAmount)
    {
        SaloonType = saloonType;
        SaloonSize = saloonSize;
        BetAmount = betAmount;
    }
}
