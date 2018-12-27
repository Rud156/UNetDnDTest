using UnityEngine;
using UnityEngine.Networking;
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

        private float _nextTick;
        private float _currentRate;

        private ScreenWrapper _screenWrapper;

        private void Start()
        {
            _screenWrapper = GetComponent<ScreenWrapper>();
            _currentRate = useConstantRate ? fireRate : Random.Range(minFireRate, maxFireRate);
        }

        private void Update() => ServerUpdate();

        private void ServerUpdate()
        {
            _nextTick += Time.deltaTime;
            if (_nextTick / _currentRate >= 1)
            {
                ShootAtPlayer();

                _nextTick = 0;
                _currentRate = useConstantRate ? fireRate : Random.Range(minFireRate, maxFireRate);
            }

            if (isServer)
                return;

            _screenWrapper.CheckObjectOutOfScreen();

            Vector3 position = transform.position;

            float percentX = ExtensionFunctions.Map(position.x, _screenWrapper.LeftMostPoint,
                _screenWrapper.RightMostPoint, -1, 1);
            float percentY = ExtensionFunctions.Map(position.y, _screenWrapper.TopMostPoint,
                _screenWrapper.BottomMostPoint, 1, -1);

            RpcSendPositionsToClients(percentX, percentY);
        }

        [ClientRpc]
        private void RpcSendPositionsToClients(float percentX, float percentY)
        {
            if (isServer)
                return;

            transform.position = new Vector2(
                ExtensionFunctions.Map(percentX, -1, 1,
                    _screenWrapper.LeftMostPoint, _screenWrapper.RightMostPoint),
                ExtensionFunctions.Map(percentY, 1, -1,
                    _screenWrapper.TopMostPoint, _screenWrapper.BottomMostPoint)
            );
        }

        private void ShootAtPlayer()
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag(TagManager.Player);
            if (players.Length <= 0)
                return;

            int randomPlayerIndex = Random.Range(0, 1000) % players.Length;
            Transform randomPlayer = players[randomPlayerIndex]?.transform;

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