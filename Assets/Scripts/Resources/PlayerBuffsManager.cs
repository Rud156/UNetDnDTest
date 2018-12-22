using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UNetUI.Resources
{
    public class PlayerBuffsManager : MonoBehaviour
    {
        #region Singleton

        private static PlayerBuffsManager _instance;

        private void Awake()
        {
            if (_instance == null)
                _instance = this;

            if (_instance != this)
                Destroy(gameObject);
        }

        #endregion Singleton
    }
}