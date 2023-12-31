using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using Zenject;

public class PlayerInfoArea : MonoBehaviour
{
    [SerializeField] private UserInfo userInfo;
    [SerializeField, Foldout("Settings")] private float appearDuration = 0.2f;
    [SerializeField, Foldout("Settings")] private float disappearDuration = 0.2f;
    [SerializeField, Foldout("Setup")] private CanvasGroup canvasGroup;

    [Inject] private DataManager dataManager;
    [Inject] private SignalBus signalBus;
    
    private Tween fadeTween;
    private UserInfoData data;

    private bool isOpen = true;


    
    private void Start()
    {
        UpdateData();
        signalBus.Subscribe<OnBackToLobbyChoosedSignal>(x => Open());
    }

    public void Open()
    {
        if (isOpen) return;

        KillFadeTween();
        
        isOpen = true;
        canvasGroup.interactable = true;
        fadeTween = canvasGroup.DOFade(1f, appearDuration);
    }

    public void Close()
    {
        if (!isOpen) return;

        KillFadeTween();

        isOpen = false;
        canvasGroup.interactable = false;
        fadeTween = canvasGroup.DOFade(0f, disappearDuration);
    }

    public void UpdateData()
    {
        var playerData = dataManager.PlayerData;
        data = new UserInfoData("Player", playerData.WinAmount, playerData.LostAmount, playerData.PlayerTotalMoney);
        userInfo.SetData(data);
    }
    
    private void KillFadeTween()
    {
        fadeTween.Kill();
        fadeTween = null;
    }

}
