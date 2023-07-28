using System;
using UnityEditor;
using UnityEngine;
using Zenject;

public class DataManager : IDisposable
{
    public PlayerData PlayerData { get; private set; }

    [Inject]
    public DataManager(PlayerData playerData)
    {
        PlayerData = playerData;
    }
    
    public void Dispose()
    {
        SaveData();
    }
    
    public void UpdateMoney(int totalMoney)
    {
        PlayerData.PlayerTotalMoney += totalMoney;
    }
    
    public void IncreaseWinAmount()
    {
        PlayerData.WinAmount++;
    }
    
    public void IncreaseLostAmount()
    {
        PlayerData.LostAmount++;
    }

#if UNITY_EDITOR
    private void SaveData()
    {
        var transferData = ScriptableObject.CreateInstance<PlayerData>();
        var savePath = "Assets/Game/Scriptables/PlayerData" + ".asset";

        transferData.PlayerTotalMoney = PlayerData.PlayerTotalMoney;
        transferData.WinAmount = PlayerData.WinAmount;
        transferData.LostAmount = PlayerData.LostAmount;
        
        AssetDatabase.CreateAsset(transferData, savePath);
        PlayerData = (PlayerData) AssetDatabase.LoadAssetAtPath(savePath, typeof(PlayerData));
        
        EditorUtility.SetDirty(PlayerData);
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = transferData;
    }
#endif
    
}