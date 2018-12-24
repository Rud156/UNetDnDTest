using System;

namespace UNetUI.InventoryDnD
{
    [Serializable]
    public class ItemData
    {
        public string itemName;
        public string description;
        
        public float agility;
        public float damage;
        public float defence;
        public float intel;
        public float strength;

        public Item.ItemClass itemClass;
        public Item.ItemSlot slot;

        public ItemData(Item item)
        {
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