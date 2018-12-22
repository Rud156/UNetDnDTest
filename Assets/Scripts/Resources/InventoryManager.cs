﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UNetUI.Extras;
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

        [Header("Item Additions")] public Text damageAdditionText;
        public Text strengthAdditionText;
        public Text intelAdditionText;
        public Text agilityAdditionText;
        public Text defenceAdditionText;
        public Text hpAdditionText;
        public Text mannaAdditionText;
        public Text dodgeChanceAdditionText;
        public Text criticalRateAdditionText;

        [Header("Selected Holder")] public GameObject selectedHolder;
        public GameObject deselectedHolder;

        [Header("Item Equip Slots")] public PartBuff headSlot;
        public PartBuff bodySlot;
        public PartBuff feetSlot;
        public PartBuff weapon1Slot;
        public PartBuff weapon2Slot;

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

        #region EquipUnEquipButtonFunction

        // Called from external Button
        public void EquipUnEquipButton()
        {
            if (ItemSelected == null)
                return;

            if (ItemSelected.itemEquipped)
                UnEquipItem();
            else
                CheckAndEquipItem();
        }

        private void CheckAndEquipItem()
        {
            Item.ItemSlot itemSlot = ItemSelected.item.slot;
            InventoryItem inventoryItem;

            switch (itemSlot)
            {
                case Item.ItemSlot.Head:
                    inventoryItem = headSlot.GetItem();
                    if (inventoryItem != null)
                        headSlot.SetItem(null);

                    headSlot.SetItem(ItemSelected);
                    break;

                case Item.ItemSlot.Body:
                    inventoryItem = bodySlot.GetItem();
                    if (inventoryItem != null)
                        bodySlot.SetItem(null);

                    bodySlot.SetItem(ItemSelected);
                    break;

                case Item.ItemSlot.Feet:
                    inventoryItem = feetSlot.GetItem();
                    if (inventoryItem != null)
                        feetSlot.SetItem(null);

                    feetSlot.SetItem(ItemSelected);
                    break;

                case Item.ItemSlot.Weapon:
                    inventoryItem = weapon1Slot.GetItem();
                    if (inventoryItem == null)
                        weapon1Slot.SetItem(ItemSelected);
                    else
                    {
                        inventoryItem = weapon2Slot.GetItem();
                        if (inventoryItem != null)
                            weapon2Slot.SetItem(null);

                        weapon2Slot.SetItem(ItemSelected);
                    }

                    break;
            }
        }

        private void UnEquipItem()
        {
            Item.ItemSlot itemSlot = ItemSelected.item.slot;

            switch (itemSlot)
            {
                case Item.ItemSlot.Head:
                    headSlot.SetItem(null);
                    break;

                case Item.ItemSlot.Body:
                    bodySlot.SetItem(null);
                    break;

                case Item.ItemSlot.Feet:
                    feetSlot.SetItem(null);
                    break;

                case Item.ItemSlot.Weapon:
                    InventoryItem inventoryItem = weapon1Slot.GetItem();
                    if (ItemSelected == inventoryItem)
                        weapon1Slot.SetItem(null);
                    else
                        weapon2Slot.SetItem(null);
                    break;
            }
        }

        #endregion EquipUnEquipButtonFunction

        private void AddInventoryItem(Item item)
        {
            GameObject itemInstance = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
            itemInstance.transform.SetParent(inventoryHolder);
            itemInstance.transform.localScale = Vector3.one;

            InventoryItemDnD inventoryItemDnD = itemInstance.GetComponent<InventoryItemDnD>();

            switch (item.itemClass)
            {
                case Item.ItemClass.Common:
                    itemInstance.GetComponent<Image>().color = Constants.CommonColor;
                    break;

                case Item.ItemClass.Uncommon:
                    itemInstance.GetComponent<Image>().color = Constants.UncommonColor;
                    break;

                case Item.ItemClass.Rare:
                    itemInstance.GetComponent<Image>().color = Constants.RareColor;
                    break;

                case Item.ItemClass.Mythical:
                    itemInstance.GetComponent<Image>().color = Constants.MythicalColor;
                    break;

                case Item.ItemClass.Legendary:
                    itemInstance.GetComponent<Image>().color = Constants.LegendaryColor;
                    break;
            }

            Image inventoryItemImage = itemInstance.transform.GetChild(1).GetComponent<Image>();
            Image inventoryItemBorder = itemInstance.transform.GetChild(0).GetComponent<Image>();
            Text inventoryItemName = itemInstance.transform.GetChild(2).GetComponent<Text>();

            InventoryItem newItem = new InventoryItem
            {
                item = item,
                itemBorder = inventoryItemBorder,

                itemEquipped = false
            };

            inventoryItemDnD.SetItem(newItem);

            inventoryItemImage.sprite = item.icon;
            inventoryItemName.text = item.itemName;
            inventoryItemBorder.sprite = defaultBorder;

            itemInstance.GetComponent<Button>().onClick.AddListener(() => InventoryItemClicked(newItem));
            _items.Add(newItem);
        }

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