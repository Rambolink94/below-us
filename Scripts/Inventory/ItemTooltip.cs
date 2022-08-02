using UnityEngine;
using TMPro;

namespace BelowUs.Inventory
{
    public class ItemTooltip : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI itemNameText;
        [SerializeField] TextMeshProUGUI itemTypeText;
        [SerializeField] TextMeshProUGUI itemDescriptionText;

        public void ShowTooltip(Item item)
        {
            itemNameText.text = item.itemName;
            itemTypeText.text = item.GetItemType();
            itemDescriptionText.text = item.GetItemDescription();

            gameObject.SetActive(true);
        }

        public void HideTooltip()
        {
            gameObject.SetActive(false);
        }
    } 
}
