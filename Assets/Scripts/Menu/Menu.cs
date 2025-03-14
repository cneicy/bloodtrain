using System;
using JetBrains.Annotations;
using Manager;
using UnityEditor;
using UnityEngine;

namespace Menu
{
    public class Menu : MonoBehaviour
    {
        [SerializeField] private GameObject mainMenu;

        [UsedImplicitly]
        public void GameStart()
        {
            //todo:其它manager接入GameStart事件
            EventManager.Instance.TriggerEvent<object>("GameStart", null);
            mainMenu.SetActive(false);
        }

        //todo:等待保存功能完成后调用
        private void SaveGame()
        {
            throw new NotImplementedException();
        }
        
        public void Resume()
        {
            Time.timeScale = 1;
            gameObject.SetActive(false);
        }

        public void Pause()
        {
            Time.timeScale = 0;
            throw new NotImplementedException();
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