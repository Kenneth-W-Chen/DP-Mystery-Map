using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerInfo;

public class questSystem : MonoBehaviour
{
    public TextAsset openingDialogue;
    private bool newGame = true;

    // Start is called before the first frame update
    void Start()
    {
        if(newGame == true)
        {
            DialogueManager.instance.EnterDialogueMode(openingDialogue);
            Player.SetEventFlag(GameEventFlags.OpeningScene);
        }
        newGame = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
