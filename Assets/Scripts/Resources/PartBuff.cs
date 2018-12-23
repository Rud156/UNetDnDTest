using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UNetUI.Extras;

namespace UNetUI.Resources
{
    public class PartBuff : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public string[] acceptedTags;
        
        private Image _buffImage;

        private Item _buffItem;

        private Transform _draggableImage;
        private Image _draggableImageSprite;

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_buffItem == null)
                return;

            _buffImage.enabled = false;

            _draggableImageSprite.enabled = true;
            _draggableImageSprite.sprite = _buffItem.icon;
            _draggableImage.position = transform.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_buffItem == null)
                return;

            _draggableImage.position = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_buffItem == null)
                return;

            _draggableImageSprite.sprite = null;
            _draggableImageSprite.enabled = false;

            _buffImage.enabled = true;

            var results = GraphicRaycastManager.instance.GetHitObjectsUnderMouse();
            if (results.Count <= 0)
            {
                CancelDrop();
                return;
            }

            CheckAndRemoveItem(results[0].gameObject);
        }

        private void Start()
        {
            _buffImage = GetComponent<Image>();

            _draggableImage = GameObject.FindGameObjectWithTag(TagManager.DraggableImage).transform;
            if (!_draggableImage)
                throw new Exception("No Valid Image Found");

            _draggableImageSprite = _draggableImage.GetComponent<Image>();

            CheckAndLoadData();
        }

        public Item GetItem()
        {
            return _buffItem;
        }

        public void SetItem(Item item)
        {
            if (item != null)
                PlayerBuffsManager.instance.AddItem(item);
            else if (_buffItem != null)
                PlayerBuffsManager.instance.RemoveItem(_buffItem);

            _buffImage.sprite = item != null ? item.icon : null;
            _buffItem = item;

            CheckAndSaveData();
        }

        private void CheckAndRemoveItem(GameObject itemBelowPointer)
        {
            if (itemBelowPointer.CompareTag(TagManager.InventoryItem) ||
                itemBelowPointer.CompareTag(TagManager.Inventory))
            {
                InventoryManager.instance.SetItemUnEquipped(_buffItem);
                SetItem(null);
                PlayerBuffsManager.instance.ClearBuffsAddition();
            }
            else if (acceptedTags.Contains(itemBelowPointer.tag) && !itemBelowPointer.CompareTag(gameObject.tag))
            {
                itemBelowPointer.GetComponent<PartBuff>().SetItem(_buffItem);
                SetItem(null);
                PlayerBuffsManager.instance.ClearBuffsAddition();
            }
            else
            {
                CancelDrop();
            }
        }

        private void CancelDrop()
        {
            _draggableImageSprite.sprite = null;
            _draggableImageSprite.enabled = false;

            _buffImage.enabled = true;
        }

        #region LoadAndSaveData

        private void CheckAndLoadData()
        {
            var item = ItemsDataSaver.LoadEquippedItem(gameObject.tag);
            if (item != null)
            {
                SetItem(item);
                InventoryManager.instance.CheckAndAddInventoryItem(item);
                InventoryManager.instance.SetItemEquipped(item);
            }
        }

        private void CheckAndSaveData()
        {
            if (_buffItem != null)
                ItemsDataSaver.SaveEquippedItems(_buffItem, gameObject.tag);
            else
                ItemsDataSaver.RemoveSavedData(gameObject.tag);
        }

        #endregion
    }
}