using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInfoData
{
    public string UserName { get; private set; }
    public int WinAmount{ get; private set; }
    public int LostAmount{ get; private set; }
    public int MoneyAmount{ get; private set; }
    
    public UserInfoData(string userName, int winAmount, int lostAmount, int moneyAmount)
    {
        UserName = userName;
        WinAmount = winAmount;
        LostAmount = lostAmount;
        MoneyAmount = moneyAmount;
    }
}
