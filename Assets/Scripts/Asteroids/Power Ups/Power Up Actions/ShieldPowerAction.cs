using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace UNetUI.Asteroids.Power_Ups.Power_Up_Actions
{
    public class ShieldPowerAction : PowerUpAction
    {
        private GameObject _powerUpInstance;

        public override void ActivatePowerUp(Transform parent)
        {
            GameObject shieldInstance =
                Instantiate(powerUp.powerUpPrefab, parent.position, Quaternion.identity);
            shieldInstance.transform.SetParent(parent);
            shieldInstance.transform.localPosition = Vector3.zero;

            _powerUpInstance = shieldInstance;
            base.PowerUpActive = true;

            NetworkServer.Spawn(shieldInstance);
            Debug.Log(parent.gameObject);
            RpcDisplayPowerUpOnClients(shieldInstance, parent.gameObject);
            base.PowerUpCoroutine = StartCoroutine(CountdownPowerUp());
        }

        [ClientRpc]
        protected override void RpcDisplayPowerUpOnClients(GameObject shieldInstance, GameObject player)
        {
            if (isServer)
                return;

            shieldInstance.transform.SetParent(player.transform);
            shieldInstance.transform.localPosition = Vector3.zero;
        }

        public override void DeactivatePowerUp()
        {
            _powerUpInstance = null;
            base.DeactivatePowerUp();
        }

        protected override IEnumerator CountdownPowerUp()
        {
            yield return new WaitForSeconds(powerUp.powerUpAffectTime);

            NetworkServer.Destroy(_powerUpInstance);
            DeactivatePowerUp();
        }
    }
}