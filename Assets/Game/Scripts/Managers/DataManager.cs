using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    public PlayerData PlayerData => playerData;
    [SerializeField] private PlayerData playerData;

    private void OnDestroy()
    {
        SaveData();
    }
    
    public void UpdateMoney(int totalMoney)
    {
        playerData.PlayerTotalMoney += totalMoney;
    }
    
    public void IncreaseWinAmount()
    {
        playerData.WinAmount++;
    }
    
    public void IncreaseLostAmount()
    {
        playerData.LostAmount++;
    }
    
    private void SaveData()
    {
        var transferData = ScriptableObject.CreateInstance<PlayerData>();
        var savePath = "Assets/Game/Scriptables/PlayerData" + ".asset";

        transferData.PlayerTotalMoney = playerData.PlayerTotalMoney;
        transferData.WinAmount = playerData.WinAmount;
        transferData.LostAmount = playerData.LostAmount;
        
        AssetDatabase.CreateAsset(transferData, savePath);
        playerData = (PlayerData) AssetDatabase.LoadAssetAtPath(savePath, typeof(PlayerData));
        
        EditorUtility.SetDirty(playerData);
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = transferData;
    }
}