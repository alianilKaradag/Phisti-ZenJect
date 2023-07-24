using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Anil/PlayerData")]
public class PlayerData : ScriptableObject
{
    public int WinAmount;
    public int LostAmount;

    [SerializeField] private int playerTotalMoney;
    public int PlayerTotalMoney
    {
        get
        {
            return playerTotalMoney;
        }
        set
        {
            if (value < 0)
            {
                value = 0;
            }

            playerTotalMoney = value;
        }
    }
}
