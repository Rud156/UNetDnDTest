using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.UIElements.StyleEnums;
using UnityEngine.Networking;
using UNetUI.Asteroids.NetworkedData;
using UNetUI.Asteroids.Networking;
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

        private Rigidbody2D _asteroidRb;
        private ScreenWrapper _screenWrapper;

        private void Start()
        {
            _screenWrapper = GetComponent<ScreenWrapper>();
            _asteroidRb = GetComponent<Rigidbody2D>();

            if (!isServer)
                return;

            int launchAngle = Random.Range(0, 360);
            Vector2 launchVector = new Vector2(
                Mathf.Cos(launchAngle * Mathf.Deg2Rad) * launchVelocity,
                Mathf.Sin(launchAngle * Mathf.Deg2Rad) * launchVelocity
            );
            _asteroidRb.AddForce(launchVector, ForceMode2D.Impulse);
        }

        private void Update() => ServerUpdate();

        private void ServerUpdate()
        {
            if (!isServer)
                return;

            _screenWrapper.CheckObjectOutOfScreen();
            transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);

            float rotationZ = transform.rotation.eulerAngles.z;
            Vector3 position = transform.position;

            float percentX = ExtensionFunctions.Map(position.x, _screenWrapper.LeftMostPoint,
                _screenWrapper.RightMostPoint, -1, 1);
            float percentY = ExtensionFunctions.Map(position.y, _screenWrapper.TopMostPoint,
                _screenWrapper.BottomMostPoint, 1, -1);

            RpcRemoteClientAsteroidUpdate(percentX, percentY, rotationZ);
        }

        [ClientRpc]
        private void RpcRemoteClientAsteroidUpdate(float percentX, float percentY, float rotationZ)
        {
            if (isServer)
                return;

            Vector2 normalizedPosition = new Vector2(
                ExtensionFunctions.Map(percentX, -1, 1,
                    _screenWrapper.LeftMostPoint, _screenWrapper.RightMostPoint),
                ExtensionFunctions.Map(percentY, 1, -1,
                    _screenWrapper.TopMostPoint, _screenWrapper.BottomMostPoint)
            );

            transform.position = normalizedPosition;
            transform.rotation = Quaternion.Euler(Vector3.forward * rotationZ);
        }
    }
}