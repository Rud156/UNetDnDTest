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

        public override void OnServerConnect(NetworkConnection conn)
        {
            base.OnServerConnect(conn);

//            if (!_gameStarted)
//            {
//                _gameStarted = true;
//                AsteroidSpawner.instance.CreateAsteroidsAtScreenEdge();
//            }
        }
    }
}