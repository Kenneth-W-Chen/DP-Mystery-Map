using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractivitySystem : MonoBehaviour
{
    public Transform objPoint;
    private const float objSize = 0.2f;
    public LayerMask objLayer;

    void Update()
    {
        if(returnInteraction())
        {
            Debug.Log("Interact");
        }
    }

    bool returnInteraction()
    {
        return Input.GetKeyUp(KeyCode.P);
    }

    //Based on situation will handle what type of interaction to carry-out
    void interactionType()
    {

    }

    //Logic for NPC interaction
    void npcInteraction()
    {

    }

    //Logic for item interaction
    void itemInteraction()
    {

    }
}
