using TWR.Audio;

namespace TWR.UI
{
    public class SettingsModel
    {
        readonly AudioManager _audio;
        readonly HapticManager _haptic;

        public SettingsModel(AudioManager audio, HapticManager haptic)
        {
            _audio = audio;
            _haptic = haptic;
        }

        public bool SfxEnabled
        {
            get => _audio.SfxEnabled;
            set => _audio.SetSfxEnabled(value);
        }

        public bool BgmEnabled
        {
            get => _audio.BgmEnabled;
            set => _audio.SetBgmEnabled(value);
        }

        public bool HapticEnabled
        {
            get => _haptic.Enabled;
            set => _haptic.SetEnabled(value);
        }
    }
}
