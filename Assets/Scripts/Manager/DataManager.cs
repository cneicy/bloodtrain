using System.Linq;
using Utils;

namespace Manager
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;
    using Newtonsoft.Json;
    using UnityEngine;

    namespace PlayerSystem
    {
        public class DataManager : Singleton<DataManager>
        {
            #region 事件系统

            public static class DataEvents
            {
                public const string PlayerDataLoaded = "PlayerDataLoaded";
                public const string PlayerDataSaved = "PlayerDataSaved";
                public const string PlayerDataUpdated = "PlayerDataUpdated";
                public const string PlayerDataReset = "PlayerDataReset";
                public const string PlayerDataError = "PlayerDataError";
                public const string SlotCreated = "PlayerSlotCreated";
                public const string SlotDeleted = "PlayerSlotDeleted";
                public const string SlotChanged = "PlayerSlotChanged";
            }

            public class DataUpdateEventArgs
            {
                public string SlotName { get; set; }
                public string Key { get; set; }
                public object OldValue { get; set; }
                public object NewValue { get; set; }
            }

            #endregion

            #region 配置参数

            private const int AutoSaveInterval = 300;
            private const string DefaultSlot = "Default";
            private const string SaveFolder = "PlayerSaves";
            private static readonly string[] CryptoKeys = { "AESKey1234567890", "AESIV0987654321" }; // 应替换为实际密钥

            #endregion

            #region 核心状态

            private Dictionary<string, object> _currentData = new();
            private float _saveTimer;
            private string _currentSlot = DefaultSlot;
            private bool _encryptionEnabled = true;

            #endregion

            #region 公开属性

            public string CurrentSlot
            {
                get => _currentSlot;
                set => SwitchSlot(value);
            }

            public bool EncryptionEnabled
            {
                get => _encryptionEnabled;
                set => _encryptionEnabled = value;
            }

            #endregion

            #region 生命周期

            protected override void Awake()
            {
                base.Awake();
                EnsureSaveDirectory();
                //  
                SwitchSlot(DefaultSlot);
                SetupAutoSave();
            }

            private void Update()
            {
                if (_saveTimer > 0 && (_saveTimer -= Time.deltaTime) <= 0)
                {
                    ForceSave();
                    SetupAutoSave();
                }
            }

            private void OnApplicationQuit() => ForceSave();

            #endregion

            #region 公开接口

            public T GetData<T>(string key, T defaultValue = default)
            {
                if (TryGetValue(key, out T value)) return value;
                SetData(key, defaultValue);
                return defaultValue;
            }

            public bool SetData<T>(string key, T newValue, bool immediateSave = false)
            {
                if (string.IsNullOrEmpty(key)) return false;

                var oldValue = _currentData.ContainsKey(key) ? _currentData[key] : null;
                if (Equals(oldValue, newValue)) return false;

                _currentData[key] = newValue;
                TriggerUpdateEvent(key, oldValue, newValue);

                if (immediateSave) ForceSave();
                else ResetSaveTimer();

                return true;
            }

            public void CreateNewSlot(string slotName)
            {
                if (SlotExists(slotName)) return;

                var path = GetSlotPath(slotName);
                File.WriteAllText(path, EncryptData("{}"));
                EventManager.Instance.TriggerEvent(DataEvents.SlotCreated, slotName);
            }

            public void DeleteSlot(string slotName)
            {
                var path = GetSlotPath(slotName);
                if (File.Exists(path)) File.Delete(path);
                EventManager.Instance.TriggerEvent(DataEvents.SlotDeleted, slotName);
            }

            public List<string> GetAllSlots()
            {
                var files = Directory.GetFiles(GetSaveDirectory(), "*.sav");
                return files.Select(Path.GetFileNameWithoutExtension).ToList();
            }

            public void SwitchSlot(string slotName)
            {
                if (_currentSlot == slotName) return;

                ForceSave();
                _currentSlot = slotName;
                LoadCurrentSlot();
                EventManager.Instance.TriggerEvent(DataEvents.SlotChanged, slotName);
            }

            public void ForceSave()
            {
                try
                {
                    var dataToSave = new Dictionary<string, object>(_currentData)
                    {
                        ["LastSaveTime"] = DateTime.Now
                    };

                    var json = JsonConvert.SerializeObject(dataToSave, Formatting.Indented);
                    File.WriteAllText(GetCurrentSlotPath(), _encryptionEnabled ? EncryptData(json) : json);

                    EventManager.Instance.TriggerEvent(DataEvents.PlayerDataSaved, new
                    {
                        SlotName = _currentSlot,
                        Data = dataToSave
                    });
                }
                catch (Exception e)
                {
                    HandleError($"数据保存失败: {e.Message}");
                }
            }

            #endregion

            #region 加密功能

            private string EncryptData(string plainText)
            {
                using var aes = Aes.Create();
                aes.Key = Encoding.UTF8.GetBytes(CryptoKeys[0]);
                aes.IV = Encoding.UTF8.GetBytes(CryptoKeys[1]);

                var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using var ms = new MemoryStream();
                using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
                using (var sw = new StreamWriter(cs))
                {
                    sw.Write(plainText);
                }

                return Convert.ToBase64String(ms.ToArray());
            }

            private string DecryptData(string cipherText)
            {
                try
                {
                    using var aes = Aes.Create();
                    aes.Key = Encoding.UTF8.GetBytes(CryptoKeys[0]);
                    aes.IV = Encoding.UTF8.GetBytes(CryptoKeys[1]);

                    var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                    using var ms = new MemoryStream(Convert.FromBase64String(cipherText));
                    using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
                    using var sr = new StreamReader(cs);
                    return sr.ReadToEnd();
                }
                catch
                {
                    return "{}";
                }
            }

            #endregion

            #region 私有方法

            private void LoadCurrentSlot()
            {
                try
                {
                    var path = GetCurrentSlotPath();
                    if (!File.Exists(path))
                    {
                        _currentData = new Dictionary<string, object>();
                        return;
                    }

                    var content = File.ReadAllText(path);
                    var json = _encryptionEnabled ? DecryptData(content) : content;
                    _currentData = JsonConvert.DeserializeObject<Dictionary<string, object>>(json)
                                   ?? new Dictionary<string, object>();

                    EventManager.Instance.TriggerEvent(DataEvents.PlayerDataLoaded, new
                    {
                        SlotName = _currentSlot,
                        Data = _currentData
                    });
                }
                catch (Exception e)
                {
                    HandleError($"数据加载失败: {e.Message}");
                }
            }

            private bool TryGetValue<T>(string key, out T value)
            {
                if (_currentData.TryGetValue(key, out var rawValue))
                {
                    try
                    {
                        value = (T)Convert.ChangeType(rawValue, typeof(T));
                        return true;
                    }
                    catch
                    {
                        value = default;
                        return false;
                    }
                }

                value = default;
                return false;
            }

            private void TriggerUpdateEvent(string key, object oldValue, object newValue)
            {
                EventManager.Instance.TriggerEvent(DataEvents.PlayerDataUpdated, new DataUpdateEventArgs
                {
                    SlotName = _currentSlot,
                    Key = key,
                    OldValue = oldValue,
                    NewValue = newValue
                });
            }

            private string GetSaveDirectory() => Path.Combine(
                Application.persistentDataPath,
                SaveFolder
            );

            private string GetCurrentSlotPath() => GetSlotPath(_currentSlot);

            private string GetSlotPath(string slotName) => Path.Combine(
                GetSaveDirectory(),
                $"{slotName}.sav"
            );

            private bool SlotExists(string slotName) =>
                File.Exists(GetSlotPath(slotName));

            private void EnsureSaveDirectory()
            {
                var path = GetSaveDirectory();
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            }

            private void HandleError(string message)
            {
                Debug.LogError(message);
                EventManager.Instance.TriggerEvent(DataEvents.PlayerDataError, message);
            }

            private void SetupAutoSave() => _saveTimer = AutoSaveInterval;
            private void ResetSaveTimer() => _saveTimer = AutoSaveInterval;

            #endregion
        }
    }
}