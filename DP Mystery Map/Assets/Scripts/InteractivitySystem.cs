using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractivitySystem : MonoBehaviour
{
    /// <summary>
    /// Player Based Interaction Components
    /// </summary>
    public static KeyCode interactKey = KeyCode.P;
    public Collider2D interactCollider;

    public TextAsset inkJson;

    /// <summary>
    /// Detection/Flags for Interaction
    /// </summary>
    private bool npcCollision = false;
    private bool itemCollision = false;
    private GameObject itemObject = null;
    private GameObject npcObject = null;

    //Set update to constantly check for player input
    void Update()
    {
        if (Input.GetKeyUp(interactKey))
        {
            interactionType();
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
