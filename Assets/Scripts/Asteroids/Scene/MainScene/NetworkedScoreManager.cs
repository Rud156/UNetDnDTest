using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UNetUI.Extras;

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

        private int _currentScore;

        private void Start()
        {
            _currentScore = 0;
            UpdateUiWithText();
        }

        [Command]
        public void CmdAddScore(int scoreAmount)
        {
            if (!isServer)
                return;

            _currentScore += scoreAmount;
            UpdateUiWithText();
            Debug.Log("Score Added");

            RpcUpdateClientsScoreUi(_currentScore);
        }

        [ClientRpc]
        private void RpcUpdateClientsScoreUi(int currentScore)
        {
            if (isServer)
                return;

            _currentScore = currentScore;
            UpdateUiWithText();
        }

        private void UpdateUiWithText() => scoreDisplay.text = ExtensionFunctions.FormatWithCommas(_currentScore);
    }
}