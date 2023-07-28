using System;
using DG.Tweening;
using UnityEngine.Events;
using Zenject;


public enum GameState
{
    Intro, MainMenu, Gameplay
}

public class GameManager : IInitializable, IDisposable
{
    public UnityAction<GameState> OnGameStateChanged { get; set; }
    private GameState currentState;
    private UIPanelManager uiPanelManager;

    [Inject]
    public GameManager(UIPanelManager uiPanelManager)
    {
        this.uiPanelManager = uiPanelManager;
    }
    
    public void Initialize()
    {
        uiPanelManager.Init();
        OnGameStateChanged?.Invoke(GameState.MainMenu);
    }

    public void GameplayStarted()
    {
        OnGameStateChanged?.Invoke(GameState.Gameplay);
    }
    
    public void Dispose()
    {
        DOTween.KillAll();
    }
}


