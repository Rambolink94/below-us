using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BelowUs.Inventory
{
    public class CraftingWindow : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] CraftingRecipeUI recipeUIPrefab;
        [SerializeField] RectTransform recipeUIParent;
        [SerializeField] List<CraftingRecipeUI> craftingRecipeUIs;

        [Header("Public Variables")]
        public ItemContainer itemContainer;
        public List<CraftingRecipe> craftingRecipes;

        public event Action<BaseItemSlot> OnPointerEnterEvent;
        public event Action<BaseItemSlot> OnPointerExitEvent;

        private void OnValidate()
        {
            Init();
        }

        private void Start()
        {
            Init();

            foreach (CraftingRecipeUI craftingRecipeUI in craftingRecipeUIs)
            {
                craftingRecipeUI.OnPointerEnterEvent += slot => OnPointerEnterEvent(slot);
                craftingRecipeUI.OnPointerExitEvent += slot => OnPointerExitEvent(slot);
            }
        }

        private void Init()
        {
            recipeUIParent.GetComponentsInChildren(true, craftingRecipeUIs);
            UpdateCraftingRecipes();
        }

        public void UpdateCraftingRecipes()
        {
            for (int i = 0; i < craftingRecipes.Count; i++)
            {
                if (craftingRecipeUIs.Count == i)
                {
                    craftingRecipeUIs.Add(Instantiate(recipeUIPrefab, recipeUIParent, false));
                }
                else if (craftingRecipeUIs[i] == null)
                {
                    craftingRecipeUIs[i] = Instantiate(recipeUIPrefab, recipeUIParent, false);
                }

                craftingRecipeUIs[i].itemContainer = itemContainer;
                craftingRecipeUIs[i].CraftingRecipe = craftingRecipes[i];
            }

            for (int i = craftingRecipes.Count; i < craftingRecipeUIs.Count; i++)
            {
                craftingRecipeUIs[i].CraftingRecipe = null;
            }
        }
    }
}
