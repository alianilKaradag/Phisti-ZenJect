using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class SaloonCreator : MonoBehaviour
{
    [SerializeField, Foldout("Settings")] private float appearDuration = 0.5f;
    [SerializeField, Foldout("Settings")] private float disappearDuration = 0.5f;

    [SerializeField, Foldout("Setup")] private Slider betSlider;
    [SerializeField, Foldout("Setup")] private Toggle toggle_2Player;
    [SerializeField, Foldout("Setup")] private Toggle toggle_4Player;
    [SerializeField, Foldout("Setup")] private Transform contents;
    [SerializeField, Foldout("Setup")] private TextMeshProUGUI currentBetText;
    [SerializeField, Foldout("Setup")] private TextMeshProUGUI minBetText;
    [SerializeField, Foldout("Setup")] private TextMeshProUGUI maxBetText;
    [SerializeField, Foldout("Setup")] private Button createButton;

    [Inject] private DataManager dataManager;
    [Inject] private SignalBus signalBus;
    
    private SaloonType saloonType;
    private Tween scaleTween;
    private int minBet;
    private int maxBet;
    private int currentBet;
    private bool isOpen;
    private int playerTotalMoney => dataManager == null ? 0 : dataManager.PlayerData.PlayerTotalMoney;

    private void Awake()
    {
        contents.transform.localScale = Vector3.zero;
        contents.gameObject.SetActive(false);
    }

    public void OpenCreateTable(SaloonType saloonType, int minBet, int maxBet)
    {
        this.saloonType = saloonType;
        CheckBets(minBet, maxBet);

        if (isOpen) return;

        KillScaleTween();
        isOpen = true;
        contents.gameObject.SetActive(true);
        scaleTween = contents.DOScale(1f, appearDuration).SetEase(Ease.OutBack);
    }

    private void CloseCreateTable()
    {
        if (!isOpen) return;

        isOpen = false;
        KillScaleTween();
        scaleTween = contents.DOScale(0f, disappearDuration).SetEase(Ease.InBack)
            .OnComplete(() => { contents.gameObject.SetActive(false); });
    }

    public void OnCreateClicked()
    {
        CloseCreateTable();
        SetCreateButtonInteractable(false);
        var tableData = new TableData(saloonType, toggle_2Player.isOn ? 2 : 4, currentBet);
        signalBus.TryFire(new OnJoinedTableSignal(tableData));
    }

    public void OnToggle_2PlayerClicked()
    {
        toggle_4Player.isOn = false;
    }

    public void OnToggle_4PlayerClicked()
    {
        toggle_2Player.isOn = false;
    }
    private void CheckBets(int minBet, int maxBet)
    {
        this.minBet = minBet;
        this.maxBet = maxBet;

        SetText(minBetText,Helper.ConvertBigValue(this.minBet));
        SetText(maxBetText,Helper.ConvertBigValue(this.maxBet));
        betSlider.interactable = true;
        
        if (minBet > playerTotalMoney)
        {
            betSlider.interactable = false;
            betSlider.value = 0f;
            SetText(currentBetText, "Not Enough Money");
            SetCreateButtonInteractable(false);
            return;
        }

        CalculateCurrentBet();
    }

    public void CalculateCurrentBet()
    {
        var bet = Mathf.Lerp(minBet, maxBet, betSlider.value);
        currentBet = Convert.ToInt32(bet / 500);
        currentBet *= 500;
        
        SetText(currentBetText, Helper.ConvertBigValue(currentBet));

        if (currentBet > playerTotalMoney)
        {
            SetCreateButtonInteractable(false);
            return;
        }

        createButton.interactable = true;
    }

    private void SetCreateButtonInteractable(bool isOpen)
    {
        createButton.interactable = isOpen;
    }
    
    public void OnExitButtonClicked()
    {
        CloseCreateTable();
        signalBus.TryFire(new OnGameSaloonCreationCanceledSignal());
    }

    private void KillScaleTween()
    {
        scaleTween.Kill();
        scaleTween = null;
    }

    private void SetText(TextMeshProUGUI textmesh, string text)
    {
        textmesh.text = "<sprite=0>" + text;
    }
}