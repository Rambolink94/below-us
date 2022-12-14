using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace BelowUs.Inventory
{
    public class ItemSlot : BaseItemSlot, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
    {
        public event Action<BaseItemSlot> OnBeginDragEvent;
        public event Action<BaseItemSlot> OnEndDragEvent;
        public event Action<BaseItemSlot> OnDragEvent;
        public event Action<BaseItemSlot> OnDropEvent;

        private Color dragColor = new Color(1, 1, 1, 0.5f);

        public override bool CanAddStack(Item item, int amount = 1)
        {
            return base.CanAddStack(item, amount) && Amount + amount <= item.maximumStacks;
        }

        public override bool CanReceiveItem(Item item)
        {
            return true;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (Item != null) itemImage.color = dragColor;

            OnBeginDragEvent?.Invoke(this);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (Item != null) itemImage.color = normalColor;

            OnEndDragEvent?.Invoke(this);
        }

        public void OnDrag(PointerEventData eventData)
        {
            OnDragEvent?.Invoke(this);
        }

        public void OnDrop(PointerEventData eventData)
        {
            OnDropEvent?.Invoke(this);
        }
    }
}
