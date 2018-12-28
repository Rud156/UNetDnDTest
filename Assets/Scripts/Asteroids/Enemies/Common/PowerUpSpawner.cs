using UnityEngine;
using UnityEngine.Networking;
using UNetUI.Asteroids.Scene.MainScene;
using UNetUI.Asteroids.Shared;
using UNetUI.Extras;

namespace UNetUI.Asteroids.Enemies.Common
{
    [RequireComponent(typeof(HealthSetter))]
    public class PowerUpSpawner : NetworkBehaviour
    {
        public GameObject[] powerUps;
        [SerializeField] [Range(0, 1)] private float spawnProbability;

        private HealthSetter _healthSetter;
        private Transform _powerUpsHolder;

        private void Start()
        {
            if(!isServer)
                return;
            
            _healthSetter = GetComponent<HealthSetter>();
            _healthSetter.healthZero += CheckAndSpawnPowerUp;
            
            _powerUpsHolder = GameObject.FindGameObjectWithTag(TagManager.PowerUpsHolder)?.transform;
        }

        private void CheckAndSpawnPowerUp()
        {
            if (!isServer)
                return;
   
            float randomValue = Random.value;            
            if (randomValue > spawnProbability)
                return;

            int randomPowerUp = Random.Range(0, 1000) % powerUps.Length;

            GameObject powerUpInstance = Instantiate(powerUps[randomPowerUp], transform.position, Quaternion.identity);
            powerUpInstance.transform.SetParent(_powerUpsHolder);

            NetworkServer.Spawn(powerUpInstance);
        }
    }
}