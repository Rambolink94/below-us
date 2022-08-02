using System;
using System.Collections.Generic;
using UnityEngine;

namespace BelowUs.Inventory
{
    public class Inventory : ItemContainer
    {
        [SerializeField] List<Item> startingItems;
        [SerializeField] Transform itemsParent;

        private void Start()
        {
            SetStartingItems();
        }

        protected override void OnValidate()
        {
            if (itemsParent != null) itemSlots = itemsParent.GetComponentsInChildren<ItemSlot>(includeInactive: true);

            SetStartingItems();
        }

        private void SetStartingItems()
        {
            Clear();
            for (int i = 0; i < startingItems.Count; i++)
            {
                AddItem(startingItems[i].GetCopy());
            }
        }
    }
}
