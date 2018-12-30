using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UNetUI.Asteroids.Enums;
using UNetUI.Asteroids.Power_Ups;
using UNetUI.Asteroids.Shared;
using UNetUI.Extras;
using Random = UnityEngine.Random;

namespace UNetUI.Asteroids.Player
{
    [RequireComponent(typeof(PlayerNetworkedShooting))]
    public class PlayerNetworkedPowerUpController : NetworkBehaviour
    {
        private PowerUpData _collectedPowerUp;
        private Transform _asteroidHolder;
        private Transform _spaceshipHolder;

        private Image _powerUpDisplay;
        private PlayerNetworkedShooting _playerNetworkedShooting;

        private bool _isShieldActive;
        private bool _isBulletEnhanced;
        private Coroutine _shieldCoroutine;
        private Coroutine _bulletCoroutine;

        private GameObject _shieldInstance;
        private float _bulletDamageIncrease;

        private void Start()
        {
            _asteroidHolder = GameObject.FindGameObjectWithTag(TagManager.AsteroidsHolder)?.transform;
            _spaceshipHolder = GameObject.FindGameObjectWithTag(TagManager.SpaceshipHolder)?.transform;

            _powerUpDisplay = GameObject.FindGameObjectWithTag(TagManager.PowerUpDisplay)?.GetComponent<Image>();
            _powerUpDisplay.enabled = false;

            _playerNetworkedShooting = GetComponent<PlayerNetworkedShooting>();
            _playerNetworkedShooting.bulletModifier += ModifyBulletDamage;
        }

        private void Update()
        {
            if (!Input.GetKeyDown(Controls.PowerUpUseKey))
                return;

            Debug.Log(_collectedPowerUp.powerUpName);

            if (!_collectedPowerUp)
                return;

            CmdUsePowerUp();
        }

        [Command]
        private void CmdUsePowerUp()
        {
            Debug.Log(_collectedPowerUp.powerUpName);

            if (!_collectedPowerUp)
                return;

            switch (_collectedPowerUp.powerUpType)
            {
                case PowerUpType.Shield:
                    CreateShieldAroundPlayer();
                    break;

                case PowerUpType.BulletDamageIncrease:
                    IncreaseBulletDamage();
                    break;

                case PowerUpType.EnemyDestroy:
                    DestroyRandomEnemy();
                    break;
            }

            CollectPowerUp(null);
        }

        #region Shield

        public bool IsShieldActive() => _isShieldActive;

        private void CreateShieldAroundPlayer()
        {
            ResetShieldDefence();

            GameObject shieldInstance =
                Instantiate(_collectedPowerUp.powerUpPrefab, transform.position, Quaternion.identity);
            shieldInstance.transform.SetParent(transform);
            shieldInstance.transform.localPosition = Vector3.zero;

            _shieldInstance = shieldInstance;

            NetworkServer.Spawn(shieldInstance);
            RpcSetShieldOnClients(shieldInstance, gameObject);
            _shieldCoroutine = StartCoroutine(DestroyShield(shieldInstance));
        }

        private void ResetShieldDefence()
        {
            if (_shieldCoroutine != null)
                StopCoroutine(_shieldCoroutine);
            _isShieldActive = false;
            if (_shieldInstance != null)
                NetworkServer.Destroy(_shieldInstance);
        }

        private IEnumerator DestroyShield(GameObject shieldInstance)
        {
            _isShieldActive = true;

            yield return new WaitForSeconds(_collectedPowerUp.powerUpAffectTime);
            NetworkServer.Destroy(shieldInstance);

            _isShieldActive = false;
        }

        [ClientRpc]
        private void RpcSetShieldOnClients(GameObject shieldInstance, GameObject player)
        {
            if (isServer)
                return;

            shieldInstance.transform.SetParent(player.transform);
            shieldInstance.transform.localPosition = Vector3.zero;
        }

        #endregion Shield

        #region Bullet

        private void ModifyBulletDamage(GameObject bulletInstance)
        {
            if (_isBulletEnhanced)
                bulletInstance.GetComponent<DamageSetter>().damageAmount += _bulletDamageIncrease;
        }

        private void IncreaseBulletDamage()
        {
            if (_bulletCoroutine != null)
                StopCoroutine(_bulletCoroutine);
            _isBulletEnhanced = false;

            _bulletDamageIncrease = _collectedPowerUp.damageAmount;
            _bulletCoroutine = StartCoroutine(RemoveBulletEnhancement());
        }

        private IEnumerator RemoveBulletEnhancement()
        {
            _isBulletEnhanced = true;
            yield return new WaitForSeconds(_collectedPowerUp.powerUpAffectTime);
            _isBulletEnhanced = false;
        }

        #endregion Bullet

        #region DestroyEnemy

        private void DestroyRandomEnemy()
        {
            int asteroidChildCount = _asteroidHolder.childCount;
            if (asteroidChildCount > 0)
            {
                int randomIndex = Random.Range(0, 1000) % asteroidChildCount;
                HealthSetter asteroidHealthSetter =
                    _asteroidHolder.GetChild(randomIndex).GetComponent<HealthSetter>();
                asteroidHealthSetter.ReduceHealth(int.MaxValue);

                return;
            }

            int spaceshipChildCount = _spaceshipHolder.childCount;
            if (spaceshipChildCount > 0)
            {
                int randomIndex = Random.Range(0, 1000) % spaceshipChildCount;
                HealthSetter spaceshipHealthSetter =
                    _spaceshipHolder.GetChild(randomIndex).GetComponent<HealthSetter>();
                spaceshipHealthSetter.ReduceHealth(int.MaxValue);
            }
        }

        #endregion DestroyEnemy

        public void CollectPowerUp(PowerUpData powerUp)
        {
            if (!isServer)
                return;

            _collectedPowerUp = powerUp;
            RpcUpdateLocalClientPowerUpDisplay(!powerUp ? null : powerUp.powerUpName);
        }

        [ClientRpc]
        private void RpcUpdateLocalClientPowerUpDisplay(string powerUpName)
        {
            if (!isLocalPlayer || isServer)
                return;

            Sprite powerUpImage = PowerUpGetter.instance.GetPowerUpImageByName(powerUpName);
            PowerUpData powerUp = PowerUpGetter.instance.GetPowerUpByName(powerUpName);

            if (!powerUpImage || !powerUp)
                _powerUpDisplay.enabled = false;
            else
            {
                _collectedPowerUp = powerUp;

                _powerUpDisplay.enabled = true;
                _powerUpDisplay.sprite = powerUpImage;
            }
        }
    }
}