using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

public enum GameState
{
    Intro, MainMenu, Gameplay
}

public interface IGameManager
{
    public UnityAction<GameState> OnGameStateChanged { get; set;}
    public void GameplayStarted();
}

public class GameManager : MonoBehaviour, IGameManager
{
    public UnityAction<GameState> OnGameStateChanged { get; set; }
    private GameState currentState;
    private IUIPanelManager uiPanelManager;
    

    [Inject]
    public void Construct(IUIPanelManager uiPanelManager)
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


