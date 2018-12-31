using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UNetUI.Asteroids.NetworkedData.Common;
using UNetUI.Asteroids.Shared;
using UNetUI.Extras;

namespace UNetUI.Asteroids.Enemies.Asteroid
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(ScreenWrapper))]
    public class AsteroidMovement : NetworkBehaviour
    {
        public float launchVelocity;
        public float rotationSpeed;

        [Header("Network Data")] public bool isPredictionEnabled = true;
        public float updateSendRate = 0.1f;

        private Rigidbody2D _asteroidRb;
        private ScreenWrapper _screenWrapper;

        private bool _defaultsSet;
        private float _nextTick;

        private List<PositionTimestampReceivePackage> _predictedPackages;

        private void Start()
        {
            SetDefaults();

            if (!isServer)
                return;

            int launchAngle = Random.Range(0, 360);
            Vector2 launchVector = new Vector2(
                Mathf.Cos(launchAngle * Mathf.Deg2Rad) * launchVelocity,
                Mathf.Sin(launchAngle * Mathf.Deg2Rad) * launchVelocity
            );
            _asteroidRb.AddForce(launchVector, ForceMode2D.Impulse);

            RpcSendInitialAsteroidForceToClients(launchVector);
        }

        private void SetDefaults()
        {
            if (_defaultsSet)
                return;

            _predictedPackages = new List<PositionTimestampReceivePackage>();
            _screenWrapper = GetComponent<ScreenWrapper>();
            _asteroidRb = GetComponent<Rigidbody2D>();
            _defaultsSet = true;
        }

        [ClientRpc]
        private void RpcSendInitialAsteroidForceToClients(Vector2 force)
        {
            if (isServer)
                return;

            SetDefaults();

            _asteroidRb.AddForce(force, ForceMode2D.Impulse);
        }

        private void FixedUpdate()
        {
            LocalClientUpdate();
            ServerUpdate();
        }

        private void LocalClientUpdate()
        {
            if (isServer)
                return;

            transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
            float timestamp = Time.time;

            if (isPredictionEnabled)
            {
                _screenWrapper.CheckObjectOutOfScreen();
                Vector2 position = transform.position;

                _predictedPackages.Add(new PositionTimestampReceivePackage
                {
                    percentX = ExtensionFunctions.Map(position.x, _screenWrapper.LeftMostPoint,
                        _screenWrapper.RightMostPoint, -1, 1),
                    percentY = ExtensionFunctions.Map(position.y, _screenWrapper.TopMostPoint,
                        _screenWrapper.BottomMostPoint, 1, -1),

                    timestamp = timestamp
                });
            }
        }

        private void ServerUpdate()
        {
            if (!isServer)
                return;

            _screenWrapper.CheckObjectOutOfScreen();
            transform.Rotate(Vector3.forward * rotationSpeed * Time.fixedDeltaTime);

            Vector3 position = transform.position;

            float percentX = ExtensionFunctions.Map(position.x, _screenWrapper.LeftMostPoint,
                _screenWrapper.RightMostPoint, -1, 1);
            float percentY = ExtensionFunctions.Map(position.y, _screenWrapper.TopMostPoint,
                _screenWrapper.BottomMostPoint, 1, -1);

            _nextTick += Time.fixedDeltaTime;
            if (_nextTick / updateSendRate >= 1)
            {
                RpcLocalClientAsteroidPositionFixer(percentX, percentY, Time.time);
                _nextTick = 0;
            }
        }

        [ClientRpc]
        private void RpcLocalClientAsteroidPositionFixer(float percentX, float percentY, float timestamp)
        {
            if (isServer)
                return;

            Vector2 normalizedPosition = new Vector2(
                ExtensionFunctions.Map(percentX, -1, 1,
                    _screenWrapper.LeftMostPoint, _screenWrapper.RightMostPoint),
                ExtensionFunctions.Map(percentY, 1, -1,
                    _screenWrapper.TopMostPoint, _screenWrapper.BottomMostPoint)
            );

            if (isPredictionEnabled)
            {
                PositionTimestampReceivePackage predictedPackage =
                    _predictedPackages.LastOrDefault(_ => _.timestamp <= timestamp);
                if (predictedPackage == null)
                    return;

                Vector2 normalizedPredictedPosition = new Vector2(
                    ExtensionFunctions.Map(predictedPackage.percentX, -1, 1,
                        _screenWrapper.LeftMostPoint, _screenWrapper.RightMostPoint),
                    ExtensionFunctions.Map(predictedPackage.percentY, 1, -1,
                        _screenWrapper.TopMostPoint, _screenWrapper.BottomMostPoint)
                );

                if (Vector2.Distance(normalizedPosition, normalizedPredictedPosition) > 1.5f)
                    transform.position = normalizedPosition;

                _predictedPackages.RemoveAll(_ => _.timestamp <= timestamp);
            }
            else
                transform.position = normalizedPosition;
        }
    }
}