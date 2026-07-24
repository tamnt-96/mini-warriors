using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using CoreKit.UI;

namespace TWR.UI
{
    public class SettingsView : BasePanel
    {
        [SerializeField] UISwitch _sfxToggle;
        [SerializeField] UISwitch _bgmToggle;
        [SerializeField] UISwitch _hapticToggle;
        [SerializeField] Button _retryButton;
        [SerializeField] Button _lobbyButton;
        [SerializeField] Button _closeButton;
        [SerializeField] TMP_Dropdown _languageDropdown;

        public event Action<bool> SfxToggled;
        public event Action<bool> BgmToggled;
        public event Action<bool> HapticToggled;
        public event Action RetryPressed;
        public event Action LobbyPressed;
        public event Action ClosePressed;
        public event Action<int> LanguageSelected;

        void Awake()
        {
            if (_sfxToggle != null) _sfxToggle.ValueChanged += v => SfxToggled?.Invoke(v);
            if (_bgmToggle != null) _bgmToggle.ValueChanged += v => BgmToggled?.Invoke(v);
            if (_hapticToggle != null) _hapticToggle.ValueChanged += v => HapticToggled?.Invoke(v);

            if (_retryButton != null) _retryButton.onClick.AddListener(() => RetryPressed?.Invoke());
            if (_lobbyButton != null) _lobbyButton.onClick.AddListener(() => LobbyPressed?.Invoke());
            if (_closeButton != null) _closeButton.onClick.AddListener(() => ClosePressed?.Invoke());
            if (_languageDropdown != null) _languageDropdown.onValueChanged.AddListener(i => LanguageSelected?.Invoke(i));
        }

        public void Refresh(bool sfx, bool bgm, bool haptic)
        {
            if (_sfxToggle != null) _sfxToggle.SetIsOnWithoutNotify(sfx);
            if (_bgmToggle != null) _bgmToggle.SetIsOnWithoutNotify(bgm);
            if (_hapticToggle != null) _hapticToggle.SetIsOnWithoutNotify(haptic);
        }

        public void ConfigureLanguageOptions(IReadOnlyList<string> labels)
        {
            if (_languageDropdown == null || labels == null) return;

            _languageDropdown.ClearOptions();
            var options = new List<TMP_Dropdown.OptionData>(labels.Count);
            for (int i = 0; i < labels.Count; i++)
                options.Add(new TMP_Dropdown.OptionData(labels[i]));
            _languageDropdown.AddOptions(options);
        }

        public void SetLanguageSelectionWithoutNotify(int index)
        {
            if (_languageDropdown == null) return;
            if (index < 0 || index >= _languageDropdown.options.Count) return;
            _languageDropdown.SetValueWithoutNotify(index);
        }
    }
}
