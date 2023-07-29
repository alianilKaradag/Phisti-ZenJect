using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SceneInstaller : MonoInstaller<SceneInstaller>
{
    public override void InstallBindings()
    {
        BindNonMonoBehaviourManagers();
        BindFactories();
        BindMonoBehaviourManagers();
        BindSignals();
    }
    
    private void BindNonMonoBehaviourManagers()
    {
        Container.BindInterfacesAndSelfTo<GameManager>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<DataManager>().AsSingle().NonLazy();
        
        Container.Bind<ScoreManager>().AsSingle();
        Container.Bind<InputHandler>().AsSingle();
        Container.Bind<RandomUserInfoGenerator>().AsSingle();
        Container.Bind<TableData>().AsTransient();
        Container.Bind<UserHandler>().AsSingle();
    }

    private void BindFactories()
    {
        Container.BindFactory<Card, Card, Card.Factory>();
    }

    private void BindMonoBehaviourManagers()
    {
        Container.Bind<UIPanelManager>().FromComponentInHierarchy().AsSingle();
        Container.Bind<BoardManager>().FromComponentsInHierarchy().AsSingle();
        Container.Bind<CardDealer>().FromComponentsInHierarchy().AsSingle();
        Container.Bind<CameraManager>().FromComponentsInHierarchy().AsSingle();
        Container.Bind<CharacterManager>().FromComponentsInHierarchy().AsSingle();
        Container.Bind<SaloonCreator>().FromComponentsInHierarchy().AsSingle();
        Container.Bind<SaloonManager>().FromComponentsInHierarchy().AsSingle();
        Container.Bind<ScrollManager>().FromComponentsInHierarchy().AsSingle();
        Container.Bind<ConfirmationPopUp>().FromComponentsInHierarchy().AsSingle();
        Container.Bind<GameCompletePopUp>().FromComponentsInHierarchy().AsSingle();
        Container.Bind<OptionsMenu>().FromComponentsInHierarchy().AsSingle();
        Container.Bind<PlayerInfoArea>().FromComponentsInHierarchy().AsSingle();
        Container.Bind<UserProfilePopUp>().FromComponentsInHierarchy().AsSingle();

    }

    private void BindSignals()
    {
        SignalBusInstaller.Install(Container);
        
        Container.DeclareSignal<OnACardPlayedSignal>();
        Container.DeclareSignal<OnJoinedTableSignal>();
        Container.DeclareSignal<OnGameSaloonCreationSelectedSignal>();
        Container.DeclareSignal<OnGameSaloonCreationCanceledSignal>();
        Container.DeclareSignal<OnClickedConfirmationButtonSignal>();
        Container.DeclareSignal<OnNewGameChoosedSignal>();
        Container.DeclareSignal<OnBackToLobbyChoosedSignal>();
        Container.DeclareSignal<OnEnableScrollPanelSignal>();

    }
}