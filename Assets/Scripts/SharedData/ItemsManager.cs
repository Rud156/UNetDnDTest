using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UNetUI.Resources;

namespace UNetUI.SharedData
{
    public class ItemsManager : MonoBehaviour
    {
        #region Singleton

        public static ItemsManager instance;

        private void Awake()
        {
            if (instance == null)
                instance = this;
            
            if(instance != this)
                Destroy(gameObject);
        }

        #endregion Singleton

        public List<Item> items;

        public List<Item> GetItems() => items;

        public Sprite GetTextureByName(string itemName)
        {
            foreach (var item in items)
            {
                if (item.itemName == itemName)
                    return item.icon;
            }

            return null;
        }
    }
}