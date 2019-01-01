using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UNetUI.Asteroids.Networking;
using UNetUI.Extras;

namespace UNetUI.Asteroids.Scene.MainScene
{
    public class ServerClientMenuManager : NetworkBehaviour
    {
        public GameObject pausePanel;
        public Text serverOrClient;
        public Text addressText;

        private NetworkedGameManager _gameManager;
        private bool _isPausePanelOpen;

        private void Start()
        {
            _gameManager = GameObject.FindGameObjectWithTag(TagManager.GameManager)
                ?.GetComponent<NetworkedGameManager>();

            serverOrClient.text = isServer ? "Server" : "Client";
            addressText.text = _gameManager.networkAddress;

            pausePanel.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(Controls.ExitKey))
            {
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