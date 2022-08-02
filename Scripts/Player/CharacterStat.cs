using System;
using System.Collections.Generic;
using UnityEngine;

namespace BelowUs.CharacterStats
{
    [Serializable]
    public class CharacterStat
    {
        public float baseValue;

        protected float _value;
        public virtual float Value
        {
            get
            {
                if (hasChanged || baseValue != lastBaseValue)
                {
                    lastBaseValue = baseValue;
                    _value = CalculateFinalValue();
                    hasChanged = false;
                }
                return _value;
            }
        }

        protected bool hasChanged = true;
        protected float lastBaseValue = float.MinValue;

        [SerializeField] protected readonly List<StatModifier> statModifiers;

        public CharacterStat()
        {
            statModifiers = new List<StatModifier>();
        }
        public CharacterStat(float baseValue) : this()
        {
            this.baseValue = baseValue;
        }

        public virtual List<StatModifier> GetStatModifiers()
        {
            return statModifiers;
        }

        public virtual void AddModifier(StatModifier modifier)
        {
            hasChanged = true;
            statModifiers.Add(modifier);
            statModifiers.Sort(CompareModifierOrder);
        }

        protected virtual int CompareModifierOrder(StatModifier a, StatModifier b)
        {
            if (a.order < b.order)
            {
                return -1;
            }
            else if (a.order > b.order)
            {
                return 1;
            }
            return 0;
        }

        public virtual bool RemoveModifier(StatModifier modifier)
        {
            if (statModifiers.Remove(modifier))
            {
                hasChanged = true;
                return true;
            }
            return false;
        }

        public virtual bool RemoveAllModifiersFromSource(object source)
        {
            bool didRemove = false;

            for (int i = statModifiers.Count - 1; i >= 0; i--)
            {
                if (statModifiers[i].source == source)
                {
                    hasChanged = true;
                    didRemove = true;
                    statModifiers.RemoveAt(i);
                }
            }

            return didRemove;
        }

        protected virtual float CalculateFinalValue()
        {
            float finalValue = baseValue;
            float sumPercentAdd = 0;

            for (int i = 0; i < statModifiers.Count; i++)
            {
                StatModifier modifier = statModifiers[i];

                if (modifier.statType == StatModType.Flat)
                {
                    finalValue += modifier.value;
                }
                else if (modifier.statType == StatModType.PercentAdd)
                {
                    sumPercentAdd += modifier.value;

                    if (i + 1 >= statModifiers.Count || statModifiers[i + 1].statType != StatModType.PercentAdd)
                    {
                        finalValue *= 1 + sumPercentAdd;
                        sumPercentAdd = 0;
                    }
                }
                else if (modifier.statType == StatModType.PercentMult)
                {
                    // ex. finalValue = 10 : 10 *= 1 + 0.1 == 110%
                    finalValue *= 1 + modifier.value;
                }
                finalValue += statModifiers[i].value;
            }


            return (float)Math.Round(finalValue, 4);
        }
    }
}
