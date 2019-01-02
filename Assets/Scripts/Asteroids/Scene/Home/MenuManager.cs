using UnityEngine;
using UnityEngine.UI;
using UNetUI.Asteroids.Networking;
using UNetUI.Extras;

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

        public InputField addressInput;
        public Button hostButton;
        public Button clientButton;
        public Button quitButton;
        
        private NetworkedGameManager _gameManager;

        private void Start()
        {
            _gameManager = GameObject.FindGameObjectWithTag(TagManager.GameManager)
                ?.GetComponent<NetworkedGameManager>();
            
            addressInput.text = "localhost";
            
            hostButton.onClick.AddListener(OnHostButtonClick);
            clientButton.onClick.AddListener(OnClientButtonClick);
            quitButton.onClick.AddListener(QuitGame);
        }

        private void OnHostButtonClick() => _gameManager.StartHost();

        private void OnClientButtonClick()
        {
            _gameManager.networkAddress = addressInput.text;
            _gameManager.StartClient();
        }

        private void QuitGame() => Application.Quit();
    }
}