using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UNetUI.Asteroids.NetworkedData;
using UNetUI.Asteroids.Networking;
using UNetUI.Extras;

namespace UNetUI.Asteroids.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Animator))]
    public class PlayerController : NetworkPacketController
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
        private float _roll;

        private List<PositionReceivePackage> _predictedPackages;
        private Vector3 _lastPosition;
        private Vector3 _lastRotation;

        private void Start()
        {
            _playerRb = GetComponent<Rigidbody2D>();
            _playerAnim = GetComponent<Animator>();

            _roll = transform.rotation.eulerAngles.z;

            PacketManager.SendSpeed = networkSendRate;
            ServerPacketManager.SendSpeed = networkSendRate;

            _predictedPackages = new List<PositionReceivePackage>();
        }

        private void Update()
        {
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

                Vector3 position = transform.position;
                Vector3 rotation = transform.rotation.eulerAngles;

                _predictedPackages.Add(new PositionReceivePackage
                {
                    positionX = position.x,
                    positionY = position.y,
                    positionZ = position.z,

                    rotationZ = rotation.z,

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

            if (_lastPosition == transform.position && _lastRotation == transform.rotation.eulerAngles)
                return;

            Vector3 position = transform.position;
            Vector3 rotation = transform.rotation.eulerAngles;

            ServerPacketManager.AddPackage(new PositionReceivePackage
            {
                positionX = position.x,
                positionY = position.y,
                positionZ = position.z,

                rotationZ = rotation.z,
                roll = _roll,

                timestamp = inputSendPackageData.timestamp
            });

            _lastPosition = position;
            _lastRotation = rotation;
        }

        private void RemoteClientUpdate()
        {
            if (!isLocalPlayer || isServer)
                return;

            PositionReceivePackage data = ServerPacketManager.GetNextDataReceived();
            if (data == null)
                return;

            if (isLocalPlayer && isPredictionEnabled)
            {
                PositionReceivePackage transmittedPackage =
                    _predictedPackages.FirstOrDefault(_ => _.timestamp == data.timestamp);
                if (transmittedPackage == null)
                    return;

                if (Vector3.Distance(
                        new Vector3(transmittedPackage.positionX, transmittedPackage.positionY,
                            transmittedPackage.positionZ),
                        new Vector3(data.positionX, data.positionY, data.positionZ)) > positionCorrectionThreshold)
                    transform.position = new Vector3(data.positionX, data.positionY, data.positionZ);


                if (Mathf.Abs(data.rotationZ - transmittedPackage.rotationZ) > rotationCorrectionThreshold)
                {
                    _roll = data.roll;
                    transform.rotation = Quaternion.Euler(Vector3.forward * data.rotationZ);
                }

                _predictedPackages.RemoveAll(_ => _.timestamp <= data.timestamp);
            }
            else
            {
                transform.position = new Vector3(data.positionX, data.positionY, data.positionZ);
                _roll = data.roll;
                transform.rotation = Quaternion.Euler(Vector3.forward * data.rotationZ);
            }
        }

        #region Calculator

        private void RotatePlayer(float moveH)
        {
            _roll += -moveH * rotationSpeed * Time.deltaTime;
            transform.rotation = Quaternion.Euler(Vector3.forward * _roll);
        }

        private void MovePlayerForward(float moveZ)
        {
            if (moveZ > 0)
            {
                Vector3 velocityV = transform.up * moveZ * movementSpeed * Time.deltaTime;
                _playerRb.AddForce(velocityV);

                _playerAnim.SetBool(PlayerConstantData.MovementAnimParam, true);
            }
            else
                _playerAnim.SetBool(PlayerConstantData.MovementAnimParam, false);
        }

        #endregion Calculator
    }
}