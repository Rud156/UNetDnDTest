using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UNetUI.Asteroids.NetworkedData.Common;
using UNetUI.Asteroids.Player;
using UNetUI.Asteroids.Shared;
using UNetUI.Extras;

namespace UNetUI.Asteroids.Power_Ups
{
    [RequireComponent(typeof(ScreenWrapper))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class PowerUpsController : NetworkBehaviour
    {
        public float launchVelocity = 3f;
        public PowerUpData powerUpData;

        [Header("Network Data")] public bool isPredictionEnabled = true;
        public float updateSendRate = 0.1f;

        private ScreenWrapper _screenWrapper;
        private Rigidbody2D _powerUpRb;

        private List<PositionTimestampReceivePackage> _predictedPackages;
        private bool _defaultSet;
        private float _nextTick;

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
            _powerUpRb.AddForce(launchVector, ForceMode2D.Impulse);
            
            RpcSendInitialPowerUpForce(launchVector);
        }

        private void SetDefaults()
        {
            if (_defaultSet)
                return;
            
            _predictedPackages = new List<PositionTimestampReceivePackage>();

            _screenWrapper = GetComponent<ScreenWrapper>();
            _powerUpRb = GetComponent<Rigidbody2D>();
            _defaultSet = true;
        }

        [ClientRpc]
        private void RpcSendInitialPowerUpForce(Vector2 force)
        {
            if(isServer)
                return;
            
            SetDefaults();
            _powerUpRb.AddForce(force, ForceMode2D.Impulse);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!isServer)
                return;

            if (!other.CompareTag(TagManager.Player))
                return;

            other.GetComponent<PlayerNetworkedPowerUpController>().CollectPowerUp(powerUpData);
            NetworkServer.Destroy(gameObject);
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

            Vector3 position = transform.position;
            float percentX = ExtensionFunctions.Map(position.x, _screenWrapper.LeftMostPoint,
                _screenWrapper.RightMostPoint, -1, 1);
            float percentY = ExtensionFunctions.Map(position.y, _screenWrapper.TopMostPoint,
                _screenWrapper.BottomMostPoint, 1, -1);

            _nextTick += Time.fixedDeltaTime;
            if (_nextTick / updateSendRate >= 1)
            {
                RpcRemoteClientPowerUpUpdate(percentX, percentY, Time.time);
                _nextTick = 0;
            }
        }

        [ClientRpc]
        private void RpcRemoteClientPowerUpUpdate(float percentX, float percentY, float timestamp)
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