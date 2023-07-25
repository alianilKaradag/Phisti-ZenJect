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

    private DataManager dataManager;
    private SaloonManager saloonManager;
    private int minBet;
    private int maxBet;
    
    [Inject]
    public void Construct(DataManager dataManager, SaloonManager saloonManager)
    {
        this.dataManager = dataManager;
        this.saloonManager = saloonManager;
    }
    private void Awake()
    {
        SetValues();
    }

    private void Start()
    {
        ScrollManager.OnEnable += CheckMoneyForTable;
    }

    private void OnDestroy()
    {
        ScrollManager.OnEnable -= CheckMoneyForTable;
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
        SaloonManager.OnJoinedTable?.Invoke(tableData);
    }

    public void OnCreateClicked()
    {
        SaloonManager.OnGameSaloonCreateSelected?.Invoke(saloonType, minBet, maxBet);
    }
}
