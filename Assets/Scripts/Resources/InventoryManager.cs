using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UNetUI.Resources
{
    public class InventoryManager : MonoBehaviour
    {
        #region Singleton

        private static InventoryManager _instance;

        private void Awake()
        {
            if (_instance == null)
                _instance = this;

            if (_instance != this)
                Destroy(gameObject);
        }

        #endregion Singleton

        public List<Item> items;
        public GameObject itemPrefab;
        public RectTransform inventoryHolder;

        private void Start() => CreateAndSetInventoryItems();

        private void CreateAndSetInventoryItems()
        {
            foreach (Item item in items)
            {
                GameObject itemInstance = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
                itemInstance.transform.SetParent(inventoryHolder);
                itemInstance.transform.localScale = Vector3.one;

                itemInstance.GetComponent<Image>().sprite = item.icon;
                itemInstance.transform.GetChild(0).GetComponent<Text>().text = item.itemName;
            }
        }
    }
}