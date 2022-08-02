using UnityEngine;
using BelowUs.Inventory;

public class LootTable : MonoBehaviour
{
    [SerializeField] ItemChance[] lootTable;

    public Item GetItem()
    {
        float randomNum = Random.Range(0f, 1f);

        foreach (ItemChance itemChance in lootTable)
        {
            if (randomNum <= itemChance.percentChance)
            {
                return itemChance.item;
            }
            else
            {
                randomNum -= itemChance.percentChance;
            }
        }

        return null;
    }

    [System.Serializable]
    public struct ItemChance
    {
        public Item item;
        [Range(0, 1)]
        public float percentChance;
    }
}
