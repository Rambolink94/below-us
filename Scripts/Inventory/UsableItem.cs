using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BelowUs.Inventory
{
    [CreateAssetMenu(menuName = "Items/Usable Item")]
    public class UsableItem : Item
    {
        public bool isConsumable;
        public List<UsableItemEffect> effects;
        public virtual void Use(Character character)
        {
            foreach (UsableItemEffect effect in effects)
            {
                effect.ExecuteEffect(this, character);
            }
        }

        public override Item GetCopy()
        {
            return Instantiate(this);
        }

        public override string GetItemType()
        {
            return isConsumable ? "Consumable" : "Usable";
        }

        public override string GetItemDescription()
        {
            sb.Length = 0;

            foreach (UsableItemEffect effect in effects)
            {
                sb.AppendLine(effect.GetEffectDescription());
            }

            return sb.ToString();
        }
    }

    public enum UseType
    {
        Food,
        Drink
    }
}
