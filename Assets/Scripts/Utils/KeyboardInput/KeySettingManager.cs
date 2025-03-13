using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace Utils.KeyboardInput
{
    public class KeySettingManager : Singleton<KeySettingManager>
    {
        // 存储所有键位映射
        public List<KeyMapping> keyMappings = new();

        // 文件路径，用于保存和加载键位设置
        public string filePath;

        // 当前的方向
        public Vector2 Direction { get; private set; }

        // 初始化，确保只有一个实例并加载键位设置
        protected override void Awake()
        {
            base.Awake();
            filePath = Path.Combine(Application.persistentDataPath, "KeySetting.json");
            LoadKeySettings(); // 加载键位设置
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
                    new("Attack", KeyCode.J),
                    new("Left", KeyCode.A),
                    new("Right", KeyCode.D),
                    new("Up", KeyCode.W),
                    new("Down", KeyCode.S)
                };
                SaveKeySettings(); // 保存默认设置
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
            if (mapping == null) return;
            mapping.keyCode = newKeyCode;
            SaveKeySettings(); // 保存更新后的设置
        }
    }
}