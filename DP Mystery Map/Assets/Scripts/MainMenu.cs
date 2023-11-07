using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
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
    public List<GameObject> saveFileGameObjects;
    public List<Button> saveFileButtons;
    public List<Animator> saveFileAnimator;
    
    private static readonly Color Transparent = new Color(0, 0, 0, 0);
    
    private List<PlayerSave> m_PlayerSaves;
    
    private static readonly int HasSaveFile = Animator.StringToHash("HasSaveFile");

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

        string savePath = Path.Combine(PlayerSave.defaultSavePath,"1");
        if (!File.Exists(savePath))
            Player.SaveFilePath = String.Copy(savePath);
        else if (!File.Exists((savePath = Path.Combine(PlayerSave.defaultSavePath, "2"))))
            Player.SaveFilePath = String.Copy(savePath);
        else if (!File.Exists((savePath = Path.Combine(PlayerSave.defaultSavePath, "3"))))
            Player.SaveFilePath = String.Copy(savePath);
        // todo: prompt player where they want to save the file
        else Player.SaveFilePath = Path.Combine(PlayerSave.defaultSavePath, "999");
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
    public void LoadGameMenu()
    {
        LoadGameSetup();
    }

    public void Back_LoadGame()
    {
        for (int i = 0; i < m_PlayerSaves.Count; i++)
            m_PlayerSaves[i] = null;
        foreach (Button saveFileButton in saveFileButtons)
        {
            saveFileButton.interactable = false;
        }
        /*StartCoroutine(OverlayFadeOutCoroutine(action));*/
    }

    /// <summary>
    /// Loads the game from a save file.
    /// </summary>
    /// <param name="index">Index of save file</param>
    public void StartLoadGame(int index)
    {
        
    }

    /// <summary>
    /// Finds save files and starts <see cref="SaveOpenDelay"/> coroutine for save files that exist
    /// </summary>
    private void LoadGameSetup()
    {
        for (int i = 0; i < m_PlayerSaves.Count; i++)
        {
            if ((m_PlayerSaves[i] =
                    PlayerSave.GetSaveFile(Path.Combine(PlayerSave.defaultSavePath, (i+1).ToString()))) is null)
                return;
            StartCoroutine(SaveOpenDelay(i, 0.5f * i));
        }
    }

    /// <summary>
    /// function for unit testing; attach to buttons
    /// </summary>
    public void test()
    {
       
    }
    
    /// <summary>
    /// Delays playing the animation for the loading dock open animation
    /// </summary>
    /// <param name="saveFileNumber">Number of the save file to open. Based on array indexing</param>
    /// <param name="delay">Time delay in seconds before opening.</param>
    private IEnumerator SaveOpenDelay(int saveFileNumber, float delay = 0f)
    {
        float time = 0f;
        while (time < delay)
        {
            yield return null;
            time += Time.deltaTime;
        }

        saveFileButtons[saveFileNumber].interactable = true;
        saveFileAnimator[saveFileNumber].SetBool(HasSaveFile, true);
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
