using System.Collections;
using System.Collections.Generic;
using EZCameraShake;
using UnityEngine;
using UnityEngine.Networking;
using UNetUI.Asteroids.Scene.MainScene;
using UNetUI.Asteroids.Shared;
using UNetUI.Asteroids.UI;

namespace UNetUI.Asteroids.Player
{
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(PlayerNetworkedController))]
    [RequireComponent(typeof(PlayerNetworkedShooting))]
    [RequireComponent(typeof(PlayerNetworkedPowerUpController))]
    public class PlayerNetworkedDestroy : NetworkBehaviour
    {
        [Header("Camera Shaker")] [SerializeField]
        private CameraShakerData _shakerData;

        private Collider2D _collider;
        private Animator _playerAnim;

        private PlayerNetworkedController _playerNetworkedController;
        private PlayerNetworkedShooting _playerNetworkedShooting;
        private PlayerNetworkedPowerUpController _playerNetworkedPowerUp;

        private void Start()
        {
            _collider = GetComponent<Collider2D>();
            _playerAnim = GetComponent<Animator>();

            _playerNetworkedController = GetComponent<PlayerNetworkedController>();
            _playerNetworkedShooting = GetComponent<PlayerNetworkedShooting>();
            _playerNetworkedPowerUp = GetComponent<PlayerNetworkedPowerUpController>();
        }

        public void DestroyPlayer()
        {
            if (!isServer)
                return;

            RpcShakeCameraOnClients();
            DisableControlsAndPlayAnimation();
            Invoke(nameof(RemovePlayer), 0.8f);
        }

        private void RemovePlayer()
        {
            InfoTextManager.instance.RpcDisplayText("You Died", Color.red, true);
            ServerClientMenuManager.instance.DelayedExitGame(3.5f);
            NetworkServer.Destroy(gameObject);
        }

        private void DisableControlsAndPlayAnimation()
        {
            if (!isServer)
                return;

            _collider.enabled = false;
            _playerNetworkedController.enabled = false;
            _playerNetworkedShooting.enabled = false;
            _playerNetworkedPowerUp.enabled = false;

            _playerAnim.SetBool(PlayerConstantData.DeathAnimParam, true);

            RpcDisableCollidersAndPlayerAnimation();
        }

        [ClientRpc]
        private void RpcShakeCameraOnClients()
        {
            if (isServer)
                return;

            CameraShaker.Instance.ShakeOnce(
                _shakerData.magnitude,
                _shakerData.roughness,
                _shakerData.fadeInTime,
                _shakerData.fadeOutTime
            );
        }

        [ClientRpc]
        private void RpcDisableCollidersAndPlayerAnimation()
        {
            if (isServer)
                return;

            _collider.enabled = false;
            _playerNetworkedController.enabled = false;
            _playerNetworkedShooting.enabled = false;
            _playerNetworkedPowerUp.enabled = false;


            _playerAnim.SetBool(PlayerConstantData.DeathAnimParam, true);
        }
    }
}