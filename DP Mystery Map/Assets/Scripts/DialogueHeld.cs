using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using UnityEngine.TextCore.Text;

public class DialogueHeld : MonoBehaviour
{
    public UnityEngine.TextAsset[] npcLinesText;
    public bool randomAsset = false;
    public bool randomLine = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    public UnityEngine.TextAsset returnRandomAsset(UnityEngine.TextAsset[] textAssets)
    {
        if (textAssets != null && textAssets.Length > 0)
        {
            return textAssets[Random.Range(0, textAssets.Length)];
        }
        else
        {
            Debug.LogError("TextAssets array is null or empty.");
            return null;
        }
    }

    public UnityEngine.TextAsset returnRandomLine(UnityEngine.TextAsset thisLine)
    {
        //Store each dialogue line into an array, return random line
        string[] lines = thisLine.text.Split('\n');
        string finalString = lines[Random.Range(0, lines.Length)].Trim();
        UnityEngine.TextAsset returnThis = TempTextAsset(finalString);
        return returnThis;
    }

    UnityEngine.TextAsset TempTextAsset(string content)
    {
        // Create a temporary TextAsset with the provided content
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(content);
        UnityEngine.TextAsset temporaryTextAsset = new UnityEngine.TextAsset(System.Text.Encoding.UTF8.GetString(bytes));
        return temporaryTextAsset;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
