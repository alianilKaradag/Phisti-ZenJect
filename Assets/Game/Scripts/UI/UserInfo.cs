using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class UserInfo : MonoBehaviour
{
    [SerializeField, Foldout("Settings")] private bool isPlayerInfo;

    [SerializeField, Foldout("Setup")] private TextMeshProUGUI userNameText;
    [SerializeField, Foldout("Setup")] private TextMeshProUGUI moneyText;

    private UserInfoData userInfoData;

    public void OnProfileClicked()
    {
        UserInfoData data;

        if (isPlayerInfo)
        {
            var playerData = DataManager.Instance.PlayerData;
            data = new UserInfoData("Player", playerData.WinAmount, playerData.LostAmount, playerData.PlayerTotalMoney);
        }
        else
        {
            data = userInfoData;
        }

        UserProfilePopUp.Instance.Open(data);
    }
    
    public void SetData(UserInfoData userInfoData)
    {
        this.userInfoData = userInfoData;

        userNameText.text = userInfoData.UserName;
        moneyText.text = "<sprite=0>" + userInfoData.MoneyAmount;
    }
}