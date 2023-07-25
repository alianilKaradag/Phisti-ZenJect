using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class UserProfilePopUp :MonoBehaviour
{
    [SerializeField, Foldout("Settings")] private float appearDuration = 0.3f;
    [SerializeField, Foldout("Settings")] private float disappearDuration = 0.3f;
    
    [SerializeField, Foldout("Setup")] private TextMeshProUGUI winAmountText;
    [SerializeField, Foldout("Setup")] private TextMeshProUGUI lostAmountText;
    [SerializeField, Foldout("Setup")] private UserInfo insideUserInfo;
    [SerializeField, Foldout("Setup")] private Transform contents;

    private Tween scaleTween;

    private bool isOpen;

    private void Awake()
    {
        contents.localScale = Vector3.zero;
        contents.gameObject.SetActive(false);
    }
    
    public void Open(UserInfoData userInfoData)
    {
        SetValues(userInfoData);
        
        if (isOpen) return;
        
        KillScaleTween();

        isOpen = true;
        contents.localScale = Vector3.zero;
        contents.gameObject.SetActive(true);
        scaleTween = contents.DOScale(1f, appearDuration).SetEase(Ease.OutBack);
    }

    private void Close()
    {
        if (!isOpen) return;

        KillScaleTween();

        isOpen = false;
        scaleTween = contents.DOScale(0f, disappearDuration).SetEase(Ease.InBack)
            .OnComplete(() => { contents.gameObject.SetActive(false); });
    }

    private void SetValues(UserInfoData userInfoData)
    {
        insideUserInfo.SetData(userInfoData);
        winAmountText.text = userInfoData.WinAmount.ToString();
        lostAmountText.text = userInfoData.LostAmount.ToString();
    }
    
    public void OnExitButtonClicked()
    {
        Close();
    }
    
    private void KillScaleTween()
    {
        scaleTween.Kill();
        scaleTween = null;
    }
}
