using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class CharacterManager : MonoBehaviour
{
    [SerializeField] private List<CharacterController> characters;
    [Inject(Id = "PlayerController")] public CharacterController Player { get;}
    [Inject]private NPCRandomValues npcRandomValues;
    [Inject]private SignalBus signalBus;
    private BoardManager boardManager;
    private RandomUserInfoGenerator randomUserInfoDataGenerator;
    
    private TableData tableData;

    [Inject]
    public void Construct(BoardManager boardManager, RandomUserInfoGenerator randomUserInfoDataGenerator)
    {
        this.boardManager = boardManager;
        this.randomUserInfoDataGenerator = randomUserInfoDataGenerator;
    }

    private void Start()
    {
        signalBus.Subscribe<OnJoinedTableSignal>(x => DecideCharacters(x.TableData));
        
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
        return randomUserInfoDataGenerator.Generate(npcRandomValues, tableData.SaloonType);
    }
}
