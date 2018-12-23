using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UNetUI.Resources
{
    public class GraphicRaycastManager : MonoBehaviour
    {
        #region Singleton

        public static GraphicRaycastManager instance;

        private void Awake()
        {
            if (instance == null)
                instance = this;

            if (instance != this)
                Destroy(gameObject);
        }

        #endregion Singleton
        
        public EventSystem eventSystem;
        public GraphicRaycaster graphicRaycaster;
        
        private PointerEventData _pointerEventData;

        public List<RaycastResult> GetHitObjectsUnderMouse()
        {
            var results = new List<RaycastResult>();
            _pointerEventData = new PointerEventData(eventSystem)
            {
                position = Input.mousePosition
            };

            graphicRaycaster.Raycast(_pointerEventData, results);
            return results;
        }
    }
}