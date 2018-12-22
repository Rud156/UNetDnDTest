using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UNetUI.Extras;
using UNetUI.SharedData;

namespace UNetUI.Resources
{
    public class ItemDnD : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private InventoryItem _inventoryItem;

        private Image _inventoryImage;
        private Text _inventoryText;

        private Transform _draggableImage;
        private Image _draggableImageSprite;

        private void Start()
        {
            _inventoryImage = transform.GetChild(0).GetComponent<Image>();
            _inventoryText = transform.GetChild(1).GetComponent<Text>();

            _draggableImage = GameObject.FindGameObjectWithTag(TagManager.DraggableImage).transform;
            if (!_draggableImage)
                throw new Exception("No Valid Image Found");

            _draggableImageSprite = _draggableImage.GetComponent<Image>();
            _draggableImageSprite.raycastTarget = false;
        }

        public void SetItem(InventoryItem item) => _inventoryItem = item;

        public void OnDrag(PointerEventData eventData) => _draggableImage.position = eventData.position;

        public void OnEndDrag(PointerEventData eventData)
        {
            InventoryManager.instance.EnableInventoryScrolling();

            _draggableImageSprite.sprite = null;
            _draggableImageSprite.enabled = false;

            List<RaycastResult> results = GraphicRaycastManager.instance.GetHitObjectsUnderMouse();
            if (results.Count <= 0)
            {
                CancelDrop();
                return;
            }

            CheckAndEquipItem(results[0].gameObject);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            InventoryManager.instance.DisableInventoryScrolling();

            _draggableImageSprite.enabled = true;
            _draggableImageSprite.sprite = _inventoryItem.item.icon;
            _draggableImage.position = transform.position;

            _inventoryImage.enabled = false;
            _inventoryText.enabled = false;
        }

        private void CancelDrop()
        {
            _draggableImageSprite.sprite = null;
            _draggableImageSprite.enabled = false;

            _inventoryImage.enabled = true;
            _inventoryText.enabled = true;
        }

        private void CheckAndEquipItem(GameObject itemBelowPointer)
        {
            if (itemBelowPointer.CompareTag(TagManager.Body) && _inventoryItem.item.slot == Item.ItemSlot.Body)
                ClearAndReplaceItem(itemBelowPointer);
            else if (itemBelowPointer.CompareTag(TagManager.Head) && _inventoryItem.item.slot == Item.ItemSlot.Head)
                ClearAndReplaceItem(itemBelowPointer);
            else if (itemBelowPointer.CompareTag(TagManager.Feet) && _inventoryItem.item.slot == Item.ItemSlot.Feet)
                ClearAndReplaceItem(itemBelowPointer);
            else if (itemBelowPointer.CompareTag(TagManager.Weapon) && _inventoryItem.item.slot == Item.ItemSlot.Weapon)
                ClearAndReplaceItem(itemBelowPointer);
            else
                CancelDrop();
        }

        private void ClearAndReplaceItem(GameObject itemBelowPointer)
        {
            PartBuff partBuff = itemBelowPointer.GetComponent<PartBuff>();
            Item previousItem = partBuff.GetItem();

            if (previousItem != null)
            {
                InventoryManager.instance.AddInventoryItem(previousItem);
                PlayerBuffsManager.instance.RemoveItem(previousItem);
            }

            itemBelowPointer.GetComponent<Image>().sprite = _inventoryItem.item.icon;

            partBuff.SetItem(_inventoryItem.item);
            PlayerBuffsManager.instance.AddItem(_inventoryItem.item);

            Destroy(gameObject);
        }
    }
}