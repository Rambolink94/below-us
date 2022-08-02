using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BelowUs.Inventory
{
    public abstract class UsableItemEffect : ScriptableObject
    {
        public abstract void ExecuteEffect(UsableItem parentItem, Character character);

        public abstract string GetEffectDescription();
    } 
}
