using UnityEngine;
using UnityEngine.UI;
using UNetUI.Asteroids.Networking;

namespace UNetUI.Asteroids.Scene.Home
{
    public class MenuManager : MonoBehaviour
    {
        #region Singleton

        private static MenuManager _instance;

        private void Awake()
        {
            if (_instance == null)
                _instance = this;

            if (_instance != this)
                Destroy(gameObject);
        }

        #endregion Singleton

        public NetworkedGameManager gameManager;
        public InputField addressInput;
        public Button hostButton;
        public Button clientButton;

        private void Start()
        {
            addressInput.text = "localhost";
            hostButton.onClick.AddListener(OnHostButtonClick);
            clientButton.onClick.AddListener(OnClientButtonClick);
        }

        private void OnHostButtonClick() => gameManager.StartHost();

        private void OnClientButtonClick()
        {
            gameManager.networkAddress = addressInput.text;
            gameManager.StartClient();
        }
    }
}