using System;
using PlayerInfo;
using UnityEngine;
using PlayerInfo;

public class InteractivitySystem : MonoBehaviour
{
    public static InteractivitySystem reference;

    /// <summary>
    /// Player Based Interaction Components
    /// </summary>
    public Collider2D interactCollider;

<<<<<<< HEAD
    public TextAsset inkJson;
=======
    [NonSerialized] public bool canInteract = true;
>>>>>>> main

    /// <summary>
    /// Detection/Flags for Interaction
    /// </summary>
    private bool npcCollision = false;

    private bool itemCollision = false;
    private GameObject itemObject = null;
    private GameObject npcObject = null;

    void Start()
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
<<<<<<< HEAD
        if (Input.GetKeyUp(Player.interactKey))
=======
        if (Input.GetKeyUp(Player.InteractKey) && canInteract &&
            !PlayerController.playerControllerReference.WalkingGrid)
>>>>>>> main
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
        DialogueManager.instance.EnterDialogueMode(inkJson);
        npcObject = null;
    }

    //Logic for item interaction
    private void itemInteraction()
    {
        Debug.Log("Interacting with item.");
        onKeyItems();
        Destroy(itemObject);
        itemObject = null;
    }

    //Update player questProgression
    private void onKeyItems()
    {
        if(itemObject.gameObject.name == "Keys")
        {
            Debug.Log("Congratulations Keys Found!");
            Player.collectedItems = Item.Keys;
        }
    }

    /// <summary>
    /// Updates collision flags per walking towards and walking away
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController.playerControllerReference.WalkBlocked |= PlayerController.WalkBlockedFlags.DirectionBlocked;
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
        PlayerController.playerControllerReference.WalkBlocked &= ~PlayerController.WalkBlockedFlags.DirectionBlocked;
        if (itemCollision && collision.gameObject.CompareTag("Item"))
            itemCollision = false;
        else if (npcCollision && collision.gameObject.CompareTag("NPC"))
            npcCollision = false;
    }
}