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
//        private GameObject _healthHolder;
        private Transform _playerHolder;

        private bool _gameStarted;
        private int _clientsConnected;

        private bool _objectsSaved;

        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);
            
            if (!_objectsSaved)
            {

                _playerHolder = GameObject.FindGameObjectWithTag(TagManager.PlayerHolder)?.transform;
                _scoreHolder = GameObject.FindGameObjectWithTag(TagManager.ScoreHolder);
//                _healthHolder = GameObject.FindGameObjectWithTag(TagManager.HealthHolder);

                _scoreHolder.SetActive(true);
//                _healthHolder.SetActive(true);

                _objectsSaved = true;
            }
            
            Debug.Log("Client Connected");
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