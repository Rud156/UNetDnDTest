using UnityEngine.Networking;
using UNetUI.Asteroids.Spawners;

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