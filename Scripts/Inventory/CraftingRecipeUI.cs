using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BelowUs.Inventory
{
    public class CraftingRecipeUI : MonoBehaviour
    {
        [Header("References")]                    
        [SerializeField] RectTransform divider;
        [SerializeField] BaseItemSlot[] itemSlots;

        [Header("Public Variables")]
        public ItemContainer itemContainer;

        private CraftingRecipe _craftingRecipe;
        public CraftingRecipe CraftingRecipe
        {
            get { return _craftingRecipe; }
            set { SetCraftingRecipe(value); }
        }

        public event Action<BaseItemSlot> OnPointerEnterEvent;
        public event Action<BaseItemSlot> OnPointerExitEvent;

        private void OnValidate()
        {
            itemSlots = GetComponentsInChildren<BaseItemSlot>(includeInactive: true);
        }

        private void Start()
        {
            foreach (BaseItemSlot itemSlot in itemSlots)
            {
                itemSlot.OnPointerEnterEvent += slot => OnPointerEnterEvent(slot);
                itemSlot.OnPointerExitEvent += slot => OnPointerExitEvent(slot);
            }
        }

        public void OnCraftButtonClick()
        {
            _craftingRecipe.Craft(itemContainer);
        }

        private void SetCraftingRecipe(CraftingRecipe newCraftingRecipe)
        {
            _craftingRecipe = newCraftingRecipe;

            if (_craftingRecipe != null)
            {
                int slotIndex = 0;
                slotIndex = SetSlots(_craftingRecipe.materials, slotIndex);
                divider.SetSiblingIndex(slotIndex);
                slotIndex = SetSlots(_craftingRecipe.results, slotIndex);

                for (int i = slotIndex; i < itemSlots.Length; i++)
                {
                    itemSlots[i].transform.parent.gameObject.SetActive(true);
                }

                gameObject.SetActive(true);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        private int SetSlots(IList<ItemAmount> itemAmountList, int slotIndex)
        {
            for (int i = 0; i < itemAmountList.Count; i++, slotIndex++)
            {
                ItemAmount itemAmount = itemAmountList[i];
                BaseItemSlot itemSlot = itemSlots[slotIndex];

                itemSlot.Item = itemAmount.item;
                itemSlot.Amount = itemAmount.amount;
                itemSlot.transform.parent.gameObject.SetActive(true);
            }
            return slotIndex;
        }
    }
}
