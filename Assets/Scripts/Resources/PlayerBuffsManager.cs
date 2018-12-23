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

        private void Start()
        {
            UpdateUiWithBuffs();
            ClearBuffsAddition();
        }

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
            float modifiedDamage = currentItem.damage;
            float modifiedDefence = currentItem.defence;
            float modifiedStrength = currentItem.strength;
            float modifiedAgility = currentItem.agility;
            float modifiedIntel = currentItem.intel;

            float modifiedHp = (12 * currentItem.strength);
            float modifiedManna = (14 * currentItem.intel);
            float modifiedDodgeChance = (0.2f * currentItem.agility);
            float modifiedCriticalChance = (0.15f * currentItem.agility);

            if (prevItem != null)
            {
                if (currentItem.itemName == prevItem.itemName)
                {
                    modifiedDamage = 0;
                    modifiedDefence = 0;
                    modifiedStrength = 0;
                    modifiedAgility = 0;
                    modifiedIntel = 0;

                    modifiedHp = 0;
                    modifiedManna = 0;
                    modifiedDodgeChance = 0;
                    modifiedCriticalChance = 0;
                }
                else
                {
                    modifiedDamage -= prevItem.damage;
                    modifiedDefence -= prevItem.defence;
                    modifiedStrength -= prevItem.strength;
                    modifiedAgility -= prevItem.agility;
                    modifiedIntel -= prevItem.intel;

                    modifiedHp -= (12 * prevItem.strength);
                    modifiedManna -= (14 * prevItem.intel);
                    modifiedDodgeChance -= (0.2f * prevItem.agility);
                    modifiedCriticalChance -= (0.15f * prevItem.agility);
                }
            }

            damageAdditionText.text = modifiedDamage.ToString("0.00");
            strengthAdditionText.text = modifiedStrength.ToString("0.00");
            intelAdditionText.text = modifiedIntel.ToString("0.00");
            agilityAdditionText.text = modifiedAgility.ToString("0.00");
            defenceAdditionText.text = modifiedDefence.ToString("0.00");

            hpAdditionText.text = modifiedHp.ToString("0.00");
            mannaAdditionText.text = modifiedManna.ToString("0.00");
            dodgeChanceAdditionText.text = modifiedDodgeChance.ToString("0.00");
            criticalRateAdditionText.text = modifiedCriticalChance.ToString("0.00");

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

        public void ClearBuffsAddition()
        {
            damageAdditionText.text = "";
            strengthAdditionText.text = "";
            intelAdditionText.text = "";
            agilityAdditionText.text = "";
            defenceAdditionText.text = "";

            hpAdditionText.text = "";
            mannaAdditionText.text = "";
            dodgeChanceAdditionText.text = "";
            criticalRateAdditionText.text = "";
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