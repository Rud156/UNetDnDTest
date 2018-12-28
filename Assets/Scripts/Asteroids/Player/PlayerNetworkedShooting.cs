using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.Networking;
using UNetUI.Asteroids.NetworkedData;
using UNetUI.Asteroids.Networking;
using UNetUI.Extras;

namespace UNetUI.Asteroids.Player
{
    public class PlayerNetworkedShooting : NetworkBehaviour
    {
        public GameObject bulletPrefab;
        [SerializeField] private float launchVelocity;

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
            bulletInstance.GetComponent<Rigidbody2D>().velocity = transform.up * launchVelocity;

            NetworkServer.Spawn(bulletInstance);
        }
    }
}