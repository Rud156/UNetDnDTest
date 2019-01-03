using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
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
    public class PlayerNetworkedPowerUpController : NetworkBehaviour
    {
        private List<PowerUpAction> _powerUpActions;
        private Image _powerUpDisplay;
        private PowerUpData _collectedPowerUp;

        private PowerUpAction _powerUpInUseAction;

        private void Start()
        {
            if (isServer)
            {
                _powerUpActions = new List<PowerUpAction>();

                GameObject[] powerUpActions = GameObject.FindGameObjectsWithTag(TagManager.PowerUpActions);
                foreach (GameObject powerUpActionGameObject in powerUpActions)
                {
                    PowerUpAction powerUpAction = powerUpActionGameObject.GetComponent<PowerUpAction>();
                    _powerUpActions.Add(powerUpAction);
                }
            }

            _powerUpDisplay = GameObject.FindGameObjectWithTag(TagManager.PowerUpDisplay)?.GetComponent<Image>();
            _powerUpDisplay.enabled = false;
        }

        private void Update()
        {
            if (!Input.GetKeyDown(Controls.PowerUpUseKey))
                return;

            CmdUsePowerUp();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!isServer)
                return;

            if (!other.CompareTag(TagManager.PowerUp))
                return;

            CollectPowerUp(other.gameObject);
        }

        public PowerUpAction GetPowerUp() => _powerUpInUseAction;

        [Command]
        private void CmdUsePowerUp()
        {
            if (!isServer)
                return;

            if (!_collectedPowerUp)
                return;

            if (_powerUpInUseAction)
                _powerUpInUseAction.DeactivatePowerUp();

            PowerUpAction powerUpActionToUse = null;
            foreach (PowerUpAction powerUpAction in _powerUpActions)
            {
                if (powerUpAction.powerUp.powerUpName == _collectedPowerUp.powerUpName)
                {
                    powerUpActionToUse = powerUpAction;
                    break;
                }
            }

            SetPowerUpAction(powerUpActionToUse);
            CollectPowerUp(null);
        }

        private void SetPowerUpAction(PowerUpAction powerUpAction)
        {
            if (!isServer)
                return;

            _powerUpInUseAction = powerUpAction;
            powerUpAction.ActivatePowerUp(transform);
        }

        private void CollectPowerUp(GameObject powerUp)
        {
            if (!isServer)
                return;

            PowerUpsController powerUpAction = powerUp?.GetComponent<PowerUpsController>();
            PowerUpData powerUpData = powerUpAction?.powerUp;

            _collectedPowerUp = powerUpData;

            RpcUpdateLocalClientPowerUpDisplay(!powerUpData ? null : powerUpData.powerUpName);
            NetworkServer.Destroy(powerUp);
        }

        [ClientRpc]
        private void RpcUpdateLocalClientPowerUpDisplay(string powerUpName)
        {
            if (!isLocalPlayer || isServer)
                return;

            Sprite powerUpImage =
                PowerUpGetter.instance.GetPowerUpImageByName(powerUpName);

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