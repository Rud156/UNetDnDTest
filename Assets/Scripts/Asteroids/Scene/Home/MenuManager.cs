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

        public void OnHostButtonClick() => gameManager.StartHost();

        public void OnClientButtonClick()
        {
            gameManager.networkAddress = addressInput.text;
            gameManager.StartClient();
        }
    }
}