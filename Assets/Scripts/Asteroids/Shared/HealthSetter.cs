using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UNetUI.Asteroids.Shared
{
    public class HealthSetter : MonoBehaviour
    {
        [SerializeField] private float maxHealth;
        public bool destroyOnZero;

        private float _currentHealth;
        private bool _zeroFunctionInvoked;

        public delegate void HealthZero();

        public HealthZero healthZero;

        private void OnTriggerEnter(Collider other)
        {
            DamageSetter damageSetter = other.GetComponent<DamageSetter>();
            if (!damageSetter)
                ReduceHealth(damageSetter.damageAmount);
        }

        public void AddHealth(float amount)
        {
            if (_currentHealth + amount > maxHealth)
                _currentHealth = maxHealth;
            else
                _currentHealth += amount;
        }

        private void ReduceHealth(float amount)
        {
            _currentHealth -= amount;
            if (_currentHealth < 0 && !_zeroFunctionInvoked)
            {   
                healthZero?.Invoke();
                _zeroFunctionInvoked = true;
                
                if(destroyOnZero)
                    Destroy(gameObject);
            }
        }
    }
}