using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace UNetUI.Asteroids.Scene._Home
{
    public class LobbyManager : NetworkLobbyManager
    {
        #region Singleton

        private static LobbyManager _instance;

        private void Awake()
        {
            if (_instance == null)
                _instance = this;

            if (_instance != this)
                Destroy(gameObject);
        }

        #endregion Singleton

        public GameObject lobby;
        public GameObject mainMenu;

        private void Start()
        {
            lobby.SetActive(false);
            mainMenu.SetActive(true);
        }

        public override void OnStartHost()
        {
            base.OnStartHost();

            lobby.SetActive(true);
            mainMenu.SetActive(false);
        }

        public override void OnStopHost()
        {
            base.OnStopHost();

            lobby.SetActive(false);
            mainMenu.SetActive(true);
        }
    }
}