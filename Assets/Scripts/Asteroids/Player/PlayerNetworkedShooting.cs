using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.Networking;
using UNetUI.Asteroids.Enums;
using UNetUI.Asteroids.NetworkedData;
using UNetUI.Asteroids.Networking;
using UNetUI.Asteroids.Power_Ups;
using UNetUI.Asteroids.Shared;
using UNetUI.Extras;

namespace UNetUI.Asteroids.Player
{
    [RequireComponent(typeof(PlayerNetworkedPowerUpController))]
    public class PlayerNetworkedShooting : NetworkBehaviour
    {
        public GameObject bulletPrefab;
        [SerializeField] private float launchVelocity;

        private PlayerNetworkedPowerUpController _playerNetworkedPowerUpController;

        private void Start() => _playerNetworkedPowerUpController = GetComponent<PlayerNetworkedPowerUpController>();


        private void Update()
        {
            if (!Input.GetKeyDown(Controls.ShootKey))
                return;

            LocalClientUpdate();
        }

        private void LocalClientUpdate()
        {
            if (!isLocalPlayer || isServer)
                return;

            CmdServerUpdate(true);
        }

        [Command]
        private void CmdServerUpdate(bool keyPressed)
        {
            if (!isServer || !keyPressed)
                return;

            GameObject bulletInstance = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

            PowerUpAction powerUpAction = _playerNetworkedPowerUpController.GetPowerUp();
            if (powerUpAction)
                if (powerUpAction.powerUp.powerUpType == PowerUpType.Modifier && powerUpAction.IsPowerUpActive())
                    powerUpAction.ModifyGameObject(bulletInstance);

            bulletInstance.GetComponent<Rigidbody2D>().velocity = transform.up * launchVelocity;

            NetworkServer.Spawn(bulletInstance);
        }
    }
}