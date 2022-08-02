using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BelowUs.CharacterStats;

namespace BelowUs.Inventory
{
    public class StatPanel : MonoBehaviour
    {
        [SerializeField] StatDisplay[] statDisplays;
        [SerializeField] string[] statNames;

        private CharacterStat[] stats;

        private void OnValidate()
        {
            statDisplays = GetComponentsInChildren<StatDisplay>();
        }

        public void SetStats(params CharacterStat[] characterStats)
        {
            stats = characterStats;

            if (stats.Length > statDisplays.Length)
            {
                Debug.LogError("Not Enough Stat Displays!");
            }

            for (int i = 0; i < statDisplays.Length; i++)
            {
                statDisplays[i].gameObject.SetActive(i < stats.Length);

                if (i < stats.Length) statDisplays[i].Stat = stats[i];
            }
        }

        public void UpdateStatValues()
        {
            for (int i = 0; i < stats.Length; i++)
            {
                statDisplays[i].UpdateStatValue();
            }
        }

        public void UpdateStatNames()
        {
            for (int i = 0; i < statNames.Length; i++)
            {
                statDisplays[i].Name = statNames[i];
            }
        }
    } 
}
