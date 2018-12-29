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
    public class PlayerPowerUpController : NetworkBehaviour
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

            if (!_collectedPowerUp)
                return;

            CmdUsePowerUp();
        }

        [Command]
        private void CmdUsePowerUp()
        {
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
            _shieldCoroutine = StartCoroutine(DestroyShield(shieldInstance));
        }

        private void ResetShieldDefence()
        {
            StopCoroutine(_shieldCoroutine);
            _isShieldActive = false;
            NetworkServer.Destroy(_shieldInstance);
        }

        private IEnumerator DestroyShield(GameObject shieldInstance)
        {
            _isShieldActive = true;

            yield return new WaitForSeconds(_collectedPowerUp.powerUpAffectTime);
            NetworkServer.Destroy(shieldInstance);

            _isShieldActive = false;
        }

        #endregion Shield

        #region Bullet

        private void ModifyBulletDamage(GameObject bulletInstance)
        {
            if (_isBulletEnhanced)
                bulletInstance.GetComponent<DamageSetter>().damageAmount += _collectedPowerUp.damageAmount;
        }

        private void IncreaseBulletDamage()
        {
            StopCoroutine(_bulletCoroutine);
            _isBulletEnhanced = false;

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

            if (!powerUpImage)
                _powerUpDisplay.enabled = false;
            else
            {
                _powerUpDisplay.enabled = true;
                _powerUpDisplay.sprite = powerUpImage;
            }
        }
    }
}