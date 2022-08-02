using System.Text;
using UnityEngine;
using TMPro;
using BelowUs.CharacterStats;

namespace BelowUs.Inventory
{
    public class StatTooltip : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI statNameText;
        [SerializeField] TextMeshProUGUI statModifiersLabelText;
        [SerializeField] TextMeshProUGUI statModifiersText;

        private StringBuilder sb = new StringBuilder();

        public void ShowTooltip(CharacterStat stat, string statName)
        {
            statNameText.SetText(GetStatTopText(stat, statName));
            statModifiersText.SetText(GetStatModifiersText(stat));

            gameObject.SetActive(true);
        }

        public void HideTooltip()
        {
            gameObject.SetActive(false);
        }

        private string GetStatTopText(CharacterStat stat, string statName)
        {
            sb.Length = 0;
            sb.Append(statName);
            sb.Append(" ");
            sb.Append(stat.Value);

            if (stat.Value != stat.baseValue)
            {
                sb.Append(" (");
                sb.Append(stat.baseValue);

                if (stat.Value > stat.baseValue) sb.Append("+");

                sb.Append(System.Math.Round(stat.Value - stat.baseValue, 4));
                sb.Append(")");
            }

            return sb.ToString();
        }

        private string GetStatModifiersText(CharacterStat stat)
        {
            sb.Length = 0;

            foreach (StatModifier modifier in stat.GetStatModifiers())
            {
                if (sb.Length > 0) sb.AppendLine();

                if (modifier.value > 0) sb.Append("+");

                if (modifier.statType == StatModType.Flat)
                {
                    sb.Append(modifier.value);
                }
                else
                {
                    sb.Append(modifier.value * 100);
                    sb.Append("%");
                }

                Item item = modifier.source as Item;

                if (item != null)
                {
                    sb.Append(" ");
                    sb.Append(item.itemName);
                }
                else
                {
                    Debug.LogError("Modifier is not an Item!");
                }
            }

            return sb.ToString();
        }
    } 
}
