using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UNetUI.Extras;

namespace UNetUI.Asteroids.Power_Ups
{
    public abstract class PowerUpAction : NetworkBehaviour
    {
        public PowerUpData powerUp;

        protected bool PowerUpActive;
        protected Coroutine PowerUpCoroutine;

        public bool IsPowerUpActive() => PowerUpActive;

        public abstract void ActivatePowerUp(Transform parent);
        
        // For Display Type PowerUps
        protected virtual void RpcDisplayPowerUpOnClients(GameObject powerUpDisplay, GameObject parent)
        {
            
        }

        // For Modify Type PowerUps
        public virtual void ModifyGameObject(GameObject affectedObject)
        {
        }

        public virtual void DeactivatePowerUp()
        {
            PowerUpActive = false;

            if (PowerUpCoroutine != null)
                StopCoroutine(PowerUpCoroutine);
        }

        protected virtual IEnumerator CountdownPowerUp()
        {
            yield break;
        }
    }
}