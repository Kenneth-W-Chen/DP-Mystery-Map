using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerInfo;

public class questSystem : MonoBehaviour
{
    public TextAsset openingDialogue;

    // Start is called before the first frame update
    void Start()
    {
        if(Player.collectedItems == Item.None)
        {
            DialogueManager.instance.EnterDialogueMode(openingDialogue);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
