using System;
using UnityEngine.SceneManagement;

namespace PlayerInfo
{
    public class GameplayCanvas : GameplayScript
    {
        private static GameplayCanvas reference;
        protected override void Start()
        {
            if (reference is not null)
            {
                Destroy(this.gameObject);
                return;
            }
            base.Start();
        }

        private void OnDestroy()
        {
            if (reference == this)
                reference = null;
            SceneManager.sceneLoaded -= OnLevelLoad;
        }
    }
}