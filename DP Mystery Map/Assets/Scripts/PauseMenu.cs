using System.Collections;
using System.Collections.Generic;
using PlayerInfo;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    // The pause menu content
    public GameObject m_Content;

    public void UnpauseGame()
    {
        HidePauseMenu();
        PlayerController.playerControllerReference.canWalk = InteractivitySystem.reference.canInteract = true;
        
    }
    
    public void PauseGame()
    {
        ShowPauseMenu();
        PlayerController.playerControllerReference.canWalk = InteractivitySystem.reference.canInteract = false;
    }
    
    void Update()
    {
        if (PlayerController.playerControllerReference.WalkingGrid||!Input.GetKeyDown(Player.PauseKey))
            return;
        // if pause menu visible
        if(m_Content.activeSelf)
            UnpauseGame();
        else // pause menu is not visible, so show it
        {
            PauseGame();
        }
    }

    private void ShowPauseMenu()
    {
        m_Content.SetActive(true);
    }
    
    private void HidePauseMenu()
    {
        m_Content.SetActive(false);
    }
}
