using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NPCRandomValues", menuName = "Anil/NPCRandomValues")]
public class NPCRandomValues : ScriptableObject
{
    [Header("New Bie")] 
    public Vector2Int NewBie_WinAmountRange;
    public Vector2Int NewBie_LostAmountRange;
    public Vector2Int NewBie_MoneyAmountRange;
    
    [Header("Rookie")] 
    public Vector2Int Rookie_WinAmountRange;
    public Vector2Int Rookie_LostAmountRange;
    public Vector2Int Rookie_MoneyAmountRange;
    
    [Header("Noble")] 
    public Vector2Int Noble_WinAmountRange;
    public Vector2Int Noble_LostAmountRange;
    public Vector2Int Noble_MoneyAmountRange;

    [Header("Names")] public List<string> Names;
}
