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
    
    //public TextAsset inkJson;
    [NonSerialized] public bool canInteract = true;

    /// <summary>
    /// Detection/Flags for Interaction
    /// </summary>
    private bool npcCollision = false;

    private bool itemCollision = false;
    private GameObject itemObject = null;
    private GameObject npcObject = null;
    private TextAsset inkJson = null;


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
        if (Input.GetKeyUp(Player.InteractKey) && canInteract &&
            !PlayerController.playerControllerReference.WalkingGrid)
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
        else if (npcCollision && (PlayerController.playerControllerReference.WalkBlocked != PlayerController.WalkBlockedFlags.Dialogue))
            npcInteraction();
        else
            Debug.Log("There is nothing here!");
    }


    //Logic for NPC interaction
    private void npcInteraction()
    {
        //Interact with NPC, allow random lines for misc NPCs
        Debug.Log("Interacting with NPC.");
        if (npcObject.GetComponent<DialogueHeld>().randomAsset == false)
            inkJson = npcObject.GetComponent<DialogueHeld>().npcLinesText[UnityEngine.Random.Range(0, npcObject.GetComponent<DialogueHeld>().npcLinesText.Length)];
        else
            inkJson = npcObject.GetComponent<DialogueHeld>().returnRandomAsset(npcObject.GetComponent<DialogueHeld>().npcLinesText);

        if (npcObject.GetComponent<DialogueHeld>().randomLine == false)
            inkJson = npcObject.GetComponent<DialogueHeld>().npcLinesText[UnityEngine.Random.Range(0, npcObject.GetComponent<DialogueHeld>().npcLinesText.Length)];
        else
            inkJson = npcObject.GetComponent<DialogueHeld>().returnRandomLine(npcObject.GetComponent<DialogueHeld>().returnRandomAsset(npcObject.GetComponent<DialogueHeld>().npcLinesText));

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
        if(itemObject.gameObject.name == "keys")
        {
            Debug.Log("Congratulations Keys Found!");
            Player.collectedItems = Item.Keys;
        }
        else if (itemObject.gameObject.name == "Pen")
        {
            Debug.Log("Congratulations Pen Found!");
            Player.collectedItems = Item.Pen;
        }
        else if (itemObject.gameObject.name == "phone")
        {
            Debug.Log("Congratulations phone Found!");
            Player.collectedItems = Item.Phone;
            Destroy(GameObject.Find("A wing"));
        }
        else if (itemObject.gameObject.name == "book")
        {
            Debug.Log("Congratulations book Found!");
            Player.collectedItems = Item.Pen;
        }

        inkJson = itemObject.gameObject.GetComponent<DialogueHeld>().npcLinesText[0];
        DialogueManager.instance.EnterDialogueMode(inkJson);
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

        //If A wing's dark overlay is present, notify player
        if(collision.gameObject.name == "A wing")
        {
            inkJson = collision.gameObject.GetComponent<DialogueHeld>().npcLinesText[0];
            DialogueManager.instance.EnterDialogueMode(inkJson);
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