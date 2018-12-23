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
            string savePath = $"{Application.persistentDataPath}/{equippedTag}.{FileExtension}";
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(savePath, FileMode.Create);

            ItemData data = new ItemData(item);

            Debug.Log($"Saving Data At: {savePath}");

            formatter.Serialize(stream, data);
            stream.Close();
        }

        public static Item LoadEquippedItems(string equippedTag)
        {
            string savePath = $"{Application.persistentDataPath}/{equippedTag}.{FileExtension}";
            if (File.Exists(savePath))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                try
                {
                    FileStream stream = new FileStream(savePath, FileMode.Open);

                    ItemData itemData = formatter.Deserialize(stream) as ItemData;
                    stream.Close();
                    
                    Debug.Log($"Loading Data From: {savePath}");

                    if (itemData == null)
                        return null;

                    Item item = ScriptableObject.CreateInstance<Item>();

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
            else
            {
                Debug.Log($"No Save File Found At: {savePath}");
                return null;
            }
        }

        public static void RemoveSavedData(string equippedTag)
        {
            string savePath = $"{Application.persistentDataPath}/{equippedTag}.{FileExtension}";
            Debug.Log($"Removing Data At: {savePath}");

            if (File.Exists(savePath))
                File.Delete(savePath);
        }
    }
}