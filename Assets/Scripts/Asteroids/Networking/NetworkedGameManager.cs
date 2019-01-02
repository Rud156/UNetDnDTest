using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UNetUI.Asteroids.Player;
using UNetUI.Asteroids.Scene.MainScene;
using UNetUI.Asteroids.Shared;
using UNetUI.Asteroids.Spawners;
using UNetUI.Extras;

namespace UNetUI.Asteroids.Networking
{
    public class NetworkedGameManager : NetworkManager
    {
        #region Singleton

        private static NetworkedGameManager _instance;

        private void Awake()
        {
            if (_instance == null)
                _instance = this;

            if (_instance != this)
                Destroy(gameObject);
        }

        #endregion Singleton

        private GameObject _scoreHolder;
        private Transform _playerHolder;

        private bool _gameStarted;
        private int _clientsConnected;

        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);

            _playerHolder = GameObject.FindGameObjectWithTag(TagManager.PlayerHolder)?.transform;
            _scoreHolder = GameObject.FindGameObjectWithTag(TagManager.ScoreHolder);

            _scoreHolder.SetActive(true);
        }

        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
        {
            GameObject playerInstance = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
            playerInstance.transform.SetParent(_playerHolder);

            NetworkServer.AddPlayerForConnection(conn, playerInstance, playerControllerId);
        }

        public override void OnServerConnect(NetworkConnection conn)
        {
            base.OnServerConnect(conn);

            _clientsConnected += 1;

            if (_clientsConnected == 2 && !_gameStarted)
            {
                NetworkedScoreManager.instance.StartScoring();
                _gameStarted = true;
            }
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            base.OnServerDisconnect(conn);

            _clientsConnected = 0;
            _gameStarted = false;
        }
    }
}