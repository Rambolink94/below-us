using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BelowUs.CharacterStats;

namespace BelowUs.Inventory
{
    [CreateAssetMenu(menuName = "Items/EquippableItem")]
    public class EquippableItem : Item
    {
        // TODO: Change this approach. It's, well, just not the best.
        public int strenghtBonus;
        public int agilityBonus;
        public int intelligenceBonus;
        public int vitalityBonus;
        [Space]
        public float strengthPercentBonus;
        public float agilityPercentBonus;
        public float intelligencePercentBonus;
        public float vitalityPercentBonus;
        [Space]
        public EquipmentType equipmentType;

        public override Item GetCopy()
        {
            return Instantiate(this);
        }

        public override void Destroy()
        {
            Destroy(this);
        }

        public void Equip(Character character)
        {
            // TODO: Fix this. It is not at all extendable.
            if (strenghtBonus != 0) character.strength.AddModifier(new StatModifier(strenghtBonus, StatModType.Flat, this));
            if (agilityBonus != 0) character.agility.AddModifier(new StatModifier(agilityBonus, StatModType.Flat, this));
            if (intelligenceBonus != 0) character.intelligence.AddModifier(new StatModifier(intelligenceBonus, StatModType.Flat, this));
            if (vitalityBonus != 0) character.vitality.AddModifier(new StatModifier(vitalityBonus, StatModType.Flat, this));

            if (strengthPercentBonus != 0) character.strength.AddModifier(new StatModifier(strengthPercentBonus, StatModType.PercentMult, this));
            if (agilityPercentBonus != 0) character.agility.AddModifier(new StatModifier(agilityPercentBonus, StatModType.PercentMult, this));
            if (intelligencePercentBonus != 0) character.intelligence.AddModifier(new StatModifier(intelligencePercentBonus, StatModType.PercentMult, this));
            if (vitalityPercentBonus != 0) character.vitality.AddModifier(new StatModifier(vitalityPercentBonus, StatModType.PercentMult, this));
        }

        public void Unequip(Character character)
        {
            character.strength.RemoveAllModifiersFromSource(this);
            character.agility.RemoveAllModifiersFromSource(this);
            character.intelligence.RemoveAllModifiersFromSource(this);
            character.vitality.RemoveAllModifiersFromSource(this);
        }

        public override string GetItemType()
        {
            return equipmentType.ToString();
        }

        public override string GetItemDescription()
        {
            sb.Length = 0;
            AddStat(strenghtBonus, "Strength");
            AddStat(agilityBonus, "Agility");
            AddStat(intelligenceBonus, "Intelligence");
            AddStat(vitalityBonus, "Vitality");

            AddStat(strengthPercentBonus, "Vitality", true);
            AddStat(agilityPercentBonus, "Vitality", true);
            AddStat(intelligencePercentBonus, "Vitality", true);
            AddStat(vitalityPercentBonus, "Vitality", true);
            return sb.ToString();
        }

        public void AddStat(float value, string statName, bool isPercent = false)
        {
            if (value != 0)
            {
                if (sb.Length > 0) sb.AppendLine();

                if (value > 0) sb.Append("+");

                if (isPercent)
                {
                    sb.Append(value * 100);
                    sb.Append("% ");
                }
                else
                {
                    sb.Append(value);
                    sb.Append(" ");
                    sb.Append(statName);
                }
            }
        }
    }

    public enum EquipmentType
    {
        Helmet,
        Chest,
        Gloves,
        Boots,
        WeaponPrimary,
        WeaponSecondary
    }
}