using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UNetUI.Resources;

namespace UNetUI.Extras
{
    public static class ItemsDataSaver
    {
        public static void SaveEquippedItems(Item item, string equippedTag)
        {
            string savePath = $"{Application.persistentDataPath}/${equippedTag}.UnUiTe";
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(savePath, FileMode.Create);

            ItemData data = new ItemData(item);

            formatter.Serialize(stream, data);
            stream.Close();
        }

        public static Item LoadEquippedItems(string equippedTag)
        {
            string savePath = $"{Application.persistentDataPath}/${equippedTag}.UnUiTe";
            if (File.Exists(savePath))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(savePath, FileMode.Open);

                ItemData itemData = formatter.Deserialize(stream) as ItemData;
                stream.Close();

                if (itemData == null)
                    return null;

                Texture2D texture2D = new Texture2D(512, 512);
                texture2D.LoadImage(itemData.icon);

                Item item = ScriptableObject.CreateInstance<Item>();

                item.icon = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), Vector2.one);
                item.itemName = itemData.itemName;
                item.description = itemData.description;

                item.itemClass = itemData.itemClass;
                item.slot = itemData.slot;

                item.damage = itemData.damage;
                item.defence = itemData.defence;
                item.strength = itemData.strength;
                item.agility = itemData.agility;
                item.intel = itemData.intel;

                return item;
            }
            else
            {
                Debug.Log($"No Save File Found At {savePath}");
                return null;
            }
        }
    }
}