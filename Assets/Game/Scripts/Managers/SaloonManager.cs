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
    [Inject] private CameraManager cameraManager;
    [Inject] private SaloonCreator saloonCreator;
    [Inject] private ScrollManager scrollManager;
    [Inject] private OptionsMenu optionsMenu;
    [Inject] private PlayerInfoArea playerInfoArea;
    [Inject] private SaloonTypeValues saloonTypeValues;
    [Inject] private SignalBus signalBus;
    
    private TableData tableData;
    private SaloonType currentSaloonType;
    
    private void Awake()
    {
        signalBus.Subscribe<OnGameSaloonCreationSelectedSignal>(x => CreateSaloonSelected(x.SaloonType, x.MinBet, x.MaxBet));
        signalBus.Subscribe<OnJoinedTableSignal>( x => JoinedTable(x.TableData));
        signalBus.Subscribe<OnGameSaloonCreationCanceledSignal>(x => SaloonCreationCanceled());
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

    public void JoinedTable(TableData tableData)
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
