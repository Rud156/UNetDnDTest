using UnityEngine;

namespace UNetUI.Resources
{
    [CreateAssetMenu(menuName = "Inventory/Data/Item", fileName = "Item")]
    public class Item : ScriptableObject
    {
        public enum ItemClass
        {
            Common,
            Uncommon,
            Rare,
            Legendary,
            Mythical
        }

        public enum ItemSlot
        {
            Weapon,
            Head,
            Body,
            Feet
        }

        public Sprite icon;
        public string itemName;
        [TextArea] public string description;
        
        public float strength;
        public float intel;
        public float agility;
        public float damage;
        public float defence;

        public ItemClass itemClass;
        public ItemSlot slot;
    }
}