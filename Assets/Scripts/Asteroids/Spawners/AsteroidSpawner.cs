using UnityEngine;
using UnityEngine.Networking;
using UNetUI.Asteroids.Enemies.Asteroid;
using UNetUI.Extras;

namespace UNetUI.Asteroids.Spawners
{
    public class AsteroidSpawner : NetworkBehaviour
    {
        #region Singleton

        public static AsteroidSpawner instance;

        private void Awake()
        {
            if (instance == null)
                instance = this;

            if (instance != this)
                Destroy(gameObject);
        }

        #endregion Singleton

        public GameObject asteroidPrefab;
        public float leftRightOffset = 1;
        public int minSpawnCount = 4;
        public int maxSpawnCount = 10;

        [Header("Debug")] public bool spawnOnStart;
        private Transform _asteroidsHolder;

        private int _currentSpawnCount;

        private void Start()
        {
            _currentSpawnCount = minSpawnCount;
            _asteroidsHolder = GameObject.FindGameObjectWithTag(TagManager.AsteroidsHolder)?.transform;

            if (spawnOnStart)
                CreateAsteroidsAtScreenEdge();
        }

        public void CreateAsteroidsAtScreenEdge()
        {
            if (!isServer)
                return;

            if (_asteroidsHolder == null)
                _asteroidsHolder = GameObject.FindGameObjectWithTag(TagManager.AsteroidsHolder)?.transform;

            Camera mainCamera = Camera.main;
            Vector3 topLeft = mainCamera.ScreenToWorldPoint(new Vector2(0, mainCamera.pixelHeight));
            Vector3 bottomRight =
                mainCamera.ScreenToWorldPoint(new Vector2(mainCamera.pixelWidth, 0));

            _currentSpawnCount = _currentSpawnCount + 1 > maxSpawnCount ? maxSpawnCount : _currentSpawnCount + 1;

            // Left Asteroids
            for (int i = 0; i < _currentSpawnCount / 2 + 1; i++)
            {
                float randomHeight = ExtensionFunctions.Map(Random.value, 0, 1,
                    bottomRight.y, topLeft.y);

                GameObject asteroidInstance = Instantiate(asteroidPrefab,
                    new Vector2(topLeft.x + leftRightOffset, randomHeight),
                    Quaternion.identity);
                asteroidInstance.transform.SetParent(_asteroidsHolder);

                NetworkServer.Spawn(asteroidInstance);
            }

            // Right Asteroids
            for (int i = 0; i < _currentSpawnCount / 2 + 1; i++)
            {
                float randomHeight = ExtensionFunctions.Map(Random.value, 0, 1,
                    bottomRight.y, topLeft.y);

                GameObject asteroidInstance = Instantiate(asteroidPrefab,
                    new Vector2(bottomRight.x - leftRightOffset, randomHeight),
                    Quaternion.identity);
                asteroidInstance.transform.SetParent(_asteroidsHolder);

                NetworkServer.Spawn(asteroidInstance);
            }
        }
    }
}