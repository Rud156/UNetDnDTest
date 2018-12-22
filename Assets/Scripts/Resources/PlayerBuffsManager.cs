using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UNetUI.Resources
{
    public class PlayerBuffsManager : MonoBehaviour
    {
        #region Singleton

        public static PlayerBuffsManager instance;

        private void Awake()
        {
            if (instance == null)
                instance = this;

            if (instance != this)
                Destroy(gameObject);
        }

        #endregion Singleton

        [Header("UI Display")] public Text damageText;
        public Text strengthText;
        public Text intelText;
        public Text agilityText;
        public Text defenceText;
        public Text hpText;
        public Text mannaText;
        public Text dodgeChanceText;
        public Text criticalRateText;

        private float _damage;
        private float _defence;
        private float _strength;
        private float _agility;
        private float _intel;

        #region Damage

        public void AddDamage(float damage) => _damage += damage;
        public void RemoveDamage(float damage) => _damage -= damage;

        #endregion Damage

        #region Defence

        public void AddDefence(float defence) => _defence += defence;
        public void RemoveDefence(float defence) => _defence -= defence;

        #endregion Defence

        #region Strength

        public void AddStrength(float strength) => _strength += strength;
        public void RemoveStrength(float strength) => _strength -= strength;

        #endregion Strength

        #region Agility

        public void AddAgility(float agility) => _agility += agility;
        public void RemoveAgility(float agility) => _agility -= agility;

        #endregion Agility

        #region Intel

        public void AddIntel(float intel) => _intel += intel;
        public void RemoveIntel(float intel) => _intel -= intel;

        #endregion Intel
    }
}