using System.Collections.Generic;
using UnityEngine;

namespace BelowUs.Inventory.IO
{
    public class ItemSaveManager : MonoBehaviour
    {
        [SerializeField] ItemDatabase itemDatabase;

        private const string inventoryFileName = "Inventory";
        private const string equipmentFileName = "Equipment";

        public void SaveInventory(Character character)
        {
            SaveItems(character.inventory.itemSlots, inventoryFileName);
        }

        public void LoadInventory(Character character)
        {
            ItemContainerSaveData savedSlots = ItemSaveIO.LoadItems(inventoryFileName);
            if (savedSlots == null) return;

            character.inventory.Clear();

            for (int i = 0; i < savedSlots.savedSlots.Length; i++)
            {
                ItemSlot itemSlot = character.inventory.itemSlots[i];
                ItemSlotSaveData savedSlot = savedSlots.savedSlots[i];

                if (savedSlot == null)
                {
                    itemSlot.Item = null;
                    itemSlot.Amount = 0;
                }
                else
                {
                    itemSlot.Item = itemDatabase.GetItemCopy(savedSlot.itemID);
                    itemSlot.Amount = savedSlot.amount;
                }
            }
        }

        public void SaveEquipment(Character character)
        {
            SaveItems(character.equipmentPanel.equipmentSlots, equipmentFileName);
        }

        public void LoadEquipment(Character character)
        {
            ItemContainerSaveData savedSlots = ItemSaveIO.LoadItems(equipmentFileName);
            if (savedSlots == null) return;

            foreach (ItemSlotSaveData savedSlot in savedSlots.savedSlots)
            {
                if (savedSlot == null) continue;

                Item item = itemDatabase.GetItemCopy(savedSlot.itemID);

                // TODO: This is bad. Fix it!
                character.inventory.AddItem(item);
                character.Equip((EquippableItem)item);
            }
        }

        private void SaveItems(IList<ItemSlot> itemSlots, string filename)
        {
            var saveData = new ItemContainerSaveData(itemSlots.Count);

            for (int i = 0; i < saveData.savedSlots.Length; i++)
            {
                ItemSlot itemSlot = itemSlots[i];

                if (itemSlot.Item == null)
                {
                    saveData.savedSlots[i] = null;
                }
                else
                {
                    saveData.savedSlots[i] = new ItemSlotSaveData(itemSlot.Item.ID, itemSlot.Amount);
                }
            }

            ItemSaveIO.SaveItems(saveData, filename);
        }
    } 
}
