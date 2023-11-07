using System;
using System.Collections;
using System.Collections.Generic;
using PlayerInfo;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    /// <summary>
    /// Image for the fade-in transition
    /// </summary>
    public Image overlay;

    [FormerlySerializedAs("content")] public GameObject newGameContent;

    public TMP_Dropdown majorDropdown;

    public float transitionSpeed = 0.1f;
    public float targetTime = 3f;

    private static readonly Color Transparent = new Color(0, 0, 0, 0);
    
    /// <summary>
    /// Shows the prompt for major selection.
    /// Called by New Game Button
    /// </summary>
    public void NewGame()
    {
        StartCoroutine(OverlayFadeInCoroutine(newGameContent.SetActive, true, false));
    }

    /// <summary>
    /// Initializes all variables and item locations. Then loads the player into a new scene.
    /// Called by New Game Content > Start Button
    /// </summary>
    public void StartNewGame()
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
    /// Goes back from the major selection screen to the title screen.
    /// Called by New Game Content > Back Button
    /// </summary>
    public void Back_NewGame()
    {
        StartCoroutine(OverlayFadeOutCoroutine(newGameContent.SetActive, false, true));
    }

    /// <summary>
    /// Shows list of game saves.
    /// Called by Load Game Button
    /// </summary>
    public void LoadGame()
    {
        
    }

    /// <summary>
    /// Coroutine to have a fancy fade-in transition
    /// </summary>
    /// <param name="action">A function to be executed before or after the fade in is complete</param>
    /// <param name="param">Parameter to be passed to action</param>
    /// <param name="before">Whether to execute action before or after the fade in begins and finishes</param>
    private IEnumerator OverlayFadeInCoroutine(Action<bool> action, bool param, bool before = false)
    {
        if (before) action(param);
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

        if (!before) action(param);
    }

    /// <summary>
    /// Coroutine to have a fancy fade-out transition
    /// </summary>
    /// <param name="action">A function to be executed before or after the fade in is complete</param>
    /// <param name="param">Parameter to be passed to action</param>
    /// <param name="before">Whether to execute action before or after the fade in begins and finishes</param>
    private IEnumerator OverlayFadeOutCoroutine(Action<bool> action, bool param, bool before = false)
    {
        if (before) action(param);
        overlay.color = Color.black;
        float time = 0f;
        while (overlay.color != Transparent)
        {
            time += Time.deltaTime * transitionSpeed;
            overlay.color = Color.Lerp(overlay.color, Transparent, time / targetTime);
            yield return null;
        }

        if (!before) action(param);
    }
}
