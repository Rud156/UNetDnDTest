﻿using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using EZCameraShake;
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
        public CameraShakerData shakerData;

        [Header("Spawn Count")] public int minSpawnCount;
        public int maxSpawnCount;

        private HealthSetter _healthSetter;
        private Transform _asteroidsHolder;

        private void Start()
        {
            if (!isServer)
                return;

            _healthSetter = GetComponent<HealthSetter>();
            _healthSetter.healthZero += DestroyAsteroid;

            _asteroidsHolder = GameObject.FindGameObjectWithTag(TagManager.AsteroidsHolder)?.transform;
        }

        private void DestroyAsteroid()
        {
            if (!isServer)
                return;

            if (spawnMiniAsteroid)
            {
                int asteroidSpawnCount = Random.Range(minSpawnCount, maxSpawnCount);

                for (int i = 0; i < asteroidSpawnCount; i++)
                {
                    GameObject miniAsteroidInstance = Instantiate(miniAsteroid, transform.position,
                        Quaternion.Euler(0, 0, Random.value * 360));
                    miniAsteroidInstance.transform.SetParent(_asteroidsHolder);

                    NetworkServer.Spawn(miniAsteroidInstance);
                }
            }

            int scoreAmount = GetComponent<ScoreSetter>().scoreAmount;
            NetworkedScoreManager.instance.AddScore(scoreAmount);

            RpcShakeClientsOnDestroy();
            NetworkServer.Destroy(gameObject);
        }

        [ClientRpc]
        private void RpcShakeClientsOnDestroy()
        {
            if (isServer)
                return;

            CameraShaker.Instance.ShakeOnce(
                shakerData.magnitude,
                shakerData.roughness,
                shakerData.fadeInTime,
                shakerData.fadeOutTime
            );
        }
    }
}