using UnityEngine;
using UnityEngine.UI;

namespace UNetUI.Asteroids.Scene._Home
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

        public LobbyManager lobbyManager;
        public InputField hostInput;

        // Called From External Button
        public void OnHostButtonClick()
        {
            lobbyManager.StartMatchMaker();
            lobbyManager.matchMaker.CreateMatch(hostInput.text, (uint) lobbyManager.maxPlayers, true,
                "", "", "", 0, 0,
                lobbyManager.OnMatchCreate);
        }
    }
}