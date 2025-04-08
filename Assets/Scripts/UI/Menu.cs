using System;
using JetBrains.Annotations;
using Manager;
using UnityEditor;
using UnityEngine;

namespace UI
{
    public class Menu : MonoBehaviour
    {
        [SerializeField] private GameObject mainMenu;//主菜单
        [SerializeField] private GameObject pauseMenu;//暂停菜单
        [SerializeField] private GameObject failMenu;//失败菜单

        private void OnEnable()
        {
            EventManager.Instance.RegisterEventHandlersFromAttributes(this);//处理[EventSubscribe()]特性标注的事件订阅
        }

        private void OnDisable()
        {
            if (!EventManager.Instance) return;
            EventManager.Instance.UnregisterAllEventsForObject(this);//事件取消订阅
        }
        
        /// <summary>
        /// 在游戏结束事件触发时显示失败菜单并暂停游戏
        /// </summary>
        [EventSubscribe("GameFail")]
        public object OnGameFail(float speed)
        {
            failMenu.SetActive(true);
            Time.timeScale = 0;
            return null;
        }

        /// <summary>
        /// 触发游戏开始事件并隐藏主菜单
        /// 由开始游戏按钮调用
        /// </summary>
        public void GameStart()
        {
            EventManager.Instance.TriggerEvent("GameStart", this);
            mainMenu.SetActive(false);
        }

        private void Awake()
        {
            pauseMenu.SetActive(false);
        }

        /// <summary>
        /// 游戏开始后按下ESC时打开暂停菜单
        /// </summary>
        private void FixedUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && !mainMenu.activeSelf && !pauseMenu.activeSelf) Pause();
        }

        //todo:等待保存功能完成后调用
        private void SaveGame()
        {
            throw new NotImplementedException();
        }

        public void Resume()//游戏继续
        {
            Time.timeScale = 1;
            pauseMenu.SetActive(false);
        }

        public void Pause()//游戏暂停
        {
            Time.timeScale = 0;
            pauseMenu.SetActive(true);
        }
        
        public void GameQuit()//游戏退出
        {
            if (Application.isEditor)
                EditorApplication.isPlaying = false;
            Application.Quit();
        }
    }
}