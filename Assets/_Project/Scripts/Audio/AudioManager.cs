using UnityEngine;

namespace TWR.Audio
{
    public class AudioManager
    {
        const string SfxEnabledKey = "Audio.SfxEnabled";
        const string BgmEnabledKey = "Audio.BgmEnabled";

        AudioSource _musicSource;
        AudioSource _sfxSource;

        public bool SfxEnabled { get; private set; } = true;
        public bool BgmEnabled { get; private set; } = true;

        public void Initialize(AudioSource musicSource, AudioSource sfxSource)
        {
            _musicSource = musicSource;
            _sfxSource = sfxSource;

            SetSfxEnabled(PlayerPrefs.GetInt(SfxEnabledKey, 1) != 0);
            SetBgmEnabled(PlayerPrefs.GetInt(BgmEnabledKey, 1) != 0);
        }

        public void PlaySFX(AudioClip clip)
        {
            if (clip == null) return;
            _sfxSource?.PlayOneShot(clip);
        }

        public void PlayMusic(AudioClip clip, bool loop = true)
        {
            if (clip == null || _musicSource == null) return;
            if (_musicSource.clip == clip) return;
            _musicSource.clip = clip;
            _musicSource.loop = loop;
            _musicSource.Play();
        }

        public void StopMusic() => _musicSource?.Stop();

        public void SetSfxEnabled(bool enabled)
        {
            SfxEnabled = enabled;
            if (_sfxSource != null) _sfxSource.mute = !enabled;
            PlayerPrefs.SetInt(SfxEnabledKey, enabled ? 1 : 0);
        }

        public void SetBgmEnabled(bool enabled)
        {
            BgmEnabled = enabled;
            if (_musicSource != null) _musicSource.mute = !enabled;
            PlayerPrefs.SetInt(BgmEnabledKey, enabled ? 1 : 0);
        }
    }
}