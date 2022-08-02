using System.IO;
using UnityEngine;

namespace BelowUs.Utility
{
    public class SaveManager
    {
        /*
        public static void LoadOrInitializeInventory()
        {
            if (File.Exists(Path.Combine(Application.persistentDataPath, "inventory.json")))
            {
                Debug.Log("Found file inventory.json, loading inventory...");
                Inventory.Inventory.LoadFromJSON(Path.Combine(Application.persistentDataPath, "inventory.json"));
            }
            else
            {
                Debug.Log("Couldn't find inventory.json, loading from template...");
                Inventory.Inventory.InitializeFromDefault();
            }
        }

        public static void SaveInventory()
        {
            Inventory.Inventory.Instance.SaveToJSON(Path.Combine(Application.persistentDataPath, "inventory.json"));
        }

        public static void LoadFromTemplate()
        {
            Inventory.Inventory.InitializeFromDefault();
        }
        */
    }
}
