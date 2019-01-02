using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UNetUI.Asteroids.Networking;
using UNetUI.Extras;

namespace UNetUI.Asteroids.Scene.MainScene
{
    public class ServerClientMenuManager : NetworkBehaviour
    {
        #region Singleton

        public static ServerClientMenuManager instance;

        private void Awake()
        {
            if (instance == null)
                instance = this;

            if (instance != this)
                Destroy(gameObject);
        }

        #endregion Singleton

        public GameObject pausePanel;
        public Text serverOrClient;
        public Text addressText;
        public Button stopButton;

        private NetworkedGameManager _gameManager;
        private bool _isPausePanelOpen;

        private void Start()
        {
            _gameManager = GameObject.FindGameObjectWithTag(TagManager.GameManager)
                ?.GetComponent<NetworkedGameManager>();

            serverOrClient.text = isServer ? "Server" : "Client";
            addressText.text = $"Address: {_gameManager.networkAddress}";

            stopButton.onClick.AddListener(ExitGame);
            pausePanel.SetActive(false);
        }

        private void Update()
        {
            if (!Input.GetKeyDown(Controls.ExitKey))
                return;

            if (_isPausePanelOpen)
            {
                pausePanel.SetActive(false);
                _isPausePanelOpen = false;
            }
            else
            {
                pausePanel.SetActive(true);
                _isPausePanelOpen = true;
            }
        }

        public void ExitGame()
        {
            if (isServer)
                _gameManager.StopHost();
            else
                _gameManager.StopClient();
        }
    }
}