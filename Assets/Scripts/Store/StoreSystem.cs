using System.Collections.Generic;
using Manager;
using UnityEngine;

namespace Store
{
    public class StoreItemData
    {
        public string ItemName{ get; private set; }
        public string EffectText{get;private set;}
        public string PropertyText{get;private set;}
        public int Level{get;private set;}
        public float Price{get;private set;}
        public Sprite Icon{get;private set;}
        public StoreItemData(string itemName, string effectText, string propertyText, int level, float price, Sprite icon)
        {
            ItemName = itemName;
            EffectText = effectText;
            PropertyText = propertyText;
            Level = level;
            Price = price;
            Icon = icon;
        }
    }
    public class StoreSystem : MonoBehaviour
    {
        private List<StoreItemData> _storeItemsLevel1 = new List<StoreItemData>();
        private List<StoreItemData> _storeItemsLevel2 = new List<StoreItemData>();
        private List<StoreItemData> _storeItemsLevel3 = new List<StoreItemData>();
        //商店物品预制体
        [SerializeField] private GameObject storeItemPrefab;
        //商店面板
        [SerializeField] private GameObject storePanel;
        //游戏阶段
        private string _gameStage;
        private void Start()
        {
            //初始化数据
            InstantiateStoreItem();
            //打开面板时生成item
            CreateStoreItem();
        }

        private void CreateStoreItem()
        {
            for (int i = 0; i < 3; i++)
            {
                GameObject storeItem = Instantiate(storeItemPrefab, storePanel.transform);
                //随机的方法
                RandomStoreItem(_gameStage);
            }
        }

        private void RandomStoreItem(string gameStage)
        { 
            float randomLevel1,randomLevel2,randomLevel3,randomLevel4,randomLevel5;
            switch (gameStage)
            {
                case "a":
                    randomLevel1 = 1;
                    break;
                case "b":
                    randomLevel1 = 0.7f;
                    randomLevel2 = 1;
                    break;
                case "c":
                    randomLevel1 = 0.5f;
                    randomLevel2 = 0.85f;
                    randomLevel3 = 1;
                    break;
            }
            Random.Range(0, 1f);
        }

        //初始化商店数据
        private void InstantiateStoreItem()
        {
            //读取所有itemData
            _gameStage = GamePhraseManager.Instance.currentPhrase;
        }
    }
}
