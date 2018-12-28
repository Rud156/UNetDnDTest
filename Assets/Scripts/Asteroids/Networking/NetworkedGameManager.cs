using UnityEngine;
using UnityEngine.Networking;
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

        public GameObject scoreHolder;
        public GameObject healthHolder;
        public Transform playerHolder;
            
        private bool _gameStarted;
        private int _clientsConnected;

        private void Start()
        {
            scoreHolder.SetActive(false);
            healthHolder.SetActive(false);
        }


        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);
            
            scoreHolder.SetActive(true);
            healthHolder.SetActive(true);

            // TODO: Save these Id's and later use them to create players...
            ClientScene.AddPlayer(conn, (short)Mathf.FloorToInt(Random.value * 16)); 
        }

        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
        {    
            Debug.Log("Adding Player");
            
            GameObject playerInstance = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
            playerInstance.transform.SetParent(playerHolder);
            NetworkServer.AddPlayerForConnection(conn, playerInstance, playerControllerId);
        }

        public override void OnServerConnect(NetworkConnection conn)
        {
            base.OnServerConnect(conn);

            _clientsConnected += 1;
            
            if(_clientsConnected == 1)
                AsteroidSpawner.instance.CreateAsteroidsAtScreenEdge();
        }

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn);

            _clientsConnected -= 1;
        }
    }
}