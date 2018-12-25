using UnityEngine.Networking;

namespace UNetUI.Asteroids.Spawners
{
    public class AsteroidSpawner : NetworkBehaviour
    {
        #region Singleton

        private static AsteroidSpawner _instance;

        private void Awake()
        {
            if (_instance == null)
                _instance = this;

            if (_instance != this)
                Destroy(gameObject);
        }

        #endregion Singleton

        private void Start()
        {
            if (isServer)
                CreateAsteroidsAtScreenEdge();
        }

        private void CreateAsteroidsAtScreenEdge()
        {
        }
    }
}