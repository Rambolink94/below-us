using System;

namespace BelowUs.Inventory.IO
{
    [Serializable]
    public class ItemSlotSaveData
    {
        public string itemID;
        public int amount;

        public ItemSlotSaveData(string id, int amount)
        {
            itemID = id;
            this.amount = amount;
        }
    }

    [Serializable]
    public class ItemContainerSaveData
    {
        public ItemSlotSaveData[] savedSlots;

        public ItemContainerSaveData(int numItems)
        {
            savedSlots = new ItemSlotSaveData[numItems];
        }
    }
}
