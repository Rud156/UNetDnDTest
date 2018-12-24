using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UNetUI.Extras;

namespace UNetUI.InventoryDnD
{
    public class InventoryItemDnD : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private Transform _draggableImage;
        private Image _draggableImageSprite;
        private InventoryItem _inventoryItem;

        public void OnBeginDrag(PointerEventData eventData)
        {
            InventoryManager.instance.DisableInventoryScrolling();
            InventoryManager.instance.SetItemSelected(_inventoryItem);

            _draggableImageSprite.enabled = true;
            _draggableImageSprite.sprite = _inventoryItem.item.icon;
            _draggableImage.position = transform.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            _draggableImage.position = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            InventoryManager.instance.EnableInventoryScrolling();

            _draggableImageSprite.sprite = null;
            _draggableImageSprite.enabled = false;

            var results = GraphicRaycastManager.instance.GetHitObjectsUnderMouse();
            if (results.Count <= 0 || _inventoryItem.itemEquipped)
            {
                CancelDrop();
                return;
            }

            CheckAndEquipItem(results[0].gameObject);
        }

        private void Start()
        {
            _draggableImage = GameObject.FindGameObjectWithTag(TagManager.DraggableImage).transform;
            if (!_draggableImage)
                throw new Exception("No Valid Image Found");

            _draggableImageSprite = _draggableImage.GetComponent<Image>();
        }

        public void SetItem(InventoryItem item)
        {
            _inventoryItem = item;
        }

        private void CancelDrop()
        {
            _draggableImageSprite.sprite = null;
            _draggableImageSprite.enabled = false;
        }

        private void CheckAndEquipItem(GameObject itemBelowPointer)
        {
            if (itemBelowPointer.CompareTag(TagManager.Body) && _inventoryItem.item.slot == Item.ItemSlot.Body)
                ClearAndReplaceItem(itemBelowPointer);
            else if (itemBelowPointer.CompareTag(TagManager.Head) && _inventoryItem.item.slot == Item.ItemSlot.Head)
                ClearAndReplaceItem(itemBelowPointer);
            else if (itemBelowPointer.CompareTag(TagManager.Feet) && _inventoryItem.item.slot == Item.ItemSlot.Feet)
                ClearAndReplaceItem(itemBelowPointer);
            else if ((itemBelowPointer.CompareTag(TagManager.Weapon1) ||
                      itemBelowPointer.CompareTag(TagManager.Weapon2)) &&
                     _inventoryItem.item.slot == Item.ItemSlot.Weapon)
                ClearAndReplaceItem(itemBelowPointer);
            else
                CancelDrop();
        }

        private void ClearAndReplaceItem(GameObject itemBelowPointer)
        {
            var partBuff = itemBelowPointer.GetComponent<PartBuff>();
            var previousItem = partBuff.GetItem();

            if (previousItem != null)
            {
                partBuff.SetItem(null);
                InventoryManager.instance.SetItemUnEquipped(previousItem);
            }

            _inventoryItem.itemEquipped = true;
            partBuff.SetItem(_inventoryItem.item);
            PlayerBuffsManager.instance.ClearBuffsAddition();
        }
    }
}