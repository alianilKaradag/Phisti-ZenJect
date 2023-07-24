using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class UIMainManuPanel : UIPanelBase
{
    [Inject] private IGameManager gameManager;
    
    public void OnTapToPlayClicked()
    {
        gameManager.GameplayStarted();
    }
}
