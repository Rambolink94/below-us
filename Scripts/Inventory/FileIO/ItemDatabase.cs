using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BelowUs.Inventory.IO
{
    [CreateAssetMenu(menuName = "Item Database")]
    public class ItemDatabase : ScriptableObject
    {
        [SerializeField] Item[] items;

        public Item GetItemReference(string itemID)
        {
            foreach (Item item in items)
            {
                if (item.ID == itemID) return item;
            }
            return null;
        }

        public Item GetItemCopy(string itemID)
        {
            Item item = GetItemReference(itemID);
            return item != null ? item.GetCopy() : null;
        }

#if UNITY_EDITOR

        private void OnValidate()
        {
            LoadItems();
        }

        private void OnEnable()
        {
            EditorApplication.projectChanged -= LoadItems;
            EditorApplication.projectChanged += LoadItems;
        }

        private void OnDisable()
        {
            EditorApplication.projectChanged -= LoadItems;
        }

        private void LoadItems()
        {
            items = FindAssetsByType<Item>("Assets/Resources/Items/v3");
        }

        public static T[] FindAssetsByType<T>(params string[] foldersToSearch) where T : Object
        {
            string type = typeof(T).ToString().Replace("UnityEngine.", "");

            string[] guids;
            if (foldersToSearch == null || foldersToSearch.Length == 0)
            {
                guids = AssetDatabase.FindAssets("t:" + type);
            }
            else
            {
                guids = AssetDatabase.FindAssets("t:" + type, foldersToSearch);
            }

            T[] assets = new T[guids.Length];

            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                assets[i] = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            }
            return assets;
        }
#endif
    }
}
