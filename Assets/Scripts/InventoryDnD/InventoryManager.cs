﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UNetUI.Extras;

namespace UNetUI.InventoryDnD
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

        [Header("Borders")] public Sprite defaultBorder;
        public Sprite selectedBorder;

        [Header("Item Equip Slots")] public PartBuff headSlot;
        public PartBuff bodySlot;
        public PartBuff weapon1Slot;
        public PartBuff weapon2Slot;
        public PartBuff feetSlot;

        [Header("Inventory")] public RectTransform inventoryHolder;
        public ScrollRect inventoryScroller;

        [Header("Item Details")] public Image itemImage;
        public Text itemName;
        public Text itemDescription;
        public Text damageText;
        public Text strengthText;
        public Text intelText;
        public Text agilityText;
        public Text defenceText;

        [Header("Inventory Items")] public GameObject itemPrefab;

        [Header("Selected Holder")] public GameObject selectedHolder;
        public GameObject deselectedHolder;

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
            SetSortType();
        }

        #region Scroller

        public void DisableInventoryScrolling()
        {
            inventoryScroller.enabled = false;
        }

        public void EnableInventoryScrolling()
        {
            inventoryScroller.enabled = true;
        }

        #endregion Scroller

        #region EquipUnEquipButtonFunction

        // Called from external Button
        public void EquipUnEquipButton()
        {
            if (ItemSelected == null)
                return;

            if (ItemSelected.itemEquipped)
                UnEquipSelectedItem();
            else
                CheckAndEquipSelectedItem();

            PlayerBuffsManager.instance.ClearBuffsAddition();
        }

        private void CheckAndEquipSelectedItem()
        {
            var itemSlot = ItemSelected.item.slot;
            Item inventoryItem = null;

            switch (itemSlot)
            {
                case Item.ItemSlot.Head:
                    inventoryItem = headSlot.GetItem();
                    if (inventoryItem != null)
                        headSlot.SetItem(null);

                    headSlot.SetItem(ItemSelected.item);
                    break;

                case Item.ItemSlot.Body:
                    inventoryItem = bodySlot.GetItem();
                    if (inventoryItem != null)
                        bodySlot.SetItem(null);

                    bodySlot.SetItem(ItemSelected.item);
                    break;

                case Item.ItemSlot.Feet:
                    inventoryItem = feetSlot.GetItem();
                    if (inventoryItem != null)
                        feetSlot.SetItem(null);

                    feetSlot.SetItem(ItemSelected.item);
                    break;

                case Item.ItemSlot.Weapon:
                    inventoryItem = weapon1Slot.GetItem();
                    if (inventoryItem == null)
                    {
                        weapon1Slot.SetItem(ItemSelected.item);
                    }
                    else
                    {
                        inventoryItem = weapon2Slot.GetItem();
                        if (inventoryItem != null)
                            weapon2Slot.SetItem(null);

                        weapon2Slot.SetItem(ItemSelected.item);
                    }

                    break;
            }

            SetItemUnEquipped(inventoryItem);
            ItemSelected.itemEquipped = true;
        }

        private void UnEquipSelectedItem()
        {
            var itemSlot = ItemSelected.item.slot;

            switch (itemSlot)
            {
                case Item.ItemSlot.Head:
                    headSlot.SetItem(null);
                    ItemSelected.itemEquipped = false;
                    break;

                case Item.ItemSlot.Body:
                    bodySlot.SetItem(null);
                    ItemSelected.itemEquipped = false;
                    break;

                case Item.ItemSlot.Feet:
                    feetSlot.SetItem(null);
                    ItemSelected.itemEquipped = false;
                    break;

                case Item.ItemSlot.Weapon:
                    var inventoryItem = weapon1Slot.GetItem();
                    if (ItemSelected.item == inventoryItem)
                        weapon1Slot.SetItem(null);
                    else
                        weapon2Slot.SetItem(null);

                    ItemSelected.itemEquipped = false;
                    break;
            }
        }

        #endregion EquipUnEquipButtonFunction

        #region ExternalInventoryManipulation

        public void SetItemSelected(InventoryItem inventoryItem)
        {
            ItemSelected = inventoryItem;
        }

        public void CheckAndAddInventoryItem(Item item)
        {
            var itemExists = false;

            for (var i = 0; i < _items.Count; i++)
                if (_items[i].item.itemName == item.itemName)
                {
                    itemExists = true;
                    break;
                }

            if (!itemExists)
                AddInventoryItem(item);

            UpdateUiWithItemSelected();
        }

        public void SetItemUnEquipped(Item item)
        {
            if (item == null)
                return;

            foreach (var inventoryItem in _items)
            {
                if (inventoryItem.item.itemName != item.itemName)
                    continue;

                inventoryItem.itemEquipped = false;
                break;
            }
        }

        public void SetItemEquipped(Item item)
        {
            if (item == null)
                return;

            foreach (var inventoryItem in _items)
            {
                if (inventoryItem.item.itemName != item.itemName)
                    continue;

                inventoryItem.itemEquipped = true;
                break;
            }
        }

        #endregion ExternalInventoryManipulation

        #region Sorting

        public void SetSortType(int index = 3)
        {
            switch (index)
            {
                case 0:
                    _items = _items.OrderBy(_ => _.item.itemName)
                        .GroupBy(_ => _.item.slot)
                        .OrderByDescending(_ => _.Key)
                        .SelectMany(_ => _)
                        .ToList();

                    break;

                case 1:
                    _items = _items.OrderBy(_ => _.item.itemName)
                        .GroupBy(_ => _.item.itemClass)
                        .OrderByDescending(_ => _.Key)
                        .SelectMany(_ => _)
                        .ToList();
                    break;

                case 2:
                    _items = _items.OrderBy(_ => _.item.itemName)
                        .ToList();
                    break;

                default:
                    _items = _items.OrderByDescending(_ => _.item.itemClass)
                        .GroupBy(_ => _.item.slot)
                        .SelectMany(_ => _)
                        .ToList();
                    break;
            }

            for (var i = 0; i < _items.Count; i++)
                _items[i].itemInstance.transform.SetSiblingIndex(i);
        }

        #endregion Sorting

        private void AddInventoryItem(Item item)
        {
            var itemInstance = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
            itemInstance.transform.SetParent(inventoryHolder);
            itemInstance.transform.localScale = Vector3.one;

            var inventoryItemDnD = itemInstance.GetComponent<InventoryItemDnD>();

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

            var inventoryItemImage = itemInstance.transform.GetChild(1).GetComponent<Image>();
            var inventoryItemBorder = itemInstance.transform.GetChild(0).GetComponent<Image>();
            var inventoryItemName = itemInstance.transform.GetChild(2).GetComponent<Text>();

            var newItem = new InventoryItem
            {
                item = item,
                itemBorder = inventoryItemBorder,

                itemInstance = itemInstance,

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
            var items = ItemsManager.instance.GetItems();
            foreach (var item in items)
                AddInventoryItem(item);
        }

        private void UpdateUiWithItemSelected()
        {
            foreach (var inventoryItem in _items)
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

                    Item equippedItem = null;
                    switch (inventoryItem.item.slot)
                    {
                        case Item.ItemSlot.Head:
                            equippedItem = headSlot.GetItem();
                            break;

                        case Item.ItemSlot.Body:
                            equippedItem = bodySlot.GetItem();
                            break;

                        case Item.ItemSlot.Feet:
                            equippedItem = feetSlot.GetItem();
                            break;

                        case Item.ItemSlot.Weapon:
                            equippedItem = weapon1Slot.GetItem();
                            if (equippedItem != null)
                                equippedItem = weapon2Slot.GetItem();
                            break;
                    }

                    deselectedHolder.SetActive(false);
                    selectedHolder.SetActive(true);
                    PlayerBuffsManager.instance.DisplayBuffDifference(equippedItem, inventoryItem.item);
                }
                else
                {
                    inventoryItem.itemBorder.sprite = defaultBorder;
                }
        }

        private void InventoryItemClicked(InventoryItem item)
        {
            ItemSelected = item;
        }
    }
}