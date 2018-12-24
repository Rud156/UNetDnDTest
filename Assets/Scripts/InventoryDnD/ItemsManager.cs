using System.Collections.Generic;
using UnityEngine;

namespace UNetUI.InventoryDnD
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

        [SerializeField] private List<Item> _items;

        public List<Item> GetItems() => _items;

        public Item GetItemByName(string itemName)
        {
            foreach (Item item in _items)
            {
                if (item.itemName == itemName)
                    return item;
            }

            return null;
        }

        public Sprite GetTextureByName(string itemName)
        {
            foreach (var item in _items)
                if (item.itemName == itemName)
                    return item.icon;

            return null;
        }
    }
}