using UnityEngine;

namespace CoreKit.UI
{
    [CreateAssetMenu(fileName = "UIButtonSettings", menuName = "CoreKit/UI/UIButton Settings")]
    public class UIButtonSettings : ScriptableObject
    {
        public AudioClip defaultClickSfx;
        [Range(0.5f, 1f)] public float pressScale = 0.94f;
        public float animDuration = 0.08f;
        public bool hapticsEnabled = true;

        static UIButtonSettings _instance;

        // Optional: place an asset at "Assets/.../Resources/UIButtonSettings.asset" to override these defaults project-wide.
        public static UIButtonSettings Instance
        {
            get
            {
                if (_instance == null)
                    _instance = Resources.Load<UIButtonSettings>("UIButtonSettings");
                return _instance;
            }
        }
    }
}
