using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public enum CardType
{
    Heart,
    Diamond,
    Club,
    Spade
}

public enum CardValue
{
    One,
    Two,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
    Ten,
    Joker,
    Queen,
    King
}

public class Card : MonoBehaviour
{
    public CardType CardType => cardType;
    public CardValue CardValue => cardValue;

    [SerializeField, Foldout("Settings")] private CardType cardType;
    [SerializeField, Foldout("Settings")] private CardValue cardValue;
    
    private MeshRenderer meshRenderer;
    private Coroutine playCardRoutine;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void FlyToCharacterHand(List<Transform> destinations, UnityAction onComplete = null)
    {
        StartCoroutine(FlyToCharacterHandRoutine(destinations, onComplete));
    }

    private IEnumerator FlyToCharacterHandRoutine(List<Transform> destinations, UnityAction onComplete = null)
    {
        transform.DOLocalRotate(Vector3.zero, 0.4f);

        foreach (var destination in destinations)
        {
            transform.SetParent(destination);
            yield return transform.DOMove(destination.position, 6.5f).SetSpeedBased().WaitForCompletion();
        }

        onComplete?.Invoke();
    }

    public void Play(Transform destination, float yPos, bool isFaceUp = true, UnityAction onComplete = null)
    {
        if (playCardRoutine != null)
        {
            StopCoroutine(playCardRoutine);
            playCardRoutine = null;
        }
        
        playCardRoutine = StartCoroutine(PlayRoutine(destination, yPos, isFaceUp, onComplete));
    }

    private IEnumerator PlayRoutine(Transform destination, float yPos, bool isFaceUp = true, UnityAction onComplete = null)
    {
        transform.SetParent(destination);
        var targetPos = destination.position;
        targetPos.y = yPos;

        Vector3 randomRot;

        if (isFaceUp)
        {
            randomRot = new Vector3(0, Random.Range(0, 20), 0);
        }
        else
        {
            randomRot = new Vector3(0, Random.Range(0, 20), 180);
        }

        transform.rotation = Quaternion.Euler(randomRot);
        yield return transform.DOMove(targetPos, 6f).SetSpeedBased().WaitForCompletion();

        onComplete?.Invoke();
    }

    public void HideCard()
    {
        transform.DOScale(0f, 0.2f).SetDelay(0.2f)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                meshRenderer.enabled = false;
            });
    }

    public void DestroyCard()
    {
        transform.DOKill();
        Destroy(gameObject);
    }

}