using System;
using System.Collections;
using System.Collections.Generic;
using PlayerInfo;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Stairs : MonoBehaviour
{
   public int stairIndex;
   public bool _isFloorTwo;
   
   private PlayerPosition _position;

   private void Start()
   {
      _position = SceneManager.GetActiveScene().name == "FloorOne"? StageData.FloorOneToFloorTwoPos[stairIndex]: StageData.FloorTwoToFloorOnePos[stairIndex];
   }
   
   private void OnTriggerEnter2D(Collider2D col)
   {
      if (!col.gameObject.CompareTag("Player"))
      {
         return;
      }
      LoadingOverlay.Reference.Show();
      PlayerController.playerControllerReference.transform.position = _position.Position;
      Player.FacingDirection = _position.direction;
      PlayerController.playerControllerReference.StopWalking();
      SceneManager.LoadScene(_isFloorTwo? "FloorOne":"FloorTwo");
   }
}
