using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;
using UnityEngine.Networking;
using UNetUI.Asteroids.Scene.MainScene;
using UNetUI.Asteroids.Shared;
using UNetUI.Extras;

namespace UNetUI.Asteroids.Enemies.Asteroid
{
    [RequireComponent(typeof(HealthSetter))]
    [RequireComponent(typeof(ScoreSetter))]
    public class AsteroidDestroy : NetworkBehaviour
    {
        public bool spawnMiniAsteroid;
        public GameObject miniAsteroid;

        [Header("Spawn Count")] public int minSpawnCount;
        public int maxSpawnCount;

        private HealthSetter _healthSetter;
        private Transform _asteroidsHolder;

        private void Start()
        {
            if(!isServer)
                return;
            
            _healthSetter = GetComponent<HealthSetter>();
            _healthSetter.healthZero += DestroyAsteroid;

            _asteroidsHolder = GameObject.FindGameObjectWithTag(TagManager.AsteroidsHolder)?.transform;
        }

        private void DestroyAsteroid()
        {
            if(!isServer)
                return;

            if (spawnMiniAsteroid)
            {
                int asteroidSpawnCount = Random.Range(minSpawnCount, maxSpawnCount);

                for (int i = 0; i < asteroidSpawnCount; i++)
                {
                    GameObject miniAsteroidInstance = Instantiate(miniAsteroid, transform.position,
                        Quaternion.Euler(0, 0, Random.value * 360));
                    miniAsteroidInstance.transform.SetParent(_asteroidsHolder);
                    
                    Rigidbody2D rb = miniAsteroidInstance.GetComponent<Rigidbody2D>();
                    float launchVelocity = miniAsteroidInstance.GetComponent<AsteroidMovement>().launchVelocity;

                    int launchAngle = Random.Range(0, 360);
                    Vector2 launchVector = new Vector2(
                        Mathf.Cos(launchAngle * Mathf.Deg2Rad) * launchVelocity,
                        Mathf.Sin(launchAngle * Mathf.Deg2Rad) * launchVelocity
                    );
                    rb.AddForce(launchVector, ForceMode2D.Impulse);
                    
                    NetworkServer.Spawn(miniAsteroidInstance);
                }
            }

            int scoreAmount = GetComponent<ScoreSetter>().scoreAmount;
            NetworkedScoreManager.instance.AddScore(scoreAmount);

            NetworkServer.Destroy(gameObject);
        }
    }
}