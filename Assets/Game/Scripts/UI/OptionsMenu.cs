using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

public class OptionsMenu : MonoBehaviour
{
    private enum OptionType
    {
        NewGame,
        BackToLobby
    }

    public bool IsOpen { get; private set; }

    [SerializeField, Foldout("Setup")] private Transform contents;
    [SerializeField, Foldout("Setup")] private Transform optionsButtonImage;

    [Inject] private BoardManager boardManager;
    [Inject] private ConfirmationPopUp confirmationPopUp;
    [Inject] private SignalBus signalBus;
    
    private Tween moveTween;
    private OptionType optionType;

    private const float contentsVisibleLocalXPos = -260;
    private const float contentsUnvisibleLocalXPos = 640;
    
    
    private void Start()
    {
        contents.gameObject.SetActive(false);
        signalBus.Subscribe<OnClickedConfirmationButtonSignal>(x => ConfirmationButtonClicked(x.IsAccepted));
    }

    public void SetOpenableStatus(bool canOpen)
    {
        contents.gameObject.SetActive(canOpen);
        contents.transform.localPosition = Vector3.right * contentsUnvisibleLocalXPos;
    }

    public void Show()
    {
        KillAllTweens();

        IsOpen = true;
        contents.DOLocalMoveX(contentsVisibleLocalXPos, 0.2f);
        optionsButtonImage.localRotation = Quaternion.Euler(Vector3.forward * 0);
    }

    private void Hide(UnityAction onComplete = null)
    {
        KillAllTweens();

        IsOpen = false;
        optionsButtonImage.localRotation = Quaternion.Euler(Vector3.forward * 180);
        contents.DOLocalMoveX(contentsUnvisibleLocalXPos, 0.2f)
            .OnComplete(() => { onComplete?.Invoke(); });
    }


    private void KillAllTweens()
    {
        moveTween.Kill();
        moveTween = null;
    }

    public void OnOptionsClicked()
    {
        if (IsOpen)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }

    public void OnNewGameClicked()
    {
        optionType = OptionType.NewGame;

        if (!boardManager.IsGameCompleted)
        {
            confirmationPopUp.Open();
            return;
        }
        
        Hide(() => SetOpenableStatus(true));
        signalBus.TryFire(new OnNewGameChoosedSignal());
    }

    public void OnBackToLobbyClicked()
    {
        optionType = OptionType.BackToLobby;

        if (!boardManager.IsGameCompleted)
        {
            confirmationPopUp.Open();
            return;
        }
        
        Hide(() => SetOpenableStatus(false));
        signalBus.TryFire(new OnBackToLobbyChoosedSignal());
    }

    private void ConfirmationButtonClicked(bool isAccepted)
    {
        if (isAccepted)
        {
            if (optionType == OptionType.NewGame)
            {
                Hide(() => SetOpenableStatus(true));
                signalBus.TryFire(new OnNewGameChoosedSignal());
            }
            else
            {
                Hide(() => SetOpenableStatus(false));
                signalBus.TryFire(new OnBackToLobbyChoosedSignal());
            }
        }
    }
}