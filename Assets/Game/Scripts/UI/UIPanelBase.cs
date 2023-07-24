using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public class UIPanelBase : MonoBehaviour
{
    [SerializeField,Foldout("Setup")] protected CanvasGroup canvasGroup;
    
    [SerializeField, Foldout("Settings")] protected float appearTime = 0.5f, appearDelay = 0f;
    [SerializeField, Foldout("Settings")] protected float disappearTime = 0.5f, disappearDelay = 0f;

    public virtual void Appear(bool isInstant = false, UnityAction OnComplete = null, float delay = 0f)
    {
        gameObject.SetActive(true);

        if (isInstant)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = canvasGroup.blocksRaycasts = true;
            OnAppearEnd();
            OnComplete?.Invoke();
            return;
        }
        
        canvasGroup.DOFade(1f, appearTime)
            .SetDelay(delay + appearDelay)
            .SetEase(Ease.Linear)
            .OnStart(()=> OnAppearStart())
            .OnComplete(() =>
            {
                canvasGroup.interactable = canvasGroup.blocksRaycasts = true;
                OnAppearEnd();
                OnComplete?.Invoke();
            });
    }

    public virtual void Disappear(bool isInstant = false, UnityAction OnComplete = null, float delay = 0f)
    {
        if (isInstant)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = canvasGroup.blocksRaycasts = false;
            OnDisappearEnd();
            gameObject.SetActive(false);
            return;            
        }
        
        canvasGroup.DOFade(0f, disappearTime)
            .SetDelay(delay + disappearDelay)
            .SetEase(Ease.Linear)
            .OnStart(() =>
            {
                canvasGroup.interactable = canvasGroup.blocksRaycasts = false;
                OnDisappearStart();
            })
            .OnComplete(() =>
            {
                OnDisappearEnd();
                gameObject.SetActive(false);
            });
    }
    
    public virtual void OnAppearStart() { }
    public virtual void OnAppearEnd() { }
    public virtual void OnDisappearStart() { }
    public virtual void OnDisappearEnd() { }
}

