using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public enum SaloonType
{
    NewBies, Rookies, Nobles
}

public class SaloonManager : Singleton<SaloonManager>
{
    public static UnityAction<TableData> OnJoinedTable;
    public static UnityAction<SaloonType, int, int> OnGameSaloonCreateSelected;
    public static UnityAction OnGameSaloonCreationCanceled;
    
    [SerializeField, Foldout("Setup")] private SaloonTypeValues saloonTypeValues;

    private TableData tableData;
    private SaloonType currentSaloonType;

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
        ScrollManager.Instance.Close();
        SaloonCreator.Instance.OpenCreateTable(saloonType, minBet, maxBet);
    }

    private void JoinedTable(TableData tableData)
    {
        this.tableData = tableData;
        PlayerInfoArea.Instance.Close();
        ScrollManager.Instance.Close();
        CameraManager.Instance.ChangeCamera(CameraType.Gameplay);
        OptionsMenu.Instance.SetOpenableStatus(true);
    }

    private void SaloonCreationCanceled()
    {
        ScrollManager.Instance.Open();
        PlayerInfoArea.Instance.Open();
    }
}
