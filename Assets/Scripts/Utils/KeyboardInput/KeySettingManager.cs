using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace Utils.KeyboardInput
{
    public class KeySettingManager : MonoBehaviour
    {
        // 存储所有键位映射
        public List<KeyMapping> keyMappings = new();
        // 文件路径，用于保存和加载键位设置
        public string filePath;

        // 单例模式
        private static KeySettingManager _instance;

        // 获取或创建KeySettingManager的单例实例
        public static KeySettingManager Instance
        {
            get
            {
                if (_instance != null) return _instance;
                
                _instance = FindObjectOfType<KeySettingManager>();
                if (_instance == null)
                {
                    var go = new GameObject(nameof(KeySettingManager));
                    _instance = go.AddComponent<KeySettingManager>();
                }
                
                return _instance;
            }
        }

        // 当前的方向
        public Vector2 Direction { get; private set; }

        // 初始化，确保只有一个实例并加载键位设置
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);  // 保证对象不会在场景切换时被销毁
                filePath = Path.Combine(Application.persistentDataPath, "KeySetting.json");
                LoadKeySettings();  // 加载键位设置
            }
        }

        // 加载键位设置，如果不存在则创建默认配置
        private void LoadKeySettings()
        {
            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                keyMappings = JsonConvert.DeserializeObject<List<KeyMapping>>(json);
            }
            else
            {
                // 创建默认键位设置
                keyMappings = new List<KeyMapping>
                {
                    new KeyMapping("Attack", KeyCode.J),
                    new KeyMapping("Left", KeyCode.A),
                    new KeyMapping("Right", KeyCode.D),
                    new KeyMapping("Up", KeyCode.W),
                    new KeyMapping("Down", KeyCode.S)
                };
                SaveKeySettings();  // 保存默认设置
            }
        }

        // 保存键位设置到文件
        private void SaveKeySettings()
        {
            var json = JsonConvert.SerializeObject(keyMappings, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        // 根据动作名获取对应的键位
        public KeyCode GetKey(string actionName)
        {
            return keyMappings.FirstOrDefault(mapping => mapping.actionName == actionName)?.keyCode ?? KeyCode.None;
        }

        // 更新指定动作的键位并保存
        public void SetKey(string actionName, KeyCode newKeyCode)
        {
            var mapping = keyMappings.FirstOrDefault(m => m.actionName == actionName);
            if (mapping != null)
            {
                mapping.keyCode = newKeyCode;
                SaveKeySettings();  // 保存更新后的设置
            }
        }

        // 每帧更新方向，基于键盘输入
        private void Update()
        {
            float horizontal = 0;
            float vertical = 0;

            if (Input.GetKey(GetKey("Left")))
                horizontal = -1;
            else if (Input.GetKey(GetKey("Right")))
                horizontal = 1;

            if (Input.GetKey(GetKey("Up")))
                vertical = 1;
            else if (Input.GetKey(GetKey("Down")))
                vertical = -1;

            Direction = new Vector2(horizontal, vertical);
        }
    }
}
