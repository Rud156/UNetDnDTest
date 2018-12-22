using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UNetUI.SharedData;

namespace UNetUI.Resources
{
    public class InventoryManager : MonoBehaviour
    {
        #region Singleton

        public static InventoryManager instance;

        private void Awake()
        {
            if (instance == null)
                instance = this;

            if (instance != this)
                Destroy(gameObject);
        }

        #endregion Singleton

        [Header(("Inventory"))] public ScrollRect inventoryScroller;

        [Header("Item Details")] public Image itemImage;
        public Text itemName;
        public Text itemDescription;
        public Text damageText;
        public Text defenceText;
        public Text strengthText;
        public Text intelText;
        public Text agilityText;

        [Header("Selected Holder")] public GameObject selectedHolder;
        public GameObject deselectedHolder;

        [Header("Borders")] public Sprite defaultBorder;
        public Sprite selectedBorder;

        [Header(("Inventory Items"))] public List<Item> items;
        public GameObject itemPrefab;
        public RectTransform inventoryHolder;

        private List<InventoryItem> _items;

        private InventoryItem _itemSelected;

        private InventoryItem ItemSelected
        {
            get { return _itemSelected; }
            set
            {
                _itemSelected = value;
                UpdateUiWithItemSelected();
            }
        }

        private void Start()
        {
            _items = new List<InventoryItem>();

            CreateAndSetInventoryItems();
            ClearItemSelected();
        }

        #region Scroller

        public void DisableInventoryScrolling() => inventoryScroller.enabled = false;

        public void EnableInventoryScrolling() => inventoryScroller.enabled = true;

        #endregion Scroller

        #region AddRemoveItemInventory

        public void RemoveInventoryItem(InventoryItem item) => _items.Remove(item);

        public void AddInventoryItem(Item item)
        {
            GameObject itemInstance = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
            itemInstance.transform.SetParent(inventoryHolder);
            itemInstance.transform.localScale = Vector3.one;

            ItemDnD itemDnD = itemInstance.GetComponent<ItemDnD>();

            Image inventoryItemImage = itemInstance.transform.GetChild(0).GetComponent<Image>();
            Image inventoryItemBorder = itemInstance.GetComponent<Image>();
            Text inventoryItemName = itemInstance.transform.GetChild(1).GetComponent<Text>();

            InventoryItem newItem = new InventoryItem
            {
                item = item,
                itemBorder = inventoryItemBorder
            };
            
            itemDnD.SetItem(newItem);

            inventoryItemImage.sprite = item.icon;
            inventoryItemName.text = item.itemName;
            inventoryItemBorder.sprite = defaultBorder;

            _items.Add(newItem);
            itemInstance.GetComponent<Button>().onClick.AddListener(() => InventoryItemClicked(newItem));
        }

        #endregion AddRemoveItemInventory


        private void ClearItemSelected()
        {
            ItemSelected = null;
            selectedHolder.SetActive(false);
            deselectedHolder.SetActive(true);
        }

        private void CreateAndSetInventoryItems()
        {
            foreach (Item item in items)
                AddInventoryItem(item);
        }

        private void UpdateUiWithItemSelected()
        {
            deselectedHolder.SetActive(false);
            selectedHolder.SetActive(true);

            foreach (InventoryItem inventoryItem in _items)
            {
                if (inventoryItem == ItemSelected)
                {
                    inventoryItem.itemBorder.sprite = selectedBorder;

                    itemImage.sprite = inventoryItem.item.icon;
                    itemName.text = inventoryItem.item.itemName;
                    itemDescription.text = inventoryItem.item.description;

                    damageText.text = $"Damage: {inventoryItem.item.damage}";
                    defenceText.text = $"Defence: {inventoryItem.item.defence}";
                    strengthText.text = $"Strength: {inventoryItem.item.strength}";
                    intelText.text = $"Intel: {inventoryItem.item.intel}";
                    agilityText.text = $"Agility: {inventoryItem.item.agility}";
                }
                else
                    inventoryItem.itemBorder.sprite = defaultBorder;
            }
        }

        private void InventoryItemClicked(InventoryItem item) => ItemSelected = item;
    }
}