using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaloonController : MonoBehaviour
{
    [SerializeField] private SaloonType saloonType;

    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI betText;
    [SerializeField] private Button playButton;

    private int minBet;
    private int maxBet;
    
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
        if (DataManager.Instance.PlayerData.PlayerTotalMoney < minBet)
        {
            playButton.interactable = false;
        }
    }

    private void SetValues()
    {
        var values = SaloonManager.Instance.GetSaloonValues(saloonType);
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
