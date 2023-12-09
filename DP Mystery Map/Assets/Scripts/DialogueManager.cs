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
    private bool advanceText = false;

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
        PlayerController.playerControllerReference.WalkBlocked |= PlayerController.WalkBlockedFlags.Dialogue;
        ContinueStory();
    }

    private void ExitDialog()
    {
        dialogText.text = "";
        PlayerController.playerControllerReference.WalkBlocked &= ~PlayerController.WalkBlockedFlags.Dialogue;
        dialogPanel.SetActive(false);
        dialogIsPlaying = false;
    }

    IEnumerator ShowText()
    {
        if (dialogText == null)
        {
            Debug.LogError("dialogText is not assigned. Make sure to assign it in the Inspector.");
            yield break;
        }

        fullText = currentDialog.Continue(); // Retrieve the next portion of text
        for (int i = 0; i < fullText.Length; i++)
        {
            dialogText.text = fullText.Substring(0, i + 1);

            if (!advanceText)
                yield return new WaitForSeconds(charRevealDelay);
            else
            {
                dialogText.text = fullText;
                advanceText = false;
                yield return null;
                break;
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
        if ((Input.GetKeyDown(Player.InteractKey) || Input.GetKeyDown(KeyCode.Space) ||
             Input.GetKeyDown(KeyCode.Mouse0)))
        {
            if(dialogText.text==fullText)
                ContinueStory();
            else
            {
                advanceText = true;
            }
        }
    }
}