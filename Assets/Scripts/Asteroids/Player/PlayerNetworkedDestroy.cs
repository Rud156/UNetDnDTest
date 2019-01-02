using System.Collections;
using System.Collections.Generic;
using EZCameraShake;
using UnityEngine;
using UnityEngine.Networking;
using UNetUI.Asteroids.Networking;
using UNetUI.Asteroids.Scene.MainScene;
using UNetUI.Asteroids.Shared;
using UNetUI.Asteroids.UI;
using UNetUI.Extras;

namespace UNetUI.Asteroids.Player
{
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(PlayerNetworkedController))]
    [RequireComponent(typeof(PlayerNetworkedShooting))]
    [RequireComponent(typeof(PlayerNetworkedPowerUpController))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class PlayerNetworkedDestroy : NetworkBehaviour
    {
        [Header("Camera Shaker")] [SerializeField]
        private CameraShakerData _shakerData;

        private Collider2D _collider;
        private Animator _playerAnim;
        private SpriteRenderer _renderer;

        private PlayerNetworkedController _playerNetworkedController;
        private PlayerNetworkedShooting _playerNetworkedShooting;
        private PlayerNetworkedPowerUpController _playerNetworkedPowerUp;

        private void Start()
        {
            _collider = GetComponent<Collider2D>();
            _playerAnim = GetComponent<Animator>();
            _renderer = GetComponent<SpriteRenderer>();

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
            StartCoroutine(RemovePlayer());
        }

        // Called From External Animation Event
        public void DisableRenderer() => _renderer.enabled = false;

        private IEnumerator RemovePlayer()
        {
            RpcDisplayDeadText();
            ScreenFlasher.instance.RpcSetColorAndFlash(Color.red);

            yield return new WaitForSeconds(2.75F);
            RpcSwitchClientOnDestroy();
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
        private void RpcDisplayDeadText() =>
            InfoTextManager.instance.DisplayText(!isLocalPlayer ? "A Player Died !!!" : "You Died !!!", Color.red);

        [ClientRpc]
        private void RpcSwitchClientOnDestroy()
        {
            if (!isLocalPlayer || isServer)
                return;

            ServerClientMenuManager.instance.ExitGame();
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