using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Menu.MainMenu
{
    public class MainMenuButton : MonoBehaviour
    {
        [UsedImplicitly]
        public void GameStart()
        {
            SceneManager.LoadScene("Game");
        }

        [UsedImplicitly]
        public void GameQuit()
        {
            if(Application.isEditor)
                EditorApplication.isPlaying = false;
            Application.Quit();
        }
    }
}

