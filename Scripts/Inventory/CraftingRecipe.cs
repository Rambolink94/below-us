using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BelowUs.Inventory
{
    [Serializable]
    public struct ItemAmount
    {
        public Item item;
        [Range(1, 999)]
        public int amount;
    }

    [CreateAssetMenu(menuName = "Recipe")]
    public class CraftingRecipe : ScriptableObject
    {
        public List<ItemAmount> materials;
        public List<ItemAmount> results;

        public bool CanCraft(IItemContainer itemContainer)
        {
            return HasMaterials(itemContainer) && HasSpace(itemContainer);
        }

        private bool HasMaterials(IItemContainer itemContainer)
        {
            foreach (ItemAmount itemAmount in materials)
            {
                if (itemContainer.ItemCount(itemAmount.item.ID) < itemAmount.amount)
                {
                    Debug.LogWarning("You don't have the required materials.");
                    return false;
                }
            }

            return true;
        }

        private bool HasSpace(IItemContainer itemContainer)
        {
            foreach (ItemAmount itemAmount in results)
            {
                if (!itemContainer.CanAddItem(itemAmount.item, itemAmount.amount))
                {
                    // TODO: Consider triggering an inventory full event here.
                    Debug.LogWarning("Inventory Full!");
                    return false;
                }
            }

            return true;
        }

        public void Craft(IItemContainer itemContainer)
        {
            if (CanCraft(itemContainer))
            {
                RemoveMaterials(itemContainer);
                AddResults(itemContainer);
            }
        }

        private void RemoveMaterials(IItemContainer itemContainer)
        {
            foreach (ItemAmount itemAmount in materials)
            {
                for (int i = 0; i < itemAmount.amount; i++)
                {
                    Item oldItem = itemContainer.RemoveItem(itemAmount.item.ID);
                    oldItem.Destroy();
                }
            }
        }

        private void AddResults(IItemContainer itemContainer)
        {
            foreach (ItemAmount itemAmount in results)
            {
                for (int i = 0; i < itemAmount.amount; i++)
                {
                    itemContainer.AddItem(itemAmount.item.GetCopy());
                }
            }
        }
    }
}