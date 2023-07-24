using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;
using Random = UnityEngine.Random;

public class CharacterManager : Singleton<CharacterManager>
{
    public CharacterController Player;
    
    [SerializeField] private List<CharacterController> characters;
    [SerializeField] private NPCRandomValues npcRandomValues;


    private TableData tableData;
    private IBoardManager boardManager;

    [Inject]
    public void Construct(IBoardManager boardManager)
    {
        this.boardManager = boardManager;
    }
    
    private void Start()
    {
        SaloonManager.OnJoinedTable += DecideCharacters;
    }

    private void OnDestroy()
    {
        SaloonManager.OnJoinedTable -= DecideCharacters;
    }

    private void DecideCharacters(TableData tableData)
    {
        this.tableData = tableData;
        SetCharactersStatus(4, false); //At the beginning, close all users.
        SetCharactersStatus(tableData.SaloonSize, true);
    }


    private void SetCharactersStatus(int userAmount, bool isOpen)
    {
        if (userAmount == 2)
        {
            for (var i = 0; i < characters.Count; i+=2)
            {
                characters[i].SetAsUser(isOpen);

                if (isOpen) boardManager.AddUser(characters[i]);
            }
            
            return;
        }
        
        for (var i = 0; i < userAmount; i++)
        {
            characters[i].SetAsUser(isOpen);
            
            if (isOpen) boardManager.AddUser(characters[i]);
        }
    }

    public UserInfoData GetRandomData()
    {
        var randomName = npcRandomValues.Names[Random.Range(0, npcRandomValues.Names.Count)];
        var randomWinAmount = Random.Range(npcRandomValues.NewBie_WinAmountRange.x, npcRandomValues.NewBie_WinAmountRange.y);
        var randomLostAmount = Random.Range(npcRandomValues.NewBie_LostAmountRange.x, npcRandomValues.NewBie_LostAmountRange.y);
        var randomMoney = Random.Range(npcRandomValues.NewBie_MoneyAmountRange.x,npcRandomValues.NewBie_MoneyAmountRange.y);
        
        switch (tableData.SaloonType)
        {
            case SaloonType.Rookies:
                randomWinAmount = Random.Range(npcRandomValues.Rookie_WinAmountRange.x, npcRandomValues.Rookie_WinAmountRange.y);
                randomLostAmount = Random.Range(npcRandomValues.Rookie_LostAmountRange.x, npcRandomValues.Rookie_LostAmountRange.y);
                randomMoney = Random.Range(npcRandomValues.Rookie_MoneyAmountRange.x,npcRandomValues.Rookie_MoneyAmountRange.y);
                break;
            case SaloonType.Nobles:
                randomWinAmount = Random.Range(npcRandomValues.Noble_WinAmountRange.x, npcRandomValues.Noble_WinAmountRange.y);
                randomLostAmount = Random.Range(npcRandomValues.Noble_LostAmountRange.x, npcRandomValues.Noble_LostAmountRange.y);
                randomMoney = Random.Range(npcRandomValues.Noble_MoneyAmountRange.x,npcRandomValues.Noble_MoneyAmountRange.y);
                break;
           
        }
        var data = new UserInfoData(randomName, randomWinAmount, randomLostAmount, randomMoney);
        
        return data;
    }
}
