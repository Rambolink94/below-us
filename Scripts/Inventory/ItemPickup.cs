using UnityEngine;
using UnityEngine.InputSystem;

namespace BelowUs.Inventory
{
    public class ItemPickup : MonoBehaviour
    {
        [SerializeField] Item item;
        [SerializeField] int amount = 1;
        [SerializeField] LayerMask interactableLayers;

        private bool isInRange;
        private Inventory inventory;
        private ParticleSystem glowEffect;

        private void Awake()
        {
            glowEffect = GetComponentInChildren<ParticleSystem>();

            if (item != null && inventory != null) InitItemPickup(item, inventory);
        }

        private void Update()
        {
            if(CheckMouseOver())
            {
                ToggleFloatingTooltip();
            }
        }

        private void ToggleFloatingTooltip()
        {

        }

        public void PickupItem()
        {
            if (item != null)
            {
                Item itemCopy = item.GetCopy();
                if (inventory.AddItem(itemCopy))
                {
                    amount--;
                    if (amount == 0)
                    {
                        Destroy(gameObject);
                    }
                }
                else
                {
                    itemCopy.Destroy();
                }
            }
        }

        public void InitItemPickup(Item item, Inventory inventory)
        {
            this.item = item;
            this.inventory = inventory;

            var randomRotation = transform.eulerAngles;
            randomRotation.y = Random.Range(0f, 360f);
            glowEffect.transform.rotation = Quaternion.Euler(randomRotation);

            var col = glowEffect.colorOverLifetime;
            col.enabled = true;

            Gradient grad = new Gradient();
            grad.SetKeys(new GradientColorKey[] { new GradientColorKey(Color.white, 0.0f), new GradientColorKey(item.rarity.rarityColor, 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) });

            col.color = grad;

            glowEffect.Play();

            GameObject placeholder = Instantiate(item.pickupPlaceholder, transform.position, Quaternion.Euler(randomRotation), transform);
            placeholder.layer = gameObject.layer;
        }

        public bool CheckMouseOver()
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out hit, 100f, interactableLayers))
            {
                if (hit.transform.gameObject == gameObject) return true;
            }
            return false;
        }

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
            }
        }
    }
}
