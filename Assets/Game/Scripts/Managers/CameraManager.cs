using System;
using UnityEngine;
using Cinemachine;
using DG.Tweening;

public enum CameraType
{
    Intro,Gameplay
}

public class CameraManager : Singleton<CameraManager>
{
    public Camera MainCamera => mainCamera;
   
    [SerializeField] private Camera mainCamera;
    [SerializeField] private CameraType defaultCamera;
    [SerializeField] private CinemachineVirtualCamera[] cameras;

    public CinemachineVirtualCamera CurrentCamera { get; set; }

    private CameraType currentCameraType,previousCamera;
    private CinemachineBasicMultiChannelPerlin currentShake;

    private void Start()
    {
        previousCamera = defaultCamera;
        ChangeCamera(defaultCamera);
        OptionsMenu.OnBackToLobbyChoosed += BackToLobby;
    }

    private void OnDestroy()
    {
        OptionsMenu.OnBackToLobbyChoosed -= BackToLobby;
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
    public void OpenPreviousCamera() => ChangeCamera(previousCamera);
    
    public void ChangeCamera(CameraType cam)
    {
        previousCamera = Instance.currentCameraType;
        currentCameraType = cam;
        UpdateCurrentCamera();
    }

    public void Shake(float time, float intensity)
    {
        if (currentShake == null) return;
        
        var shakeId = "CameraShake";
        DOTween.Kill(shakeId);

        var amplitude = 0.2f * intensity;
        currentShake.m_AmplitudeGain = amplitude;

        DOTween.To(o => currentShake.m_AmplitudeGain = o, amplitude, 0f, time)
            .SetId(shakeId)
            .SetEase(Ease.Linear);
    }

    private void BackToLobby()
    {
        ChangeCamera(CameraType.Intro);
    }
}
