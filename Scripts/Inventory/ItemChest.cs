using UnityEngine;
using UnityEngine.InputSystem;

namespace BelowUs.Inventory
{
    public class ItemChest : ItemContainer
    {
        [SerializeField] Transform itemsParent;
        [SerializeField] Key interactionKey = Key.E;

        private bool isInRange;
        private bool isOpen;

        private Character character;

        protected override void OnValidate()
        {
            if (itemsParent != null) itemSlots = itemsParent.GetComponentsInChildren<ItemSlot>(includeInactive: true);
        }

        protected override void Awake()
        {
            base.Awake();
            itemsParent.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (isInRange && Keyboard.current[interactionKey].wasPressedThisFrame)
            {
                isOpen = !isOpen;
                itemsParent.gameObject.SetActive(isOpen);

                if (isOpen)
                {
                    character.OpenItemContainer(this);
                }
                else
                {
                    character.CloseItemContainer(this);
                }
            }
        }

        //public void OpenContainer(GameObject player)
        //{
        //    character = player.GetComponentInChildren<Character>();

        //    isOpen = !isOpen;
        //    itemsParent.gameObject.SetActive(isOpen);

        //    if (isOpen)
        //    {
        //        character.OpenItemContainer(this);
        //    }
        //    else
        //    {
        //        character.CloseItemContainer(this);
        //    }
        //}

        private void OnTriggerEnter(Collider other)
        {
            CheckCollision(other.gameObject, true);
        }

        private void OnTriggerExit(Collider other)
        {
            CheckCollision(other.gameObject, false);
        }

        private void CheckCollision(GameObject gameObject, bool state)
        {
            if (gameObject.CompareTag("Player"))
            {
                isInRange = state;

                if (!isInRange && isOpen)
                {
                    isOpen = false;
                    itemsParent.gameObject.SetActive(false);
                    character.CloseItemContainer(this);
                }

                if (isInRange)
                {
                    character = gameObject.GetComponentInChildren<Character>();
                }
                else
                {
                    character = null;
                }
            }
        }
    } 
}
