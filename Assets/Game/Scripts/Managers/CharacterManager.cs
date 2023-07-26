using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class CharacterManager : MonoBehaviour
{
    [Inject(Id = "PlayerController")] public CharacterController Player { get;}
    [Inject] private NPCRandomValues npcRandomValues;
    [SerializeField] private List<CharacterController> characters;
    
    private TableData tableData;
    private BoardManager boardManager;
    private RandomUserInfoGenerator randomUserInfoDataGenerator;

    [Inject]
    public void Construct(BoardManager boardManager, RandomUserInfoGenerator randomUserInfoDataGenerator)
    {
        this.boardManager = boardManager;
        this.randomUserInfoDataGenerator = randomUserInfoDataGenerator;
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
        return randomUserInfoDataGenerator.Generate(npcRandomValues, tableData.SaloonType);
    }
}
