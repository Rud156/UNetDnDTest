using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UNetUI.Asteroids.Shared;
using UNetUI.Extras;

namespace UNetUI.Asteroids.Enemies.Asteroid
{
    [RequireComponent(typeof(HealthSetter))]
    public class AsteroidDestroy : MonoBehaviour
    {
        public bool spawnMiniAsteroid;
        public GameObject miniAsteroid;

        [Header("Spawn Count")] public int minSpawnCount;
        public int maxSpawnCount;

        private HealthSetter _healthSetter;
        private Transform _asteroidsHolder;

        // Use this for initialization
        private void Start()
        {
            _healthSetter = GetComponent<HealthSetter>();
            _healthSetter.healthZero += DestroyAsteroid;

            _asteroidsHolder = GameObject.FindGameObjectWithTag(TagManager.AsteroidsHolder)?.transform;
        }

        private void DestroyAsteroid()
        {
            if (spawnMiniAsteroid)
            {
                int asteroidSpawnCount = Random.Range(minSpawnCount, maxSpawnCount);

                for (int i = 0; i < asteroidSpawnCount; i++)
                {
                    GameObject miniAsteroidInstance = Instantiate(miniAsteroid, transform.position,
                        Quaternion.Euler(0, 0, Random.value * 360));
                    miniAsteroidInstance.transform.SetParent(_asteroidsHolder);
                }
            }

            Destroy(gameObject);
        }
    }
}