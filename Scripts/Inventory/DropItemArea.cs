using UnityEngine;
using UnityEngine.EventSystems;
using System;

namespace BelowUs.Inventory
{
    public class DropItemArea : MonoBehaviour, IDropHandler
    {
        public event Action OnDropEvent;

        // TODO: Figure out why this doesn't work.
        public void OnDrop(PointerEventData eventData)
        {
            OnDropEvent?.Invoke();
        }
    }
}
