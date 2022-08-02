using UnityEngine;
using UnityEngine.UI;
using BelowUs.CharacterStats;
using BelowUs.Inventory.IO;
using UnityEngine.InputSystem;
using System;

namespace BelowUs.Inventory
{
    public class Character : MonoBehaviour
    {
        public event Action OnHealthChanged;

        public int maxHealth = 50;

        int _health;
        public int Health {
            get { return _health; }
            set
            {
                if (value > maxHealth)
                    _health = maxHealth;
                else if (value < 0)
                    _health = 0;
                else
                    _health = value;

                OnHealthChanged?.Invoke();
            }
        }

        public float dropRange = 1f;

        public CharacterStat strength;
        public CharacterStat agility;
        public CharacterStat intelligence;
        public CharacterStat vitality;

        public Inventory inventory;
        public EquipmentPanel equipmentPanel;
        [SerializeField] CraftingWindow craftingWindow;
        [SerializeField] StatPanel statPanel;
        [SerializeField] ItemTooltip itemTooltip;
        [SerializeField] Image draggableItem;
        [SerializeField] DropItemArea dropItemArea;
        [SerializeField] ItemSaveManager itemSaveManager;

        [Header("Prefabs")]
        [SerializeField] GameObject itemPickupPrefab;

        private BaseItemSlot dragItemSlot;

        private void OnValidate()
        {
            if (itemTooltip == null) itemTooltip = FindObjectOfType<ItemTooltip>();
        }

        private void Awake()
        {
            // TODO: Change this so that health can be saved
            Health = maxHealth;

            statPanel.SetStats(strength, agility, intelligence, vitality);
            statPanel.UpdateStatValues();
            statPanel.UpdateStatNames();

            // Setup Events:
            inventory.OnRightClickEvent += InventoryRightClick;
            equipmentPanel.OnRightClickEvent += EquipmentPanelRightClick;
            // Pointer Enter
            inventory.OnPointerEnterEvent += ShowTooltip;
            equipmentPanel.OnPointerEnterEvent += ShowTooltip;
            craftingWindow.OnPointerEnterEvent += ShowTooltip;
            // Pointer Exit
            inventory.OnPointerExitEvent += HideTooltip;
            equipmentPanel.OnPointerExitEvent += HideTooltip;
            craftingWindow.OnPointerExitEvent += HideTooltip;
            // Begin Drag
            inventory.OnBeginDragEvent += BeginDrag;
            equipmentPanel.OnBeginDragEvent += BeginDrag;
            // End Drag
            inventory.OnEndDragEvent += EndDrag;
            equipmentPanel.OnEndDragEvent += EndDrag;
            // Drag
            inventory.OnDragEvent += Drag;
            equipmentPanel.OnDragEvent += Drag;
            // Drop
            inventory.OnDropEvent += Drop;
            equipmentPanel.OnDropEvent += Drop;
            dropItemArea.OnDropEvent += DropItemOutsideUI;
        }

        private void Start()
        {
            if (itemSaveManager != null)
            {
                itemSaveManager.LoadInventory(this);
                itemSaveManager.LoadEquipment(this);
            }
        }

        private void OnDestroy()
        {
            itemSaveManager.SaveInventory(this);
            itemSaveManager.SaveEquipment(this);
        }

        private void InventoryRightClick(BaseItemSlot itemSlot)
        {
            EquippableItem equippableItem = itemSlot.Item as EquippableItem;
            if (equippableItem != null)
            {
                Equip(equippableItem);
            }

            if (itemSlot.Item is EquippableItem)
            {
                Equip((EquippableItem)itemSlot.Item);
            }
            else if (itemSlot.Item is UsableItem)
            {
                UsableItem usableItem = (UsableItem)itemSlot.Item;
                usableItem.Use(this);

                if (usableItem.isConsumable)
                {
                    inventory.RemoveItem(usableItem);
                    usableItem.Destroy();
                }
            }
        }

        private void EquipmentPanelRightClick(BaseItemSlot itemSlot)
        {
            if (itemSlot.Item is EquippableItem)
            {
                Unequip((EquippableItem)itemSlot.Item);
            }
        }

        private void ShowTooltip(BaseItemSlot itemSlot)
        {
            if (itemSlot.Item != null)
            {
                itemTooltip.ShowTooltip(itemSlot.Item);
            }
        }

        private void HideTooltip(BaseItemSlot itemSlot)
        {
            itemTooltip.HideTooltip();
        }

        private void BeginDrag(BaseItemSlot itemSlot)
        {
            if (itemSlot.Item != null)
            {
                dragItemSlot = itemSlot;
                draggableItem.sprite = itemSlot.Item.icon;
                draggableItem.transform.position = Mouse.current.position.ReadValue();
                draggableItem.enabled = true;
            }
        }
        
        private void EndDrag(BaseItemSlot itemSlot)
        {
            dragItemSlot = null;
            draggableItem.enabled = false;
        }
        
        private void Drag(BaseItemSlot itemSlot)
        {
            if (draggableItem.enabled)
            {
                draggableItem.transform.position = Mouse.current.position.ReadValue();
            }
        }

