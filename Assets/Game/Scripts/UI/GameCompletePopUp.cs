using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameCompletePopUp : MonoBehaviour
{
   [SerializeField, Foldout("Settings")] private float appearDuration = 0.4f;
   
   [SerializeField, Foldout("Setup")] private CanvasGroup canvasGroup;
   [SerializeField, Foldout("Setup")] private Transform popUp;
   [SerializeField, Foldout("Setup")] private TextMeshProUGUI winnerNameText;
   [SerializeField, Foldout("Setup")] private TextMeshProUGUI betAmountText;
   [SerializeField, Foldout("Setup")] private Image winnerImage;
   [SerializeField, Foldout("Setup")] private Image popUpBGImage;
   [SerializeField, Foldout("Setup")] private Sprite winnerSprite;
   [SerializeField, Foldout("Setup")] private Sprite lostSprite;
   [SerializeField, Foldout("Setup")] private Color winColor;
   [SerializeField, Foldout("Setup")] private Color lostColor;
   

   private Tween fadeTween;
   private Tween scaleTween;

   private bool isOpen;

   private void Start()
   {
      canvasGroup.alpha = 0f;
      canvasGroup.interactable = false;

      OptionsMenu.OnBackToLobbyChoosed += Close;
      OptionsMenu.OnNewGameChoosed += Close;
   }

   private void OnDestroy()
   {
      OptionsMenu.OnBackToLobbyChoosed -= Close;
      OptionsMenu.OnNewGameChoosed -= Close;
   }

   public void SetValues(string name, int betAmount, bool didPlayerWin)
   {
      winnerNameText.text = name;
      betAmountText.text = "<sprite=0>" + betAmount;
      SetVisuals(didPlayerWin);
   }

   public void Open()
   {
      if (isOpen) return;
         
      KillTweens();

      isOpen = true;
      popUp.localScale = Vector3.zero;
      fadeTween = canvasGroup.DOFade(1f, appearDuration);
      scaleTween = popUp.DOScale(Vector3.one, appearDuration - 0.2f)
         .SetDelay(0.2f)
         .SetEase(Ease.OutBack, 2f);
   }

   public void Close()
   {
      if (!isOpen) return;
         
      KillTweens();

      isOpen = false;
      fadeTween = canvasGroup.DOFade(0f, 0.2f).SetDelay(0.2f);
      scaleTween = popUp.DOScale(Vector3.zero, 0.2f)
         .SetEase(Ease.InBack);
   }
   
   private void KillTweens()
   {
      if (fadeTween != null)
      {
         fadeTween.Kill();
         fadeTween = null;
      }

      if (scaleTween != null)
      {
         scaleTween.Kill();
         scaleTween = null;
      }
   }

   private void SetVisuals(bool didPlayerWin)
   {
      if (didPlayerWin)
      {
         winnerImage.sprite = winnerSprite;
         popUpBGImage.color = winColor;
         return;
      }
      
      winnerImage.sprite = lostSprite;
      popUpBGImage.color = lostColor;
   }
}
