using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BelowUs.Inventory
{
    public class InventoryInput : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] GameObject inventoryWindow;
        [SerializeField] GameObject craftingWindow;

        private void Awake()
        {
            inventoryWindow.SetActive(false);
            craftingWindow.SetActive(false);
        }

        public void OnToggleInventory()
        {
            ToggleWindow(inventoryWindow);
        }

        public void OnToggleNPCMenu()
        {
            ToggleWindow(craftingWindow);
        }

        public void OnCloseAllUI()
        {
            CloseAllUI();
        }

        void CloseAllUI()
        {
            inventoryWindow.SetActive(false);
            craftingWindow.SetActive(false);
        }

        void ToggleWindow(GameObject window)
        {
            window.SetActive(!window.activeSelf);
        }
    }
}