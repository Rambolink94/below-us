using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BelowUs.Inventory
{
    public class BaseItemSlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] protected Image itemImage;
        [SerializeField] protected TextMeshProUGUI amountText;

        public event Action<BaseItemSlot> OnPointerEnterEvent;
        public event Action<BaseItemSlot> OnPointerExitEvent;
        public event Action<BaseItemSlot> OnRightClickEvent;

        protected bool isPointerOver;

        protected Color normalColor = Color.white;
        protected Color disabledColor = new Color(1, 1, 1, 0);

        private Item _item;
        public Item Item
        {
            get { return _item; }
            set
            {
                _item = value;
                if (_item == null && Amount != 0) Amount = 0;

                if (_item == null)
                {
                    itemImage.sprite = null;
                    itemImage.color = disabledColor;
                }
                else
                {
                    itemImage.sprite = _item.icon;
                    itemImage.color = normalColor;
                }

                if (isPointerOver)
                {
                    // Simulating mouse moving in and out to correct tooltips
                    OnPointerExit(null);
                    OnPointerEnter(null);
                }
            }
        }

        private int _amount;
        public int Amount
        {
            get { return _amount; }
            set
            {
                _amount = value;
                if (_amount < 0) _amount = 0;
                if (_amount == 0 && Item != null) Item = null;

                if (amountText != null)
                {
                    amountText.enabled = _item != null && _amount > 1;
                    if (amountText.enabled)
                    {
                        amountText.SetText(_amount.ToString());
                    }
                }
            }
        }

        protected virtual void OnValidate()
        {
            if (itemImage == null)
            {
                Image[] childImages = GetComponentsInChildren<Image>();
                foreach (Image image in childImages)
                {
                    if (image.gameObject != gameObject)
                    {
                        itemImage = image;
                    }
                }
            }

            if (amountText == null) amountText = GetComponentInChildren<TextMeshProUGUI>();

            // Refreshing UI
            Item = _item;
            Amount = _amount;
        }

        public virtual bool CanAddStack(Item item, int amount = 1)
        {
            return Item != null && Item.ID == item.ID;
        }

        public virtual bool CanReceiveItem(Item item)
        {
            return false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData != null && eventData.button == PointerEventData.InputButton.Right)
            {
                if (OnRightClickEvent != null)
                {
                    OnRightClickEvent(this);
                }
            }
        }

        protected virtual void OnDisable()
        {
            if (isPointerOver) OnPointerExit(null);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            isPointerOver = true;

            if (OnPointerEnterEvent != null) OnPointerEnterEvent(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isPointerOver = false;

            if (OnPointerExitEvent != null) OnPointerExitEvent(this);
        }
    }
}
