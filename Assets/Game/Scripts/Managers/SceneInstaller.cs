using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SceneInstaller : MonoInstaller<SceneInstaller>
{
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<GameManager>().AsSingle().NonLazy();
        Container.Bind<UIPanelManager>().FromComponentInHierarchy().AsSingle();
        Container.Bind<BoardManager>().FromComponentsInHierarchy().AsSingle();
        Container.BindFactory<Card, Card, Card.Factory>();
        Container.Bind<CameraManager>().FromComponentsInHierarchy().AsSingle();
        Container.Bind<CharacterManager>().FromComponentsInHierarchy().AsSingle();
        Container.Bind<DataManager>().FromComponentsInHierarchy().AsSingle();
        Container.Bind<SaloonCreator>().FromComponentsInHierarchy().AsSingle();
        Container.Bind<SaloonManager>().FromComponentsInHierarchy().AsSingle();
        Container.Bind<ScrollManager>().FromComponentsInHierarchy().AsSingle();
        Container.Bind<ConfirmationPopUp>().FromComponentsInHierarchy().AsSingle();
        Container.Bind<GameCompletePopUp>().FromComponentsInHierarchy().AsSingle();
        Container.Bind<OptionsMenu>().FromComponentsInHierarchy().AsSingle();
        Container.Bind<PlayerInfoArea>().FromComponentsInHierarchy().AsSingle();
        Container.Bind<UserProfilePopUp>().FromComponentsInHierarchy().AsSingle();
        Container.Bind<ScoreManager>().AsSingle();
        Container.Bind<InputHandler>().AsSingle();
        Container.Bind<RandomUserInfoGenerator>().AsSingle();
    }

}