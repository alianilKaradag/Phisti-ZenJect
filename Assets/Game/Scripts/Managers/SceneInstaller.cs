using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SceneInstaller : MonoInstaller<SceneInstaller>
{
    public override void InstallBindings()
    {
        Container.Bind<IGameManager>().To<GameManager>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<IUIPanelManager>().To<UIPanelManager>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<IBoardManager>().To<BoardManager>().FromComponentsInHierarchy().AsSingle().NonLazy();
    }
}