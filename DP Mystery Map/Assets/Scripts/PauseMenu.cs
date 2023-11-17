using System;
using System.Collections;
using System.Collections.Generic;
using PlayerInfo;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu reference;
    // The pause menu content
    public GameObject m_Content;

    /// <summary>
    /// Resumes the game
    /// </summary>
    public void ResumeGame()
    {
        HidePauseMenu();
        PlayerController.playerControllerReference.WalkBlocked &= ~PlayerController.WalkBlockedFlags.Paused; 
        InteractivitySystem.reference.canInteract = true;
        
    }
    
    /// <summary>
    /// Pauses the game
    /// </summary>
    public void PauseGame()
    {
        ShowPauseMenu();
        PlayerController.playerControllerReference.WalkBlocked |= PlayerController.WalkBlockedFlags.Paused;
        InteractivitySystem.reference.canInteract = false;
    }

    /// <summary>
    /// Saves the game
    /// </summary>
    public void SaveGame()
    {
        PlayerSave save = new PlayerSave();
        save.Save();
    }

    public void ExitToMainMenuConfirmation()
    {
        //todo: show a confirmation screen
        ExitToMainMenu();
    }

    private void Start()
    {
        if(reference is not null)
        {
            Destroy(this.gameObject);
            return;
        }
        reference = this;
    }

    void Update()
    {
        if (PlayerController.playerControllerReference.WalkingGrid||!Input.GetKeyDown(Player.PauseKey))
            return;
        // if pause menu visible
        if(m_Content.activeSelf)
            ResumeGame();
        else // pause menu is not visible, so show it
        {
            PauseGame();
        }
    }

    private void OnDestroy()
    {
        if (reference == this)
            reference = null;
    }

    private void ShowPauseMenu()
    {
        m_Content.SetActive(true);
    }
    
    private void HidePauseMenu()
    {
        m_Content.SetActive(false);
    }

    private void ExitToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
