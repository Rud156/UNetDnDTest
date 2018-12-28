using System;
using UnityEngine.Networking;
using UNetUI.Asteroids.Enums;

namespace UNetUI.Asteroids.Power_Ups
{
    public class PlayerPowerUpController : NetworkBehaviour
    {
        public float powerUpEffectTime;
        
        private PowerUpType _collectedPowerUp;
        
        private void UsePowerUp()
        {
            switch (_collectedPowerUp)
            {
                case PowerUpType.Shield:
                    break;

                case PowerUpType.BulletDamageIncrease:
                    break;

                case PowerUpType.EnemyDestroy:
                    break;

                case PowerUpType.None:
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void CollectPowerUp(PowerUpType powerUpType) => _collectedPowerUp = powerUpType;
    }
}