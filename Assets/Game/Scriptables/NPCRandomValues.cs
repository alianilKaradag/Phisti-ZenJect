using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "NPCRandomValues", menuName = "Anil/NPCRandomValues")]
public class NPCRandomValues : ScriptableObjectInstaller<NPCRandomValues>
{
    [SerializeField] private NPCRandomValues randomValues;
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<NPCRandomValues>().FromInstance(randomValues).AsSingle().NonLazy();
        
        // Container.BindInstance(NewBie_WinAmountRange).IfNotBound();
        // Container.BindInstance(NewBie_LostAmountRange).IfNotBound();
        // Container.BindInstance(NewBie_MoneyAmountRange).IfNotBound();
        // Container.BindInstance(Rookie_WinAmountRange).IfNotBound();
        // Container.BindInstance(Rookie_LostAmountRange).IfNotBound();
        // Container.BindInstance(Rookie_MoneyAmountRange).IfNotBound();
        // Container.BindInstance(Noble_WinAmountRange).IfNotBound();
        // Container.BindInstance(Noble_LostAmountRange).IfNotBound();
        // Container.BindInstance(Noble_MoneyAmountRange).IfNotBound();
    }
   
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
