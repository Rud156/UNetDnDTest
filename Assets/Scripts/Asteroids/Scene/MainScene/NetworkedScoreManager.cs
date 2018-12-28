using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace UNetUI.Asteroids.Scene.MainScene
{
    public class NetworkedScoreManager : NetworkBehaviour
    {
        #region Singleton

        public static NetworkedScoreManager instance;

        private void Awake()
        {
            if (instance == null)
                instance = this;

            if (instance != this)
                Destroy(gameObject);
        }

        #endregion Singleton

        public Text scoreDisplay;

        private float _currentScore;

        private void Start() => _currentScore = 0;

        public void AddScore(float scoreAmount)
        {
            _currentScore += scoreAmount;
            scoreDisplay.text = _currentScore.ToString();
        }
    }
}