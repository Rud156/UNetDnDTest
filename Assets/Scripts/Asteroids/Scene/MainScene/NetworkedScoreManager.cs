using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UNetUI.Asteroids.Power_Ups;
using UNetUI.Asteroids.Spawners;
using UNetUI.Extras;

namespace UNetUI.Asteroids.Scene.MainScene
{
    public class NetworkedScoreManager : NetworkBehaviour
    {
        #region Singleton

        public static NetworkedScoreManager instance;

        private void Awake()
        {
            if (instance == null)
                instance = this;

            if (instance != this)
                Destroy(gameObject);
        }

        #endregion Singleton

        public Text scoreDisplay;
        public float enemyPollRate;

        private Transform _asteroidsHolder;
        private Transform _spaceshipHolder;

        private int _currentScore;
        private float _nextTick;

        private void Start()
        {
            _asteroidsHolder = GameObject.FindGameObjectWithTag(TagManager.AsteroidsHolder)?.transform;
            _spaceshipHolder = GameObject.FindGameObjectWithTag(TagManager.SpaceshipHolder)?.transform;

            _currentScore = 0;
            UpdateUiWithText();
        }

        private void Update()
        {
            if (!isServer)
                return;

            _nextTick += Time.deltaTime;
            if (_nextTick / enemyPollRate >= 1)
            {
                _nextTick = 0;
                if (_asteroidsHolder.childCount == 0)
                    AsteroidSpawner.instance.CreateAsteroidsAtScreenEdge();
            }
        }

        public void AddScore(int scoreAmount)
        {
            if (!isServer)
                return;

            _currentScore += scoreAmount;

            if (_currentScore % 500 == 0)
                NetworkedHealthManager.instance.IncrementHealth();

            if (_currentScore % 500 == 0 && _spaceshipHolder.childCount == 0)
            {
                if (_currentScore < 40000)
                {
                    float randomValue = Random.value;
                    if (randomValue < 0.5f)
                        SpaceshipSpawner.instance.SpawnSmallSpaceship();
                    else
                        SpaceshipSpawner.instance.SpawnLargeSpaceship();
                }
                else
                    SpaceshipSpawner.instance.SpawnSmallSpaceship();
            }

            UpdateUiWithText();

            RpcUpdateClientsScoreUi(_currentScore);
        }

        [ClientRpc]
        private void RpcUpdateClientsScoreUi(int currentScore)
        {
            if (isServer)
                return;

            _currentScore = currentScore;
            UpdateUiWithText();
        }

        private void UpdateUiWithText() => scoreDisplay.text = ExtensionFunctions.FormatWithCommas(_currentScore);
    }
}