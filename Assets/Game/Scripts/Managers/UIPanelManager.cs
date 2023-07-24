using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public interface IUIPanelManager
{
    public void Init();
}

public class UIPanelManager : MonoBehaviour, IUIPanelManager
{
    private IGameManager gameManager;
    [SerializeField] private List<UIPanelBase> panels;

    [Inject]
    public void Construct(IGameManager gameManager)
    {
        this.gameManager = gameManager;
    }
    
    public void Init()
    {
        SetPanelsVisibility(GameState.Intro, true);
        gameManager.OnGameStateChanged += GameStateChanged;
    }

    private void OnDestroy()
    {
        gameManager.OnGameStateChanged -= GameStateChanged;
    }

    private void SetPanelsVisibility(GameState gameState, bool isInstant = false)
    {
        var index = (int) gameState;

        for (var i = 0; i < panels.Count; i++)
        {
            var item = panels[i];

            if (i == index)
            {
                item.Appear(isInstant);
                continue;
            }

            item.Disappear(isInstant);
        }
    }

    private void GameStateChanged(GameState gameState)
    {
        SetPanelsVisibility(gameState);
    }
}

