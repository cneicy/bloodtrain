using System;
using JetBrains.Annotations;
using Manager;
using UnityEditor;
using UnityEngine;

namespace UI
{
    public class Menu : MonoBehaviour
    {
        [SerializeField] private GameObject mainMenu;
        [SerializeField] private GameObject pauseMenu;

        public void GameStart()
        {
            EventManager.Instance.TriggerEvent("GameStart", this);
            mainMenu.SetActive(false);
        }

        private void Awake()
        {
            pauseMenu.SetActive(false);
        }

        private void FixedUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && !mainMenu.activeSelf && !pauseMenu.activeSelf)
            {
                Pause();
            }
        }

        //todo:等待保存功能完成后调用
        private void SaveGame()
        {
            throw new NotImplementedException();
        }

        public void Resume()
        {
            Time.timeScale = 1;
            pauseMenu.SetActive(false);
        }

        public void Pause()
        {
            Time.timeScale = 0;
            pauseMenu.SetActive(true);
        }

        [UsedImplicitly]
        public void GameQuit()
        {
            if (Application.isEditor)
                EditorApplication.isPlaying = false;
            Application.Quit();
        }
    }
}