using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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

        [Header("Item Additions")] public Text damageAdditionText;
        public Text strengthAdditionText;
        public Text intelAdditionText;
        public Text agilityAdditionText;
        public Text defenceAdditionText;
        public Text hpAdditionText;
        public Text mannaAdditionText;
        public Text dodgeChanceAdditionText;
        public Text criticalRateAdditionText;

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

        public void DisplayBuffDifference(Item prevItem, Item currentItem)
        {
            float modifiedDamage,
                modifiedDefence,
                modifiedStrength,
                modifiedAgility,
                modifiedIntel,
                modifiedHp,
                modifiedManna,
                modifiedDodgeChance,
                modifiedCriticalChance;

            if (prevItem != null)
            {
                modifiedDamage = _damage - prevItem.damage + currentItem.damage;
                modifiedDefence = _defence - prevItem.defence + currentItem.defence;
                modifiedStrength = _strength - prevItem.strength + currentItem.strength;
                modifiedAgility = _agility - prevItem.agility + currentItem.agility;
                modifiedIntel = _intel - prevItem.intel + currentItem.intel;

                modifiedHp = (12 * _strength) - (12 * prevItem.strength) + (12 * currentItem.strength);
                modifiedManna = (14 * _intel) - (14 * prevItem.intel) + (14 * currentItem.intel);
                modifiedDodgeChance =
                    (0.2f * _agility) - (0.2f * prevItem.agility) + (0.2f * currentItem.agility);
                modifiedCriticalChance =
                    (0.15f * _agility) - (0.15f * prevItem.agility) + (0.15f * currentItem.agility);
            }
            else
            {
                modifiedDamage = _damage + currentItem.damage;
                modifiedDefence = _defence + currentItem.defence;
                modifiedStrength = _strength + currentItem.strength;
                modifiedAgility = _agility + currentItem.agility;
                modifiedIntel = _intel + currentItem.intel;

                modifiedHp = (12 * _strength) + (12 * currentItem.strength);
                modifiedManna = (14 * _intel) + (14 * currentItem.intel);
                modifiedDodgeChance =
                    (0.2f * _agility) + (0.2f * currentItem.agility);
                modifiedCriticalChance =
                    (0.15f * _agility) + (0.15f * currentItem.agility);
            }
            
            float hitPoints = 12 * _strength;
            float mannaPoints = 14 * _intel;
            float dodgeChance = 0.2f * _agility;
            float criticalChance = 0.15f * _agility;

            modifiedDamage -= _damage;
            modifiedDefence -= _defence;
            modifiedStrength -= _strength;
            modifiedAgility -= _agility;
            modifiedIntel -= _intel;

            modifiedHp -= hitPoints;
            modifiedManna -= mannaPoints;
            modifiedDodgeChance -= dodgeChance;
            modifiedCriticalChance -= criticalChance;

            damageAdditionText.text = $"{modifiedDamage}";
            strengthAdditionText.text = $"{modifiedStrength}";
            intelAdditionText.text = $"{modifiedIntel}";
            agilityAdditionText.text = $"{modifiedAgility}";
            defenceAdditionText.text = $"{modifiedDefence}";

            hpAdditionText.text = $"{modifiedHp}";
            mannaAdditionText.text = $"{modifiedManna}";
            dodgeChanceAdditionText.text = $"{modifiedDodgeChance}";
            criticalRateAdditionText.text = $"{modifiedCriticalChance}";

            damageAdditionText.color = modifiedDamage < 0 ? Color.red : Color.green;
            strengthAdditionText.color = modifiedStrength < 0 ? Color.red : Color.green;
            intelAdditionText.color = modifiedIntel < 0 ? Color.red : Color.green;
            agilityAdditionText.color = modifiedAgility < 0 ? Color.red : Color.green;
            defenceAdditionText.color = modifiedDefence < 0 ? Color.red : Color.green;

            hpAdditionText.color = modifiedHp < 0 ? Color.red : Color.green;
            mannaAdditionText.color = modifiedManna < 0 ? Color.red : Color.green;
            dodgeChanceAdditionText.color = modifiedDodgeChance < 0 ? Color.red : Color.green;
            criticalRateAdditionText.color = modifiedCriticalChance < 0 ? Color.red : Color.green;
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