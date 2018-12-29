using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace UNetUI.Asteroids.Shared
{
    public class HealthSetter : NetworkBehaviour
    {
        [SerializeField] private float maxHealth;
        public bool destroyOnZero;

        private float _currentHealth;
        private bool _zeroFunctionInvoked;

        public delegate void HealthZero();

        public HealthZero healthZero;

        private void Start() => _currentHealth = maxHealth;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!isServer)
                return;

            DamageSetter damageSetter = other.GetComponent<DamageSetter>();
            if (damageSetter != null)
                ReduceHealth(damageSetter.damageAmount);
        }

        public void ReduceHealth(float amount)
        {
            _currentHealth -= amount;

            if (_currentHealth > 0 || _zeroFunctionInvoked)
                return;

            healthZero?.Invoke();
            _zeroFunctionInvoked = true;

            if (destroyOnZero)
                Destroy(gameObject);
        }
    }
}