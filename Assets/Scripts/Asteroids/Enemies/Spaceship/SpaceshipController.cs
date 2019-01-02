using System.Linq;
using Boo.Lang;
using UnityEngine;
using UnityEngine.Networking;
using UNetUI.Asteroids.NetworkedData.Common;
using UNetUI.Asteroids.Shared;
using UNetUI.Extras;

namespace UNetUI.Asteroids.Enemies.Spaceship
{
    [RequireComponent(typeof(ScreenWrapper))]
    public class SpaceshipController : NetworkBehaviour
    {
        [Header("Shooting Types")] public bool useConstantRate;
        public float fireRate;
        public float minFireRate;
        public float maxFireRate;

        [Header("Shoot Points")] public Transform shootPoint;
        public GameObject bullet;
        public float launchSpeed;
        public Vector3 launchAngleOffset;

        [Header("Movement")] public float movementLerpHeight;
        public float movementSwitchTime;
        public float initialLaunchVelocity;

        [Header("Network Data")] public bool isPredictionEnabled = false;
        public float updateSendRate = 0.1f;

        private float _nextShootTick;
        private float _currentShootRate;

        private float _nextMovementTick;
        private float _movementLerpYPosition;

        private ScreenWrapper _screenWrapper;
        private Transform _playerHolder;
        private Rigidbody2D _spaceshipRb;

        private List<PositionTimestampReceivePackage> _predictedPackages;
        private bool _defaultsSet;
        private float _nextTick;

        private void Start()
        {
            _movementLerpYPosition = transform.position.y;
            SetDefaults();

            if (isServer)
            {
                Vector2 launchVector;

                if (Random.value > 0.5f)
                    launchVector = Vector2.right * initialLaunchVelocity;
                else
                    launchVector = Vector2.left * initialLaunchVelocity;

                _spaceshipRb.AddForce(launchVector, ForceMode2D.Impulse);
                RpcSendInitialSpaceshipForceToClients(launchVector);
            }
        }

        private void SetDefaults()
        {
            if (_defaultsSet)
                return;

            _screenWrapper = GetComponent<ScreenWrapper>();
            _currentShootRate = useConstantRate ? fireRate : Random.Range(minFireRate, maxFireRate);
            _playerHolder = GameObject.FindGameObjectWithTag(TagManager.PlayerHolder)?.transform;
            _spaceshipRb = GetComponent<Rigidbody2D>();
            
            _predictedPackages = new List<PositionTimestampReceivePackage>();

            _defaultsSet = true;
        }

        [ClientRpc]
        private void RpcSendInitialSpaceshipForceToClients(Vector2 force)
        {
            if (isServer)
                return;

            SetDefaults();
            _spaceshipRb.AddForce(force, ForceMode2D.Impulse);
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
                MoveSpaceship();
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

            _nextShootTick += Time.fixedDeltaTime;
            if (_nextShootTick / _currentShootRate >= 1)
            {
                ShootAtPlayer();

                _nextShootTick = 0;
                _currentShootRate = useConstantRate ? fireRate : Random.Range(minFireRate, maxFireRate);
            }

            MoveSpaceship();
            _screenWrapper.CheckObjectOutOfScreen();

            Vector3 position = transform.position;

            float percentX = ExtensionFunctions.Map(position.x, _screenWrapper.LeftMostPoint,
                _screenWrapper.RightMostPoint, -1, 1);
            float percentY = ExtensionFunctions.Map(position.y, _screenWrapper.TopMostPoint,
                _screenWrapper.BottomMostPoint, 1, -1);

            _nextTick += Time.fixedDeltaTime;
            if (_nextTick / updateSendRate >= 1)
            {
                RpcLocalClientSpaceshipPositionFixer(percentX, percentY, _spaceshipRb.velocity, Time.time);
                _nextTick = 0;
            }
        }

        [ClientRpc]
        private void RpcLocalClientSpaceshipPositionFixer(float percentX, float percentY, Vector2 velocity, float timestamp)
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
                {
                    transform.position = normalizedPosition;
                    _spaceshipRb.velocity = velocity;
                }

                _predictedPackages.RemoveAll(_ => _.timestamp <= timestamp);
            }
            else
                transform.position = normalizedPosition;
        }

        [ClientRpc]
        private void RpcSendMovementYToClients(float movementYPosition)
        {
            if (isServer)
                return;

            _movementLerpYPosition = movementYPosition;
        }

        private void MoveSpaceship()
        {
            _nextMovementTick += Time.fixedDeltaTime;
            if (_nextMovementTick / movementSwitchTime >= 1)
            {
                _movementLerpYPosition =
                    ExtensionFunctions.Map(Random.value, 0, 1,
                        -movementLerpHeight, movementLerpHeight);

                RpcSendMovementYToClients(_movementLerpYPosition);

                _nextMovementTick = 0;
            }

            Vector2 currentPosition = transform.position;
            transform.position = Vector2.Lerp(
                new Vector2(currentPosition.x, currentPosition.y),
                new Vector2(currentPosition.x, _movementLerpYPosition),
                0.7f * Time.fixedDeltaTime
            );
        }

        private void ShootAtPlayer()
        {
            if (_playerHolder.childCount <= 0)
                return;

            int randomPlayerIndex = Random.Range(0, 1000) % _playerHolder.childCount;
            Transform randomPlayer = _playerHolder.GetChild(randomPlayerIndex);

            Vector2 playerDirection = (randomPlayer.position + new Vector3(
                                           Random.Range(-launchAngleOffset.x, launchAngleOffset.x),
                                           Random.Range(-launchAngleOffset.y, launchAngleOffset.y)
                                       )) - shootPoint.position;
            Quaternion lookRotation = Quaternion.LookRotation(playerDirection);

            shootPoint.rotation = lookRotation;
            GameObject bulletInstance = Instantiate(bullet, shootPoint.position, Quaternion.identity);
            bulletInstance.GetComponent<Rigidbody2D>().velocity = shootPoint.forward * launchSpeed;

            NetworkServer.Spawn(bulletInstance);
        }
    }
}