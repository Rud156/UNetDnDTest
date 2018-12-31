using System.Linq;
using Boo.Lang;
using UnityEngine;
using UnityEngine.Networking;
using UNetUI.Asteroids.NetworkedData.Common;
using UNetUI.Asteroids.Shared;
using UNetUI.Extras;

namespace UNetUI.Asteroids.Shared
{
    [RequireComponent(typeof(ScreenWrapper))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class BulletController : NetworkBehaviour
    {
        [Header("Network Data")] public bool isPredictionEnabled = true;
        public float updateSendRate = 0.1f;

        private ScreenWrapper _screenWrapper;
        private Rigidbody2D _bulletRb;

        private bool _defaultsSet;
        private float _nextTick;

        private List<PositionTimestampReceivePackage> _predictedPackages;

        private void Start()
        {
            SetDefaults();

            if (isServer)
                RpcSendInitialVelocityToClients(_bulletRb.velocity);
        }

        private void SetDefaults()
        {
            if (_defaultsSet)
                return;

            _predictedPackages = new List<PositionTimestampReceivePackage>();

            _screenWrapper = GetComponent<ScreenWrapper>();
            _bulletRb = GetComponent<Rigidbody2D>();
            _defaultsSet = true;
        }

        [ClientRpc]
        private void RpcSendInitialVelocityToClients(Vector2 launchVelocity)
        {
            if (isServer)
                return;

            SetDefaults();
            _bulletRb.velocity = launchVelocity;
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

            _nextTick += Time.deltaTime;
            if (_nextTick / updateSendRate >= 1)
            {
                RpcLocalClientBulletPositionFixer(percentX, percentY, Time.time);
                _nextTick = 0;
            }
        }

        [ClientRpc]
        private void RpcLocalClientBulletPositionFixer(float percentX, float percentY, float timestamp)
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