using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BelowUs.Utility
{
    public class UIController : MonoBehaviour
    {
        public GameObject characterMenu;

        public void ToggleCharacterMenu()
        {
            characterMenu.SetActive(!characterMenu.activeSelf);
        }
    }
}
