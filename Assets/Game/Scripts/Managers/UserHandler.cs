using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class UserHandler
{
    public List<CharacterController> Users { get; private set; } = new List<CharacterController>();
    private CharacterController currentUser => Users[turnIndex];
    private CharacterController lastCardGainedUser;
    private TableData tableData;

    [Inject] private ScoreManager scoreManager;
    [Inject] private BoardManager boardManager;

    private int turnIndex;

    public void AddUser(CharacterController character)
    {
        Users.Add(character);
    }

    public void SetTableData(TableData tableData)
    {
        this.tableData = tableData;
    }

    public void GainCards(List<Card> cardsOnTheTable, bool isPhisti)
    {
        if (!isPhisti)
        {
            currentUser.CardsGainedNormally(cardsOnTheTable);
            return;
        }

        currentUser.CardsGainedWithPhisti(cardsOnTheTable);
    }

    public void GainLevelEndTableCards(List<Card> cardsOnTheTable)
    {
        lastCardGainedUser.CardsGainedNormally(cardsOnTheTable);
    }

    public void DetectWinner()
    {
        var winner = scoreManager.GetWinner(Users);

        foreach (var user in Users)
        {
            user.GameCompleted(winner, boardManager.GetTableData().BetAmount, boardManager.GetTableData().SaloonSize);
        }

        boardManager.CallGameCompletePopUp(winner);
    }

    public void SetTurn(Card lastCardOnTheTable)
    {
        var user = Users[turnIndex];
        user.MyTurn(lastCardOnTheTable);
    }

    public void IncreaseTurnIndex()
    {
        turnIndex = (turnIndex + 1) % tableData.SaloonSize;
    }

    public void Reset()
    {
        turnIndex = 0;
        Users.ForEach(x => x.Reset());
    }
    
    public void ChangeLastGainUser(CharacterController user)
    {
        lastCardGainedUser = user;
    }

    public void ClearUsers()
    {
        Users.Clear();
    }
}