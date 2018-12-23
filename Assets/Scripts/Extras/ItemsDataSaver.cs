using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UNetUI.Resources;
using UNetUI.SharedData;

namespace UNetUI.Extras
{
    public static class ItemsDataSaver
    {
        private const string FileExtension = "UnUiTe";

        public static void SaveEquippedItems(Item item, string equippedTag)
        {
            var savePath = $"{Application.persistentDataPath}/{equippedTag}.{FileExtension}";
            var formatter = new BinaryFormatter();
            var stream = new FileStream(savePath, FileMode.Create);

            var data = new ItemData(item);

            formatter.Serialize(stream, data);
            stream.Close();
        }

        public static Item LoadEquippedItem(string equippedTag)
        {
            var savePath = $"{Application.persistentDataPath}/{equippedTag}.{FileExtension}";
            if (File.Exists(savePath))
            {
                var formatter = new BinaryFormatter();
                try
                {
                    var stream = new FileStream(savePath, FileMode.Open);

                    var itemData = formatter.Deserialize(stream) as ItemData;
                    stream.Close();

                    if (itemData == null)
                        return null;

                    var item = ScriptableObject.CreateInstance<Item>();

                    item.icon = ItemsManager.instance.GetTextureByName(itemData.itemName);
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
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                    return null;
                }
            }

            return null;
        }

        public static void RemoveSavedData(string equippedTag)
        {
            var savePath = $"{Application.persistentDataPath}/{equippedTag}.{FileExtension}";

            if (File.Exists(savePath))
                File.Delete(savePath);
        }
    }
}