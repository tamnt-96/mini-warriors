using UnityEngine;

namespace CoreKit.UI
{
    public static class Haptics
    {
        public static bool Enabled = true;

        public static void Vibrate()
        {
            if (!Enabled) return;
#if UNITY_ANDROID || UNITY_IOS
            Handheld.Vibrate();
#endif
        }
    }
}
