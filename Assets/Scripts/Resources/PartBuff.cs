using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UNetUI.Extras;

namespace UNetUI.Resources
{
    public class PartBuff : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private Item _buffItem;
        private Image _buffImage;

        private Transform _draggableImage;
        private Image _draggableImageSprite;

        private void Start()
        {
            _buffImage = GetComponent<Image>();

            _draggableImage = GameObject.FindGameObjectWithTag(TagManager.DraggableImage).transform;
            if (!_draggableImage)
                throw new Exception("No Valid Image Found");

            _draggableImageSprite = _draggableImage.GetComponent<Image>();
        }

        public Item GetItem() => _buffItem;

        public void SetItem(Item buffItem)
        {
            _buffImage.sprite = buffItem == null ? null : buffItem.icon;
            _buffItem = buffItem;
        }

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

            List<RaycastResult> results = GraphicRaycastManager.instance.GetHitObjectsUnderMouse();
            if (results.Count <= 0)
            {
                CancelDrop();
                return;
            }

            CheckAndRemoveItem(results[0].gameObject);
        }

        private void CheckAndRemoveItem(GameObject itemBelowPointer)
        {
            if (itemBelowPointer.CompareTag(TagManager.InventoryItem) ||
                itemBelowPointer.CompareTag(TagManager.Inventory))
            {
                PlayerBuffsManager.instance.RemoveItem(_buffItem);
                SetItem(null);
            }
            else if (itemBelowPointer.CompareTag(gameObject.tag))
            {
                itemBelowPointer.GetComponent<PartBuff>().SetItem(_buffItem);
                SetItem(null);
            }
            else
                CancelDrop();
        }

        private void CancelDrop()
        {
            _draggableImageSprite.sprite = null;
            _draggableImageSprite.enabled = false;

            _buffImage.enabled = true;
        }
    }
}