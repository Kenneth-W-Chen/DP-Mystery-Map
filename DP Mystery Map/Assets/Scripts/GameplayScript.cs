using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PlayerInfo
{
    public abstract class GameplayScript : MonoBehaviour
    {
        /// <summary>
        /// Sets the object to DontDestroyOnLoad and sets the object to be destroyed when loading the main menu scene 
        /// </summary>
        protected virtual void Start()
        {
            SceneManager.sceneLoaded += OnLevelLoad;
            DontDestroyOnLoad(this.gameObject);
        }

        protected virtual void OnLevelLoad(Scene scene, LoadSceneMode mode)
        {
            if(scene.name == "MainMenu")
                Destroy(this.gameObject);
        }
    }
}