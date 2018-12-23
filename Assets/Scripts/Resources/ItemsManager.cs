using System.Collections.Generic;
using UnityEngine;
using UNetUI.Resources;

namespace UNetUI.Resources
{
    public class ItemsManager : MonoBehaviour
    {
        #region Singleton

        public static ItemsManager instance;

        private void Awake()
        {
            if (instance == null)
                instance = this;

            if (instance != this)
                Destroy(gameObject);
        }

        #endregion Singleton
        
        public List<Item> items;

        public List<Item> GetItems()
        {
            return items;
        }

        public Sprite GetTextureByName(string itemName)
        {
            foreach (var item in items)
                if (item.itemName == itemName)
                    return item.icon;

            return null;
        }
    }
}