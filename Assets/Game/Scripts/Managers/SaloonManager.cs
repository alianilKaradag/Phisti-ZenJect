using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Zenject;

public enum SaloonType
{
    NewBies, Rookies, Nobles
}

public class SaloonManager : MonoBehaviour
{
    public static UnityAction<TableData> OnJoinedTable;
    public static UnityAction<SaloonType, int, int> OnGameSaloonCreateSelected;
    public static UnityAction OnGameSaloonCreationCanceled;
    
    private TableData tableData;
    private SaloonType currentSaloonType;
    private SaloonTypeValues saloonTypeValues;
    private CameraManager cameraManager;
    private SaloonCreator saloonCreator;
    private ScrollManager scrollManager;
    private OptionsMenu optionsMenu;
    private PlayerInfoArea playerInfoArea;

    [Inject]
    public void Construct(CameraManager cameraManager, SaloonCreator saloonCreator, ScrollManager scrollManager, OptionsMenu optionsMenu, PlayerInfoArea playerInfoArea, SaloonTypeValues saloonTypeValues)
    {
        this.cameraManager = cameraManager;
        this.saloonCreator = saloonCreator;
        this.scrollManager = scrollManager;
        this.optionsMenu = optionsMenu;
        this.playerInfoArea = playerInfoArea;
        this.saloonTypeValues = saloonTypeValues;
    }
    
    private void Awake()
    {
        OnGameSaloonCreateSelected += CreateSaloonSelected;
        OnJoinedTable += JoinedTable;
        OnGameSaloonCreationCanceled += SaloonCreationCanceled;
    }

    private void OnDestroy()
    {
        OnGameSaloonCreateSelected -= CreateSaloonSelected;
        OnJoinedTable -= JoinedTable;
        OnGameSaloonCreationCanceled -= SaloonCreationCanceled;
    }

    public List<int> GetSaloonValues(SaloonType saloonType)
    {
        List<int> values = new List<int>();
        
        switch (saloonType)
        {
            case SaloonType.NewBies:
                values.Add(saloonTypeValues.NewBie_MinBet);
                values.Add(saloonTypeValues.NewBie_MaxBet);
                break;
            case SaloonType.Rookies:
                values.Add(saloonTypeValues.Rokie_MinBet);
                values.Add(saloonTypeValues.Rokie_MaxBet);
                break;
            case SaloonType.Nobles:
                values.Add(saloonTypeValues.Noble_MinBet);
                values.Add(saloonTypeValues.Noble_MaxBet);
                break;
        }

        return values;
    }

    private void CreateSaloonSelected(SaloonType saloonType, int minBet, int maxBet)
    {
        currentSaloonType = saloonType;
        scrollManager.Close();
        saloonCreator.OpenCreateTable(saloonType, minBet, maxBet);
    }

    private void JoinedTable(TableData tableData)
    {
        this.tableData = tableData;
        playerInfoArea.Close();
        scrollManager.Close();
        cameraManager.ChangeCamera(CameraType.Gameplay);
        optionsMenu.SetOpenableStatus(true);
    }

    private void SaloonCreationCanceled()
    {
        scrollManager.Open();
        playerInfoArea.Open();
    }
}
