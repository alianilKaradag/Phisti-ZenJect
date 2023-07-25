using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

public enum GameState
{
    Intro, MainMenu, Gameplay
}

public class GameManager : MonoBehaviour
{
    public UnityAction<GameState> OnGameStateChanged { get; set; }
    private GameState currentState;
    private UIPanelManager uiPanelManager;
    

    [Inject]
    public void Construct(UIPanelManager uiPanelManager)
    {
        this.uiPanelManager = uiPanelManager;
    }
    
    private void Awake()
    {
        uiPanelManager.Init();
    }

    private void Start()
    {
        OnGameStateChanged?.Invoke(GameState.MainMenu);
    }

    private void OnDestroy()
    {
        DOTween.KillAll();
    }

    public void GameplayStarted()
    {
        OnGameStateChanged?.Invoke(GameState.Gameplay);
    }
}


