using UnityEngine;

namespace UNetUI.Resources
{
    [System.Serializable]
    public class ItemData
    {
        public byte[] icon;
        public string itemName;
        public string description;

        public Item.ItemClass itemClass;
        public Item.ItemSlot slot;

        public float damage;
        public float defence;
        public float strength;
        public float agility;
        public float intel;

        public ItemData(Item item)
        {
            icon = item.icon.texture.EncodeToPNG();
            itemName = item.itemName;
            description = item.description;

            itemClass = item.itemClass;
            slot = item.slot;

            damage = item.damage;
            defence = item.defence;
            strength = item.strength;
            agility = item.agility;
            intel = item.intel;
        }
    }
}