using System.Collections.Generic;
using System.Linq;
using EZCameraShake;
using UnityEngine;
using UnityEngine.Experimental.UIElements.StyleEnums;
using UnityEngine.Networking;
using UNetUI.Asteroids.Enums;
using UNetUI.Asteroids.NetworkedData.Player;
using UNetUI.Asteroids.Networking;
using UNetUI.Asteroids.Power_Ups;
using UNetUI.Asteroids.Scene.MainScene;
using UNetUI.Asteroids.Shared;
using UNetUI.Extras;

namespace UNetUI.Asteroids.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(ScreenWrapper))]
    [RequireComponent(typeof(PlayerNetworkedPowerUpController))]
    [RequireComponent(typeof(PlayerNetworkedDestroy))]
    public class PlayerNetworkedController : PlayerNetworkPacketController
    {
        [Header("Velocities")] [SerializeField]
        private float movementSpeed;

        [SerializeField] private float rotationSpeed;
        [SerializeField] private bool isPredictionEnabled;

        [Header("Networking")] [SerializeField] [Range(0.1f, 1)]
        private float networkSendRate = 0.2f;

        [SerializeField] private float positionCorrectionThreshold;
        [SerializeField] private float rotationCorrectionThreshold;

        private Rigidbody2D _playerRb;
        private Animator _playerAnim;
        private ScreenWrapper _screenWrapper;
        private PlayerNetworkedPowerUpController _playerNetworkedPowerUpController;
        private PlayerNetworkedDestroy _playerNetworkedDestroy;

        private float _roll;

        private List<PositionReceivePackage> _predictedPackages;
        private Vector3 _lastPosition;
        private Vector3 _lastRotation;

        private void Start()
        {
            _playerRb = GetComponent<Rigidbody2D>();
            _playerAnim = GetComponent<Animator>();
            _screenWrapper = GetComponent<ScreenWrapper>();

            _playerNetworkedPowerUpController = GetComponent<PlayerNetworkedPowerUpController>();
            _playerNetworkedDestroy = GetComponent<PlayerNetworkedDestroy>();

            _roll = transform.rotation.eulerAngles.z;

            PacketManager.SendSpeed = networkSendRate;
            ServerPacketManager.SendSpeed = networkSendRate;

            _predictedPackages = new List<PositionReceivePackage>();

            if (isLocalPlayer && isServer)
                NetworkServer.Destroy(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!isServer)
                return;

            if (other.gameObject.layer != 9)
                return;

            PowerUpAction powerUpAction = _playerNetworkedPowerUpController.GetPowerUp();
            if (powerUpAction != null)
            {
                if (powerUpAction.powerUp.powerUpType == PowerUpType.Defence && powerUpAction.IsPowerUpActive())
                    return;
            }

            _playerNetworkedDestroy.DestroyPlayer();
        }

        private void FixedUpdate()
        {
            base.SendDataUpdates();

            LocalClientUpdate();
            ServerUpdate();
            RemoteClientUpdate();
        }


        private void LocalClientUpdate()
        {
            if (!isLocalPlayer || isServer)
                return;

            float moveZ = Input.GetAxis(Controls.PlayerVM);
            float moveX = Input.GetAxis(Controls.PlayerHM);
            float timestamp = Time.time;

            if (isPredictionEnabled)
            {
                RotatePlayer(moveX);
                MovePlayerForward(moveZ);
                SetMovementAnimation(moveZ);
                _screenWrapper.CheckObjectOutOfScreen();

                Vector3 position = transform.position;
                Vector3 rotation = transform.rotation.eulerAngles;
                Vector3 velocity = _playerRb.velocity;

                _predictedPackages.Add(new PositionReceivePackage
                {
                    percentX = ExtensionFunctions.Map(position.x, _screenWrapper.LeftMostPoint,
                        _screenWrapper.RightMostPoint, -1, 1),
                    percentY = ExtensionFunctions.Map(position.y, _screenWrapper.TopMostPoint,
                        _screenWrapper.BottomMostPoint, 1, -1),

                    velocityX = velocity.x,
                    velocityY = velocity.y,

                    rotationZ = rotation.z,
                    roll = _roll,

                    movementAnimation = moveZ > 0,

                    timestamp = timestamp,
                });
            }

            PacketManager.AddPackage(new InputSendPackage
            {
                horizontal = moveX,
                vertical = moveZ,

                timestamp = timestamp
            });
        }

        private void ServerUpdate()
        {
            // isLocalPlayer added to make sure that the server is just used for computation
            // and not for anything else. Its a dumb dedicated sever 
            if (!isServer || isLocalPlayer)
                return;

            InputSendPackage inputSendPackageData = PacketManager.GetNextDataReceived();

            if (inputSendPackageData == null)
                return;

            RotatePlayer(inputSendPackageData.horizontal);
            MovePlayerForward(inputSendPackageData.vertical);
            SetMovementAnimation(inputSendPackageData.vertical);
            _screenWrapper.CheckObjectOutOfScreen();

            if (_lastPosition == transform.position && _lastRotation == transform.rotation.eulerAngles)
                return;

            Vector3 position = transform.position;
            Vector3 rotation = transform.rotation.eulerAngles;
            Vector3 velocity = _playerRb.velocity;

            ServerPacketManager.AddPackage(new PositionReceivePackage
            {
                percentX = ExtensionFunctions.Map(position.x, _screenWrapper.LeftMostPoint,
                    _screenWrapper.RightMostPoint, -1, 1),
                percentY = ExtensionFunctions.Map(position.y, _screenWrapper.TopMostPoint,
                    _screenWrapper.BottomMostPoint, 1, -1),

                velocityX = velocity.x,
                velocityY = velocity.y,

                rotationZ = rotation.z,
                roll = _roll,

                movementAnimation = inputSendPackageData.vertical > 0,

                timestamp = inputSendPackageData.timestamp
            });

            _lastPosition = position;
            _lastRotation = rotation;
        }

        private void RemoteClientUpdate()
        {
            if (isServer)
                return;

            PositionReceivePackage data = ServerPacketManager.GetNextDataReceived();
            if (data == null)
                return;

            Vector2 normalizedPosition = new Vector2(
                ExtensionFunctions.Map(data.percentX, -1, 1,
                    _screenWrapper.LeftMostPoint, _screenWrapper.RightMostPoint),
                ExtensionFunctions.Map(data.percentY, 1, -1,
                    _screenWrapper.TopMostPoint, _screenWrapper.BottomMostPoint)
            );

            if (isLocalPlayer && isPredictionEnabled)
            {
                PositionReceivePackage predictedPackage =
                    _predictedPackages.FirstOrDefault(_ => _.timestamp == data.timestamp);
                if (predictedPackage == null)
                    return;

                Vector2 normalizedPredictedPosition = new Vector2(
                    ExtensionFunctions.Map(predictedPackage.percentX, -1, 1,
                        _screenWrapper.LeftMostPoint, _screenWrapper.RightMostPoint),
                    ExtensionFunctions.Map(predictedPackage.percentY, 1, -1,
                        _screenWrapper.TopMostPoint, _screenWrapper.BottomMostPoint)
                );

                if (Vector2.Distance(normalizedPredictedPosition, normalizedPosition) > positionCorrectionThreshold)
                {
                    _playerRb.velocity = new Vector2(data.velocityX, data.velocityY);
                    transform.position = normalizedPosition;
                    SetMovementAnimation(data.movementAnimation);
                }


                if (Mathf.Abs(ExtensionFunctions.To360Angle(data.rotationZ) -
                              ExtensionFunctions.To360Angle(predictedPackage.rotationZ)) >
                    rotationCorrectionThreshold)
                {
                    _roll = data.roll;
                    transform.rotation = Quaternion.Euler(Vector3.forward * data.rotationZ);
                }

                _predictedPackages.RemoveAll(_ => _.timestamp <= data.timestamp);
            }
            else
            {
                transform.position = normalizedPosition;
                _playerRb.velocity = new Vector2(data.velocityX, data.velocityY);
                SetMovementAnimation(data.movementAnimation);

                _roll = data.roll;
                transform.rotation = Quaternion.Euler(Vector3.forward * data.rotationZ);
            }
        }

        #region Calculator

        private void RotatePlayer(float moveH)
        {
            _roll += -moveH * rotationSpeed * Time.fixedDeltaTime;
            transform.rotation = Quaternion.Euler(Vector3.forward * _roll);
        }

        private void MovePlayerForward(float moveZ)
        {
            if (moveZ <= 0)
                return;

            Vector2 velocityV = transform.up * moveZ * movementSpeed * Time.fixedDeltaTime;
            _playerRb.AddForce(velocityV, ForceMode2D.Force);
        }

        private void SetMovementAnimation(float moveZ) =>
            _playerAnim.SetBool(PlayerConstantData.MovementAnimParam, moveZ > 0);

        private void SetMovementAnimation(bool movementAnimation) =>
            _playerAnim.SetBool(PlayerConstantData.MovementAnimParam, movementAnimation);

        #endregion Calculator
    }
}