using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UNetUI.Asteroids.Scene.MainScene
{
    public class NetworkedHealthManager : MonoBehaviour
    {
        #region Singleton

        public static NetworkedHealthManager instance;

        private void Awake()
        {
            if (instance == null)
                instance = this;

            if (instance != this)
                Destroy(gameObject);
        }

        #endregion Singleton

        public Text healthCountText;

        private int _currentHealth;

        private void Start() => _currentHealth = 3;

        public void ReduceHealth()
        {
            if (_currentHealth - 1 >= 0)
                _currentHealth -= 1;
            
            UpdateUiWithText();
        }

        public void AddHealth()
        {
            _currentHealth += 1;
            UpdateUiWithText();
        }

        private void UpdateUiWithText() => healthCountText.text = $"X {_currentHealth}";
    }
}