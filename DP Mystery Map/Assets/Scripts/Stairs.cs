using System;
using System.Collections;
using System.Collections.Generic;
using PlayerInfo;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Stairs : MonoBehaviour
{
   public int stairIndex;

   private PlayerPosition _position;

   private void Start()
   {
      _position = StageData.FloorOneToFloorTwoPos[stairIndex];
   }

   private void OnTriggerEnter2D(Collider2D col)
   {
      if (!col.CompareTag("Player")) return;
      PlayerController.playerControllerReference.transform.position = _position.Position;
      Player.FacingDirection = _position.direction;
      SceneManager.LoadScene("FloorTwo");
   }
}
