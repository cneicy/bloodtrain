using System;
using System.Collections.Generic;
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
        private List<StoreItemData> _storeItems = new List<StoreItemData>();

        private void Start()
        {
            
        }
    }
}
