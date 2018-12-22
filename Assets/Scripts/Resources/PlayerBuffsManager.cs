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

        private void Start() => UpdateUiWithBuffs();

        public void AddItem(Item item)
        {
            _damage += item.damage;
            _defence += item.defence;
            _strength += item.strength;
            _agility += item.agility;
            _intel += item.intel;

            UpdateUiWithBuffs();
        }

        public void RemoveItem(Item item)
        {
            _damage -= item.damage;
            _defence -= item.defence;
            _strength -= item.strength;
            _agility -= item.agility;
            _intel -= item.intel;

            UpdateUiWithBuffs();
        }

        private void UpdateUiWithBuffs()
        {
            damageText.text = $"Damage: {_damage}";
            defenceText.text = $"Defence: {_defence}";
            strengthText.text = $"Strength: {_strength}";
            agilityText.text = $"Agility: {_agility}";
            intelText.text = $"Intel: {_intel}";

            float hitPoints = 12 * _strength;
            float mannaPoints = 14 * _intel;
            float dodgeChance = 0.2f * _agility;
            float criticalChance = 0.15f * _agility;

            hpText.text = $"HP: {hitPoints}";
            mannaText.text = $"Manna: {mannaPoints}";
            dodgeChanceText.text = $"Dodge Chance: {dodgeChance}";
            criticalRateText.text = $"Critical Rate: {criticalChance}";
        }
    }
}