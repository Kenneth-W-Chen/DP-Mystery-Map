using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PlayerInfo
{
    public abstract class GameplayScript : MonoBehaviour
    {
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