        private void Drop(BaseItemSlot dropItemSlot)
        {
            if (dragItemSlot == null) return;

            if (dropItemSlot.CanAddStack(dragItemSlot.Item))
            {
                AddStacks(dropItemSlot);
            }
            else if (dropItemSlot.CanReceiveItem(dragItemSlot.Item) && dragItemSlot.CanReceiveItem(dropItemSlot.Item))
            {
                SwapItems(dropItemSlot);
            }
        }

        private void DropItemOutsideUI()
        {
            Debug.Log("Trying to Drop Item.");
            if (dragItemSlot == null) return;

            float x = UnityEngine.Random.Range(transform.position.x - dropRange, transform.position.x + dropRange);
            float z = UnityEngine.Random.Range(transform.position.z - dropRange, transform.position.z + dropRange);

            Vector3 randomVector = new Vector3(x, 0, z);

            GameObject itemPickup = Instantiate(itemPickupPrefab, randomVector, transform.rotation);
            itemPickup.GetComponent<ItemPickup>().InitItemPickup(dragItemSlot.Item, inventory);

            dragItemSlot.Item = null;
        }

        private void SwapItems(BaseItemSlot dropItemSlot)
        {
            EquippableItem dragItem = dragItemSlot.Item as EquippableItem;
            EquippableItem dropItem = dropItemSlot.Item as EquippableItem;

            // TODO: Refactor to not require these checks.
            if (dropItemSlot is EquipmentSlot)
            {
                if (dragItem != null) dragItem.Equip(this);
                if (dropItem != null) dropItem.Unequip(this);
            }
            if (dragItemSlot is EquipmentSlot)
            {
                if (dragItem != null) dragItem.Unequip(this);
                if (dropItem != null) dropItem.Equip(this);
            }
            statPanel.UpdateStatValues();

            Item draggedItem = dragItemSlot.Item;
            int draggedItemAmount = dragItemSlot.Amount;

            dragItemSlot.Item = dropItemSlot.Item;
            dragItemSlot.Amount = dropItemSlot.Amount;

            dropItemSlot.Item = draggedItem;
            dropItemSlot.Amount = draggedItemAmount;
        }

        private void AddStacks(BaseItemSlot dropItemSlot)
        {
            // Add stacks until dropItemSlot is full
            int numAddableStacks = dropItemSlot.Item.maximumStacks - dropItemSlot.Amount;
            int stacksToAdd = Mathf.Min(numAddableStacks, dragItemSlot.Amount);

            // Remove same number of stacks from dragged item
            dropItemSlot.Amount += stacksToAdd;
            dragItemSlot.Amount -= stacksToAdd;
        }

        public void Equip(EquippableItem item)
        {
            if (inventory.RemoveItem(item))
            {
                EquippableItem previousItem;
                if (equipmentPanel.AddItem(item, out previousItem))
                {
                    if (previousItem != null)
                    {
                        inventory.AddItem(previousItem);
                        previousItem.Unequip(this);
                        statPanel.UpdateStatValues();
                    }
                    item.Equip(this);
                    statPanel.UpdateStatValues();
                }
                else
                {
                    inventory.AddItem(item);
                }
            }
        }

        public void Unequip(EquippableItem item)
        {
            if (inventory.CanAddItem(item) && equipmentPanel.RemoveItem(item))
            {
                item.Unequip(this);
                statPanel.UpdateStatValues();
                inventory.AddItem(item);
            }
        }

        public void UpdateStatValues()
        {
            statPanel.UpdateStatValues();
        }

        private ItemContainer openItemContainer;
        private void TransferToItemContainer(BaseItemSlot itemSlot)
        {
            Item item = itemSlot.Item;
            if (item != null && openItemContainer.CanAddItem(item))
            {
                inventory.RemoveItem(item);
                openItemContainer.AddItem(item);
            }
        }

        private void TransferToInventory(BaseItemSlot itemSlot)
        {
            Item item = itemSlot.Item;
            if (item != null && openItemContainer.CanAddItem(item))
            {
                openItemContainer.RemoveItem(item);
                inventory.AddItem(item);
            }
        }

        public void OpenItemContainer(ItemContainer itemContainer)
        {
            openItemContainer = itemContainer;

            inventory.OnRightClickEvent -= InventoryRightClick;
            inventory.OnRightClickEvent += TransferToItemContainer;

            itemContainer.OnRightClickEvent += TransferToInventory;

            itemContainer.OnPointerEnterEvent += ShowTooltip;
            itemContainer.OnPointerExitEvent += HideTooltip;
            itemContainer.OnBeginDragEvent += BeginDrag;
            itemContainer.OnEndDragEvent += EndDrag;
            itemContainer.OnDragEvent += Drag;
            itemContainer.OnDropEvent += Drop;
        }

        public void CloseItemContainer(ItemContainer itemContainer)
        {
            openItemContainer = null;

            inventory.OnRightClickEvent += InventoryRightClick;
            inventory.OnRightClickEvent -= TransferToItemContainer;

            itemContainer.OnRightClickEvent -= TransferToInventory;

            itemContainer.OnPointerEnterEvent -= ShowTooltip;
            itemContainer.OnPointerExitEvent -= HideTooltip;
            itemContainer.OnBeginDragEvent -= BeginDrag;
            itemContainer.OnEndDragEvent -= EndDrag;
            itemContainer.OnDragEvent -= Drag;
            itemContainer.OnDropEvent -= Drop;
        }
    } 
}
