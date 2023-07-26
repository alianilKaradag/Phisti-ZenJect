using NaughtyAttributes;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Anil/PlayerData")]
public class PlayerData : ScriptableObjectInstaller<PlayerData>
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

    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<PlayerData>().FromInstance(this).AsSingle().NonLazy();
    }
}
