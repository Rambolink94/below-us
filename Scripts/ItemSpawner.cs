using BelowUs.Inventory;
using System;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] GameObject itemPickupPrefab;
    [SerializeField] Inventory inventory;

    private ItemPickup itemPickup;
    private LootTable lootTable;

    private void Start()
    {
        lootTable = GetComponent<LootTable>();
    }

    public void GeneratePickup()
    {
        GameObject itemPickupObject = Instantiate(itemPickupPrefab, transform.position, transform.rotation);

        Item item = lootTable.GetItem();
        itemPickup = itemPickupObject.GetComponent<ItemPickup>();
        itemPickup.InitItemPickup(item, inventory);
    }
}
