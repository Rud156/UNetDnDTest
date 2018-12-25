using UnityEngine;
using UNetUI.Extras;

namespace UNetUI.Asteroids.Spawners
{
    public class AsteroidSpawner : MonoBehaviour
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

        public GameObject asteroidPrefab;
        public int spawnCount;
        public float leftRightOffset = 1;

        private void Start() => CreateAsteroidsAtScreenEdge();

        private void CreateAsteroidsAtScreenEdge()
        {
            Camera mainCamera = Camera.main;
            Vector3 topLeft = mainCamera.ScreenToWorldPoint(new Vector2(0, mainCamera.pixelHeight));
            Vector3 bottomRight =
                mainCamera.ScreenToWorldPoint(new Vector2(mainCamera.pixelWidth, 0));

            for (int i = 0; i < spawnCount; i++)
            {
                float randomHeight = ExtensionFunctions.Map(Random.value, 0, 1,
                    bottomRight.y, topLeft.y);

                Instantiate(asteroidPrefab,
                    Random.value > 0.5f
                        ? new Vector2(bottomRight.x - leftRightOffset, randomHeight)
                        : new Vector2(topLeft.x + leftRightOffset, randomHeight),
                    Quaternion.identity);
            }
        }
    }
}