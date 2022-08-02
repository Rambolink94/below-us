using UnityEngine;

namespace BelowUs.Inventory.IO
{
    public static class ItemSaveIO
    {
        private static readonly string baseSavePath;

        static ItemSaveIO()
        {
            baseSavePath = Application.persistentDataPath;
        }

        public static void SaveItems(ItemContainerSaveData items, string filename)
        {
            FileReadWrite.WriteToBinaryFile(baseSavePath + "/" + filename + ".dat", items);
        }

        public static ItemContainerSaveData LoadItems(string filename)
        {
            string filePath = baseSavePath + "/" + filename + ".dat";

            if (System.IO.File.Exists(filePath))
            {
                return FileReadWrite.ReadFromBinaryFile<ItemContainerSaveData>(filePath);
            }
            return null;
        }
    }
}
