using Ink.Parsed;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Ink.Runtime;
using PlayerInfo;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private GameObject dialogPanel;
    
    [SerializeField] private TextMeshProUGUI dialogText;

    private Ink.Runtime.Story currentDialog;

    public static bool dialogIsPlaying { get; set; }

    private string fullText;
    private float charRevealDelay = 0.05f;

    public static DialogueManager instance { get; private set; }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance exists
        }
    }
    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

        if (Player.collectedItems == Item.None)
        {
            dialogIsPlaying = true;
            dialogPanel.SetActive(true);
        }
        else
        {
            dialogIsPlaying = false;
            dialogPanel.SetActive(false);
        }
    }

    public void EnterDialogueMode(TextAsset inkJSON)
    {
        currentDialog = new Ink.Runtime.Story(inkJSON.text);
        dialogPanel.SetActive(true);
        dialogIsPlaying = true;

        ContinueStory();
    }

    private void ExitDialog() 
    {
        dialogText.text = "";
        dialogPanel.SetActive(false);
        dialogIsPlaying = false;
    }

    IEnumerator ShowText()
    {
        bool advanceText = false;

        if (dialogText == null)
        {
            Debug.LogError("dialogText is not assigned. Make sure to assign it in the Inspector.");
            yield break;
        }

        fullText = currentDialog.Continue(); // Retrieve the next portion of text

        for (int i = 0; i < fullText.Length; i++)
        {
            dialogText.text = fullText.Substring(0, i + 1);

            //If space is pressed then text will auto reveal
            if (Input.GetKeyDown(KeyCode.Space))
            {
                advanceText = true;
                Debug.Log("Space");
            }

            if (advanceText == false)
                yield return new WaitForSeconds(charRevealDelay);
            else
            {
                dialogText.text = fullText;
                yield return null;
            }
            
        }

        if (!advanceText)
        {
            // The text has been fully revealed
            yield return null;
        }
    }

    private void ContinueStory()
    {
        if (currentDialog.canContinue)
        {
            dialogText.text = currentDialog.Continue();
            StartCoroutine(ShowText());
        }
        else
            ExitDialog();
    }

    // Update is called once per frame
    void Update()
    {
        if (!dialogIsPlaying)
            return;


        if (Input.GetKeyUp(Player.interactKey) && (dialogText.text == fullText))
            ContinueStory();
    }
}