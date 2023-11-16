using System;
using System.Collections;
using System.Collections.Generic;
using PlayerInfo;
using UnityEngine;
using UnityEngine.Serialization;

public class InteractivitySystem : MonoBehaviour
{
    public static InteractivitySystem reference;
    
    /// <summary>
    /// Player Based Interaction Components
    /// </summary>
    
    public Collider2D interactCollider;

    [NonSerialized] public bool canInteract = true;
    
    /// <summary>
    /// Detection/Flags for Interaction
    /// </summary>
    private bool npcCollision = false;
    private bool itemCollision = false;
    private GameObject itemObject = null;
    private GameObject npcObject = null;

    private void Start()
    {
        if (reference is not null)
        {
            Destroy(this);
            return;
        }

        reference = this;
    }

    //Set update to constantly check for player input
    void Update()
    {
        if (Input.GetKeyUp(Player.InteractKey) && canInteract)
        {
            interactionType();
        }
    }

    private void OnDestroy()
    {
        if (reference == this)
        {
            reference = null;
        }
    }

    // Based on the situation, the function will handle what type of interaction to carry out
    public void interactionType()
    {
        if (itemCollision)
            itemInteraction();
        else if (npcCollision)
            npcInteraction();
        else
            Debug.Log("There is nothing here!");
    }

    //Logic for NPC interaction
    void npcInteraction()
    {
        Debug.Log("Interacting with an NPC.");
        npcObject = null;
    }

    //Logic for item interaction
    private void itemInteraction()
    {
        Debug.Log("Interacting with item.");
        Destroy(itemObject);
        itemObject = null;
    }

    /// <summary>
    /// Updates collision flags per walking towards and walking away
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Item"))
        {
            itemCollision = true;
            itemObject = collision.gameObject;
        }
        else if (collision.gameObject.CompareTag("NPC"))
        {
            npcCollision = true;
            npcObject = collision.gameObject;
        }
        else
        {
            itemCollision = false;
            npcCollision = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (itemCollision && collision.gameObject.CompareTag("Item"))
            itemCollision = false;
        else if (npcCollision && collision.gameObject.CompareTag("NPC"))
            npcCollision = false;
    }
}
