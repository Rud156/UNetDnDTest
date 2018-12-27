using UnityEngine;
using UnityEngine.Networking;
using UNetUI.Asteroids.Spawners;
using UNetUI.Extras;

namespace UNetUI.Asteroids.Networking
{
    public class GameManager : NetworkManager
    {
        #region Singleton

        private static GameManager _instance;

        private void Awake()
        {
            if (_instance == null)
                _instance = this;

            if (_instance != this)
                Destroy(gameObject);
        }

        #endregion Singleton

        private bool _gameStarted;
        private int _clientsConnected;
        private Transform _playerHolder;

        private void Start() => _playerHolder = GameObject.FindGameObjectWithTag(TagManager.PlayerHolder)?.transform;

        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);

            // TODO: Save these Id's and later use them to create players...
            ClientScene.AddPlayer(conn, (short)Mathf.FloorToInt(Random.value * 16)); 
        }

        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
        {   
            Debug.Log("Adding Player");
            
            GameObject playerInstance = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
            playerInstance.transform.SetParent(_playerHolder);
            NetworkServer.AddPlayerForConnection(conn, playerInstance, playerControllerId);
        }

        public override void OnServerConnect(NetworkConnection conn)
        {
            base.OnServerConnect(conn);

            _clientsConnected += 1;
            
            if(_clientsConnected == 2)
                AsteroidSpawner.instance.CreateAsteroidsAtScreenEdge();
        }

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn);

            _clientsConnected -= 1;
        }
    }
}