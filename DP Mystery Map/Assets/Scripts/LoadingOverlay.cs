using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingOverlay : MonoBehaviour
{
    public static LoadingOverlay Reference;
    public Image image;
    public TMP_Text text;

    public void Show()
    {
        image.enabled = text.enabled = true;
    }

    public void Hide()
    {
        image.enabled = text.enabled = false;
    }
    
    void Start()
    {
        if (Reference is not null)
        {
            Destroy(this.gameObject);
            return;
        }

        Reference = this;
        SceneManager.sceneLoaded += OnLevelLoad;
        Hide();
    }
    private void OnDestroy()
    {
        if (Reference != this)
            return;
        Reference = null;
        SceneManager.sceneLoaded -= OnLevelLoad;
    }
    
    private void OnLevelLoad(Scene scene, LoadSceneMode mode)
    {
        Hide();
    }
}
