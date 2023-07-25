using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class UserInfo : MonoBehaviour
{
    [SerializeField, Foldout("Settings")] private bool isPlayerInfo;

    [SerializeField, Foldout("Setup")] private TextMeshProUGUI userNameText;
    [SerializeField, Foldout("Setup")] private TextMeshProUGUI moneyText;

    private DataManager dataManager;
    private UserInfoData userInfoData;
    private UserProfilePopUp userProfilePopUp;
    
    [Inject]
    public void Construct(DataManager dataManager, UserProfilePopUp userProfilePopUp)
    {
        this.dataManager = dataManager;
        this.userProfilePopUp = userProfilePopUp;
    }

    public void OnProfileClicked()
    {
        UserInfoData data;

        if (isPlayerInfo)
        {
            var playerData = dataManager.PlayerData;
            data = new UserInfoData("Player", playerData.WinAmount, playerData.LostAmount, playerData.PlayerTotalMoney);
        }
        else
        {
            data = userInfoData;
        }

        userProfilePopUp.Open(data);
    }
    
    public void SetData(UserInfoData userInfoData)
    {
        this.userInfoData = userInfoData;

        userNameText.text = userInfoData.UserName;
        moneyText.text = "<sprite=0>" + userInfoData.MoneyAmount;
    }
}