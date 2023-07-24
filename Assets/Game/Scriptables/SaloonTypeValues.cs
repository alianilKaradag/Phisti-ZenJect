using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SaloonTypeValues", menuName = "Anil/SaloonTypeValues")]
public class SaloonTypeValues : ScriptableObject
{
    [Header("New Bie")]
    public int NewBie_MinBet = 250;
    public int NewBie_MaxBet = 5000;
    
    [Header("Rokie")]
    public int Rokie_MinBet = 2500;
    public int Rokie_MaxBet = 100000;
    
    [Header("Noble")]
    public int Noble_MinBet = 50000;
    public int Noble_MaxBet = 1000000;

}
