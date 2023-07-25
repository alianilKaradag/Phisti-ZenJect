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

    public static UnityAction OnNewGameChoosed;
    public static UnityAction OnBackToLobbyChoosed;
    public bool IsOpen { get; private set; }

    [SerializeField, Foldout("Setup")] private Transform contents;
    [SerializeField, Foldout("Setup")] private Transform optionsButtonImage;

    private Tween moveTween;
    private OptionType optionType;
    private BoardManager boardManager;
    private ConfirmationPopUp confirmationPopUp;

    private const float contentsVisibleLocalXPos = -260;
    private const float contentsUnvisibleLocalXPos = 640;

    [Inject]
    public void Construct(BoardManager boardManager, ConfirmationPopUp confirmationPopUp)
    {
        this.boardManager = boardManager;
        this.confirmationPopUp = confirmationPopUp;
    }
    
    private void Start()
    {
        contents.gameObject.SetActive(false);
        confirmationPopUp.OnClickedConfirmationButton += ConfirmationButtonClicked;
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
        OnNewGameChoosed?.Invoke();
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
        OnBackToLobbyChoosed?.Invoke();
    }

    private void ConfirmationButtonClicked(bool isAccepted)
    {
        if (isAccepted)
        {
            if (optionType == OptionType.NewGame)
            {
                Hide(() => SetOpenableStatus(true));
                OnNewGameChoosed?.Invoke();
            }
            else
            {
                Hide(() => SetOpenableStatus(false));
                OnBackToLobbyChoosed?.Invoke();
            }
        }
    }
}