using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class SaloonController : MonoBehaviour
{
    [SerializeField] private SaloonType saloonType;

    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI betText;
    [SerializeField] private Button playButton;

    [Inject]private DataManager dataManager;
    [Inject]private SaloonManager saloonManager;
    [Inject]private SignalBus signalBus;
    
    private int minBet;
    private int maxBet;
    
    private void Awake()
    {
        SetValues();
    }

    private void Start()
    {
        signalBus.Subscribe<OnEnableScrollPanelSignal>(x => CheckMoneyForTable());
    }


    private void CheckMoneyForTable()
    {
        if (dataManager.PlayerData.PlayerTotalMoney < minBet)
        {
            playButton.interactable = false;
        }
    }

    private void SetValues()
    {
        var values = saloonManager.GetSaloonValues(saloonType);
        minBet = values[0];
        maxBet = values[1];
        var finalMinText = Helper.ConvertBigValue(minBet);
        var finalMaxText = Helper.ConvertBigValue(maxBet);
        
        betText.text = $"<sprite=0>{finalMinText} - {finalMaxText}";
        title.text = saloonType.ToString().ToUpper();
    }


    public void OnPlayButtonClicked()
    {
        var tableData = new TableData(saloonType, 2, minBet);
        signalBus.TryFire(new OnJoinedTableSignal(tableData));
    }

    public void OnCreateClicked()
    {
        signalBus.TryFire(new OnGameSaloonCreationSelectedSignal(saloonType, minBet, maxBet));
    }
}
