using System;
using System.Collections;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public class ScrollManager : MonoBehaviour
{
   public static UnityAction OnEnable;
   [SerializeField, Foldout("Settings")] private float appearDuration;
   [SerializeField, Foldout("Settings")] private float disappearDuration;
   
   [SerializeField, Foldout("Setup")] private CanvasGroup canvasGroup;

   private Tween fadeTween;
   private bool isOpen = true;
   
   private IEnumerator Start()
   {
      canvasGroup.alpha = 1f;
      OptionsMenu.OnBackToLobbyChoosed += BackToLobby;

      yield return new WaitForSeconds(0.1f);
      OnEnable?.Invoke();
   }

   private void OnDestroy()
   {
      OptionsMenu.OnBackToLobbyChoosed -= BackToLobby;
   }

   public void Open()
   {
      if (isOpen) return;

      KillFadeTween();
      isOpen = true;
      OnEnable?.Invoke();
      canvasGroup.blocksRaycasts = true;
      fadeTween = canvasGroup.DOFade(1f, appearDuration).SetEase(Ease.InQuint);
   }

   public void Close()
   {
      if (!isOpen) return;

      KillFadeTween();
      isOpen = false;
      canvasGroup.blocksRaycasts = false;
      fadeTween = canvasGroup.DOFade(0f, disappearDuration);

   }

   private void KillFadeTween()
   {
      fadeTween.Kill();
      fadeTween = null;
   }

   private void BackToLobby()
   {
      StartCoroutine(BackToLobbyRoutine());
   }

   private IEnumerator BackToLobbyRoutine()
   {
      yield return new WaitForSeconds(1f);
      Open();
   }
}
