using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class InputHandler : MonoBehaviour
{
   [SerializeField] private CharacterController characterController;

   private BoardManager boardManager;

   [Inject]
   public void Construct(BoardManager boardManager)
   {
      this.boardManager = boardManager;
   }
   
   private void Update()
   {
      if (!characterController.IsMyTurn) return;
      if (boardManager.IsDealingCards) return;
         
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
               
			   characterController.PlayCard(card);
         }    
      }    
   }
}
