#if UNITY_EDITOR

using UnityEngine;

namespace BelowUs.Utility
{
    public class DebugManager : MonoBehaviour
    {
        public bool useSceneView = false;

        private static DebugManager instance = null;
        public static DebugManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DebugManager();
                }
                return instance;
            }
        }

        private void Awake()
        {
            if (useSceneView)
            {
                UnityEditor.SceneView.FocusWindowIfItsOpen(typeof(UnityEditor.SceneView));
            }
        }

        public static void DumpToConsole(object obj)
        {
            var output = JsonUtility.ToJson(obj, true);
            Debug.Log(output);
        }
    }
}
#endif