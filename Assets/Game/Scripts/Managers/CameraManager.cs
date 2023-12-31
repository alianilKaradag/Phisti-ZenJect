﻿using System;
using UnityEngine;
using Cinemachine;
using DG.Tweening;
using Zenject;

public enum CameraType
{
    Intro,Gameplay
}

public class CameraManager : MonoBehaviour
{
    public Camera MainCamera => mainCamera;
   
    [SerializeField] private CameraType defaultCamera;
    [SerializeField] private CinemachineVirtualCamera[] cameras;
    
    [Inject(Id = "Main")]private Camera mainCamera;
    [Inject]private SignalBus signalBus;

    public CinemachineVirtualCamera CurrentCamera { get; private set; }

    private CameraType currentCameraType,previousCamera;
    private CinemachineBasicMultiChannelPerlin currentShake;

   
    private void Start()
    {
        previousCamera = defaultCamera;
        ChangeCamera(defaultCamera);
        signalBus.Subscribe<OnBackToLobbyChoosedSignal>(x => BackToLobby());
    }

    private void UpdateCurrentCamera()
    {
        for (var i = 0; i < cameras.Length; i++)
        {
            var isCamOn = (int)currentCameraType == i;
            var cam = cameras[i];
            
            //This assignment operstor (=) is intentional.
            if (cam.enabled = isCamOn)
            {
                CurrentCamera = cam;
                currentShake = CurrentCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            }
        }
    }
    
    public void ChangeCamera(CameraType cam)
    {
        previousCamera = currentCameraType;
        currentCameraType = cam;
        UpdateCurrentCamera();
    }

    private void BackToLobby()
    {
        ChangeCamera(CameraType.Intro);
    }
}
