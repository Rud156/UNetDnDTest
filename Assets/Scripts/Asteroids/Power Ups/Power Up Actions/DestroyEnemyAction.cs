using UnityEngine;
using UNetUI.Asteroids.Shared;
using UNetUI.Extras;

namespace UNetUI.Asteroids.Power_Ups.Power_Up_Actions
{
    public class DestroyEnemyAction : PowerUpAction
    {
        private Transform _asteroidHolder;
        private Transform _spaceshipHolder;

        private void Start()
        {
            _asteroidHolder = GameObject.FindGameObjectWithTag(TagManager.AsteroidsHolder)?.transform;
            _spaceshipHolder = GameObject.FindGameObjectWithTag(TagManager.SpaceshipHolder)?.transform;
        }

        public override void ActivatePowerUp(Transform parent)
        {
            base.PowerUpActive = true;
            
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

            base.DeactivatePowerUp();
        }
    }
}