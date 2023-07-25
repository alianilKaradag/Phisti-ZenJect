using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class UIMainManuPanel : UIPanelBase
{
     private GameManager gameManager;

     [Inject]
    public void Construct(GameManager gameManager)
    {
        this.gameManager = gameManager;
    }
    public void OnTapToPlayClicked()
    {
        gameManager.GameplayStarted();
    }
}
