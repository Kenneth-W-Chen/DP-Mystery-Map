using System.Collections;
using System.Collections.Generic;
using PlayerInfo;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewGameSetup : MonoBehaviour
{
    /// <summary>
    /// Image for the fade-in transition
    /// </summary>
    public Image overlay;

    public GameObject content;

    public TMP_Dropdown majorDropdown;

    public float transitionSpeed = 0.1f;
    public float targetTime = 3f;

    private static readonly Color Transparent = new Color(0, 0, 0, 0);
    
    /// <summary>
    /// Shows the prompt for major selection
    /// </summary>
    public void NewGame()
    {
        StartCoroutine(NewGameCoroutine());
    }

    /// <summary>
    /// Initializes all variables and item locations. Then loads the player into a new scene
    /// </summary>
    public void StartGame()
    {
        Player.resetValues();
        Player.major = majorDropdown.value switch
        {
            0 => Major.ComputerEngineering,
            1 => Major.ComputerScience,
            2 => Major.ElectricalEngineering,
            3 => Major.MechanicalEngineering,
            _ => Player.major
        };
        //todo: set up new game 
    }

    /// <summary>
    /// Goes back from the major selection screen to the title screen
    /// </summary>
    public void Back()
    {
        StartCoroutine(BackCoroutine());
    }

    /// <summary>
    /// Coroutine to have a fancy fade-in transition
    /// </summary>
    private IEnumerator NewGameCoroutine()
    {
        // Play image transition
        overlay.color = Transparent;
        var targetColor = Color.black;
        var time = 0f;
        while (overlay.color != targetColor)
        {
            time += Time.deltaTime * transitionSpeed;
            /*i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / time*speed));*/
            overlay.color = Color.Lerp(overlay.color, targetColor, time/targetTime);
            yield return null;
        }
        
        // Set major prompt content to active
        content.SetActive(true);
    }

    private IEnumerator BackCoroutine()
    {
        content.SetActive(false);
        overlay.color = Color.black;
        float time = 0f;
        while (overlay.color != Transparent)
        {
            time += Time.deltaTime * transitionSpeed;
            overlay.color = Color.Lerp(overlay.color, Transparent, time / targetTime);
            yield return null;
        }
    }
}
