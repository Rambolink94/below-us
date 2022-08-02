using UnityEngine;

namespace BelowUs.Inventory
{
    [CreateAssetMenu(menuName = "Effects/Health Effect")]
    public class HealthItemEffect : UsableItemEffect
    {
        public int healthAmount;

        public override void ExecuteEffect(UsableItem parentItem, Character character)
        {
            character.Health += healthAmount;
        }

        public override string GetEffectDescription()
        {
            return "Heals for " + healthAmount + " health.";
        }
    } 
}
