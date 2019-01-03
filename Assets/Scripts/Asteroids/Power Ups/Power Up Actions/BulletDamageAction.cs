using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UNetUI.Asteroids.Shared;

namespace UNetUI.Asteroids.Power_Ups.Power_Up_Actions
{
    public class BulletDamageAction : PowerUpAction
    {
        private float _bulletDamageIncrease;

        public override void ActivatePowerUp(Transform parent)
        {
            base.PowerUpActive = true;
            _bulletDamageIncrease = powerUp.damageAmount;

            base.PowerUpCoroutine = StartCoroutine(CountdownPowerUp());
        }

        public override void ModifyGameObject(GameObject affectedObject)
        {
            if (base.IsPowerUpActive())
                affectedObject.GetComponent<DamageSetter>().damageAmount += _bulletDamageIncrease;
        }

        public override void DeactivatePowerUp()
        {
            _bulletDamageIncrease = 0;
            base.DeactivatePowerUp();
        }

        protected override IEnumerator CountdownPowerUp()
        {
            yield return new WaitForSeconds(powerUp.powerUpAffectTime);
            DeactivatePowerUp();
        }
    }
}