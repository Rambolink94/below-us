using BelowUs.CharacterStats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BelowUs.Inventory
{
    // TODO: Allow selection of different stats
    [CreateAssetMenu(menuName = "Effects/Stat Buff Effect")]
    public class StatBuffItemEffect : UsableItemEffect
    {
        public int agilityBuff;
        public float duration;

        public override void ExecuteEffect(UsableItem parentItem, Character character)
        {
            StatModifier statModifier = new StatModifier(agilityBuff, StatModType.Flat, parentItem);
            character.agility.AddModifier(statModifier);
            character.StartCoroutine(RemoveBuff(character, statModifier, duration));
            character.UpdateStatValues();
        }

        public override string GetEffectDescription()
        {
            return "Grants " + agilityBuff + " Agility for " + duration + " seconds.";
        }

        private static IEnumerator RemoveBuff(Character character, StatModifier statModifier, float duration)
        {
            yield return new WaitForSeconds(duration);
            character.agility.RemoveModifier(statModifier);
            character.UpdateStatValues();
        }
    }
}
