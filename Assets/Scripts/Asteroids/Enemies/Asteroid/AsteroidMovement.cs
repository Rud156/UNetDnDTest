using UnityEngine;
using UnityEngine.Experimental.UIElements.StyleEnums;
using UnityEngine.Networking;
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

        private ScreenWrapper _screenWrapper;

        private void Start() => _screenWrapper = GetComponent<ScreenWrapper>();

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

            RpcSendPositionToClients(rotationZ, percentX, percentY);
        }

        [ClientRpc]
        private void RpcSendPositionToClients(float rotationZ, float percentX, float percentY)
        {
            if (isServer)
                return;

            transform.rotation = Quaternion.Euler(0, 0, rotationZ);

            transform.position = new Vector2(
                ExtensionFunctions.Map(percentX, -1, 1,
                    _screenWrapper.LeftMostPoint, _screenWrapper.RightMostPoint),
                ExtensionFunctions.Map(percentY, 1, -1,
                    _screenWrapper.TopMostPoint, _screenWrapper.BottomMostPoint)
            );
        }
    }
}