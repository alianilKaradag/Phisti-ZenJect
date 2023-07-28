using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class InputHandler : MonoBehaviour
{
   private CharacterController playerController;
   private BoardManager boardManager;
   private CardDealer cardDealer;

   [Inject]
   public void Construct(BoardManager boardManager, [Inject(Id = "PlayerController")] CharacterController playerController, CardDealer cardDealer)
   {
      this.boardManager = boardManager;
      this.playerController = playerController;
      this.cardDealer = cardDealer;
   }
   
   
   private void Update()
   {
      if (!playerController.IsMyTurn) return;
      if (cardDealer.IsDealingCards) return;
         
      ClickCheck();
   }

   private void ClickCheck()
   {
      if(Input.GetMouseButtonDown(0))
      {
         Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
         RaycastHit hit;
         
         if (Physics.Raycast (ray, out hit, Mathf.Infinity))
         {
            var card = hit.transform.GetComponentInChildren<Card>();

            if (card == null) return;
               
            playerController.PlayCard(card);
         }    
      }    
   }
}
