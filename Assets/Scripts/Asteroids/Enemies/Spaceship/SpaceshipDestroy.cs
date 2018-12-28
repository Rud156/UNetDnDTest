using UnityEngine;
using UnityEngine.Networking;
using UNetUI.Asteroids.Shared;

namespace UNetUI.Asteroids.Enemies.Spaceship
{
    [RequireComponent(typeof(HealthSetter))]
    [RequireComponent(typeof(ScoreSetter))]
    public class SpaceshipDestroy : NetworkBehaviour
    {
        private HealthSetter _healthSetter;

        private void Start()
        {
            _healthSetter = GetComponent<HealthSetter>();
            _healthSetter.healthZero += DestroySpaceship;
        }

        private void DestroySpaceship()
        {
            if(!isServer)
                return;
            
            
        }
    }
}