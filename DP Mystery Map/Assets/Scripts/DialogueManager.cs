using Ink.Parsed;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Ink.Runtime;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private GameObject dialogPanel;
    
    [SerializeField] private TextMeshProUGUI dialogText;

    private Ink.Runtime.Story currentDialog;

    public bool dialogIsPlaying { get; private set; }

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

    // Start is called before the first frame update
    void Start()
    {
        dialogIsPlaying = false;
        dialogPanel.SetActive(false);
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

    private void ContinueStory()
    {
        if (currentDialog.canContinue)
        {
            dialogText.text = currentDialog.Continue();
        }
        else
            ExitDialog();
    }

    // Update is called once per frame
    void Update()
    {
        if (!dialogIsPlaying)
            return;

        if (Input.GetKeyUp(InteractivitySystem.interactKey))
            ContinueStory();
    }
}
