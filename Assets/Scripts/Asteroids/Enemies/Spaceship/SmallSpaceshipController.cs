using UnityEngine;
using UnityEngine.Networking;
using UNetUI.Extras;

namespace UNetUI.Asteroids.Enemies.Spaceship
{
    public class SmallSpaceshipController : NetworkBehaviour
    {
        public float fireRate;
        public Transform shootPoint;
        public GameObject bullet;
        public float launchSpeed;
        public Vector3 launchAngleOffset;

        private float _nextTick;

        private void Update() => ServerUpdate();

        private void ServerUpdate()
        {
            _nextTick += Time.deltaTime;
            if (_nextTick / fireRate >= 1)
            {
                ShootAtPlayer();
                _nextTick = 0;
            }
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