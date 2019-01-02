using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace UNetUI.Asteroids.Scene.MainScene
{
    public class NetworkedHealthManager : NetworkBehaviour
    {
        #region Singleton

        private static NetworkedHealthManager _instance;

        private void Awake()
        {
            if (_instance == null)
                _instance = this;

            if (_instance != this)
                Destroy(gameObject);
        }

        #endregion Singleton

        public Text healthCountText;

        private int _currentHealth;

        private void Start()
        {
            _currentHealth = 3;
            UpdateUiWithText();
        }

        public void IncrementHealth()
        {
            if (!isServer)
                return;

            _currentHealth += 1;
            UpdateUiWithText();
            RpcUpdateClientsHealthUi(_currentHealth);
        }

        public void ReduceHealth()
        {
            if (!isServer)
                return;

            if (_currentHealth - 1 >= 0)
                _currentHealth -= 1;

            UpdateUiWithText();
            RpcUpdateClientsHealthUi(_currentHealth);
        }

        [ClientRpc]
        private void RpcUpdateClientsHealthUi(int currentHealth)
        {
            if (isServer)
                return;

            _currentHealth = currentHealth;
            UpdateUiWithText();
        }

        private void UpdateUiWithText() => healthCountText.text = $"X {_currentHealth}";
    }
}