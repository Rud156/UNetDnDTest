using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UNetUI.Asteroids.Power_Ups;
using UNetUI.Asteroids.Spawners;
using UNetUI.Asteroids.UI;
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
        public float updateSendRate = 0.1f;

        private Transform _asteroidsHolder;
        private Transform _spaceshipHolder;

        private int _currentScore;
        private bool _startScoring;
        
        private float _enemyNextTick;
        private float _pollNextTick;

        private bool _gameEndDisplayed;

        private void Start()
        {
            _asteroidsHolder = GameObject.FindGameObjectWithTag(TagManager.AsteroidsHolder)?.transform;
            _spaceshipHolder = GameObject.FindGameObjectWithTag(TagManager.SpaceshipHolder)?.transform;

            _currentScore = 0;
            UpdateUiWithText();
        }

        private void Update()
        {
            if (!isServer || !_startScoring)
                return;

            _enemyNextTick += Time.deltaTime;
            if (_enemyNextTick / enemyPollRate >= 1)
            {
                _enemyNextTick = 0;
                if (_asteroidsHolder.childCount == 0)
                    AsteroidSpawner.instance.CreateAsteroidsAtScreenEdge();
            }

            _pollNextTick += Time.deltaTime;
            if (_pollNextTick / updateSendRate >= 1)
            {
                _pollNextTick = 0;
                RpcUpdateClientsScoreUi(_currentScore);
            }
        }

        public void AddScore(int scoreAmount)
        {
            if (!isServer)
                return;

            _currentScore = _currentScore + scoreAmount >= 990000 ? 990000 : _currentScore + scoreAmount;

//            if (_currentScore % 500 == 0)
//                NetworkedHealthManager.instance.IncrementHealth();

            if (_currentScore >= 990000 && !_gameEndDisplayed)
            {
                StartCoroutine(EndGame());
                _gameEndDisplayed = true;
                return;
            }

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
        }

        public void StartScoring() => _startScoring = true;

        private IEnumerator EndGame()
        {
            RpcNotifyGameEnd();
            yield return new WaitForSeconds(3.5f);
            ServerClientMenuManager.instance.ExitGame();
        }

        [ClientRpc]
        private void RpcNotifyGameEnd()
        {
            if(isServer)
                return;
            
            InfoTextManager.instance.DisplayText("You Won The Game !!!", Color.green);
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