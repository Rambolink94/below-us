using System.Text;
using UnityEditor;
using UnityEngine;

namespace BelowUs.Inventory
{
    [CreateAssetMenu(menuName = "Items/Item")]
    public class Item : ScriptableObject
    {
        [SerializeField] string id;
        public string ID { get { return id; } }
        public string itemName;
        public Rarity rarity;
        [Range(1, 999)]
        public int maximumStacks = 1;
        public Sprite icon;
        public GameObject physicalItem;
        [Tooltip("The item that is displayed when they item is on the ground.")]
        public GameObject pickupPlaceholder;

        protected static readonly StringBuilder sb = new StringBuilder();

        #if UNITY_EDITOR
        private void OnValidate()
        {
            string path = AssetDatabase.GetAssetPath(this);
            id = AssetDatabase.AssetPathToGUID(path);
        }
        #endif

        public virtual Item GetCopy()
        {
            return this;
        }

        public virtual void Destroy()
        {

        }

        public virtual string GetItemType()
        {
            return "";
        }

        public virtual string GetItemDescription()
        {
            return "";
        }
    }
}
