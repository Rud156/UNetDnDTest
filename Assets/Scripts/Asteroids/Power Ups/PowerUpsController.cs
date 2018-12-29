using UnityEngine;
using UnityEngine.Networking;
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

        private ScreenWrapper _screenWrapper;
        private Rigidbody2D _powerUpRb;

        private void Start()
        {
            _screenWrapper = GetComponent<ScreenWrapper>();
            _powerUpRb = GetComponent<Rigidbody2D>();

            if (!isServer)
                return;

            int launchAngle = Random.Range(0, 360);
            Vector2 launchVector = new Vector2(
                Mathf.Cos(launchAngle * Mathf.Deg2Rad) * launchVelocity,
                Mathf.Sin(launchAngle * Mathf.Deg2Rad) * launchVelocity
            );
            _powerUpRb.AddForce(launchVector, ForceMode2D.Impulse);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!isServer)
                return;

            if (other.CompareTag(TagManager.Player))
                other.GetComponent<PlayerPowerUpController>().CollectPowerUp(powerUpData);
        }

        private void Update()
        {
            if (!isServer)
                return;

            _screenWrapper.CheckObjectOutOfScreen();

            Vector3 position = transform.position;
            float percentX = ExtensionFunctions.Map(position.x, _screenWrapper.LeftMostPoint,
                _screenWrapper.RightMostPoint, -1, 1);
            float percentY = ExtensionFunctions.Map(position.y, _screenWrapper.TopMostPoint,
                _screenWrapper.BottomMostPoint, 1, -1);

            RpcRemoteClientPowerUpUpdate(percentX, percentY);
        }

        [ClientRpc]
        private void RpcRemoteClientPowerUpUpdate(float percentX, float percentY)
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
        }
    }
}