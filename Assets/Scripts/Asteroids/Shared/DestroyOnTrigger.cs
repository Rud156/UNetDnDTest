using EZCameraShake;
using UnityEngine;
using UnityEngine.Networking;

namespace UNetUI.Asteroids.Shared
{
    public class DestroyOnTrigger : NetworkBehaviour
    {
        public GameObject destroyEffect;
        public CameraShakerData shakerData;
        public bool shakeCameraOnDestroy;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!isServer)
                return;

            GameObject destroyEffectInstance = Instantiate(destroyEffect, transform.position, Quaternion.identity);

            if (shakeCameraOnDestroy)
                RpcShakeClientsOnDestroy();
            NetworkServer.Spawn(destroyEffectInstance);
        }

        [ClientRpc]
        private void RpcShakeClientsOnDestroy()
        {
            if (isServer)
                return;

            CameraShaker.Instance.ShakeOnce(
                shakerData.magnitude,
                shakerData.roughness,
                shakerData.fadeInTime,
                shakerData.fadeOutTime
            );
        }
    }
}