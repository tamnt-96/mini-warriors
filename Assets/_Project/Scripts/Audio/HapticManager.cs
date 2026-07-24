using UnityEngine;

namespace TWR.Audio
{
    public class HapticManager
    {
        const string EnabledKey = "Haptic.Enabled";

        public bool Enabled { get; private set; }

        public HapticManager()
        {
            Enabled = PlayerPrefs.GetInt(EnabledKey, 1) != 0;
        }

        public void SetEnabled(bool enabled)
        {
            Enabled = enabled;
            PlayerPrefs.SetInt(EnabledKey, enabled ? 1 : 0);
        }

        public void Vibrate()
        {
            if (Enabled) Handheld.Vibrate();
        }
    }
}
