using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

public class ConfirmationPopUp : MonoBehaviour
{
    [SerializeField] private Transform contents;
    [SerializeField] private Button acceptBtn;
    [SerializeField] private Button refuseBtn;

    [Inject] private SignalBus signalBus;
    
    private Tween scaleTween;
    private bool isOpen;
    
    private void Start()
    {
        contents.gameObject.SetActive(false);
        contents.transform.localScale = Vector3.zero;
    }

    public void Open()
    {
        if (isOpen) return;

        KillTweens();
       
        isOpen = true;
        contents.gameObject.SetActive(true);
        contents.DOScale(1f, 0.2f)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                SetButtonsInteractableStatus(true);
            });
    }
    

    private void Close()
    {
        if (!isOpen) return;
            
        KillTweens();
        SetButtonsInteractableStatus(false);
        
        isOpen = false;
        contents.DOScale(0f, 0.2f)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                contents.gameObject.SetActive(false);
            });
    }

    public void OnAcceptClicked()
    {
        Close();
        signalBus.TryFire(new OnClickedConfirmationButtonSignal(true));
    }
    
    public void OnRefuseClicked()
    {
        Close();
        signalBus.TryFire(new OnClickedConfirmationButtonSignal(false));
    }

    private void KillTweens()
    {
        scaleTween.Kill();
        scaleTween = null;
    }

    private void SetButtonsInteractableStatus(bool isOpen)
    {
        acceptBtn.interactable = isOpen;
        refuseBtn.interactable = isOpen;
    }

}
