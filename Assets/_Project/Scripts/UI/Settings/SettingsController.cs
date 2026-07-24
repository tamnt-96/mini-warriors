using UnityEngine;
using System.Collections.Generic;
using TWR.Audio;
using TWR.Core;
using TWR.Data;
using TWR.Localization;
using CoreKit.UI;

namespace TWR.UI
{
    public class SettingsController : MonoBehaviour
    {
        [SerializeField] SettingsView _view;

        SettingsModel _model;
        LocalizationManager _localization;
        readonly List<string> _localeCodes = new();

        void Awake()
        {
            if (!ServiceLocator.TryGet<AudioManager>(out var audio)) return;
            if (!ServiceLocator.TryGet<HapticManager>(out var haptic)) return;
            _model = new SettingsModel(audio, haptic);

            _view.Refresh(_model.SfxEnabled, _model.BgmEnabled, _model.HapticEnabled);

            _view.SfxToggled += OnSfxToggled;
            _view.BgmToggled += OnBgmToggled;
            _view.HapticToggled += OnHapticToggled;
            _view.RetryPressed += OnRetryPressed;
            _view.LobbyPressed += OnLobbyPressed;
            _view.ClosePressed += OnClosePressed;
            _view.LanguageSelected += OnLanguageSelected;

            SetupLocalizationOptions();
        }

        void OnDestroy()
        {
            _view.SfxToggled -= OnSfxToggled;
            _view.BgmToggled -= OnBgmToggled;
            _view.HapticToggled -= OnHapticToggled;
            _view.RetryPressed -= OnRetryPressed;
            _view.LobbyPressed -= OnLobbyPressed;
            _view.ClosePressed -= OnClosePressed;
            _view.LanguageSelected -= OnLanguageSelected;

            if (_localization != null)
                _localization.OnLanguageChanged -= OnLanguageChanged;
        }

        void OnSfxToggled(bool value) => _model.SfxEnabled = value;

        void OnBgmToggled(bool value) => _model.BgmEnabled = value;

        void OnHapticToggled(bool value) => _model.HapticEnabled = value;

        void OnRetryPressed()
        {
            EventBus<ChangeStateEvent>.Publish(new ChangeStateEvent { stateType = typeof(GameState_Play) });
        }

        void OnLobbyPressed()
        {
            EventBus<ChangeStateEvent>.Publish(new ChangeStateEvent { stateType = typeof(GameState_Lobby) });
        }

        void OnClosePressed()
        {
            UIManager.Instance?.HidePanel<SettingsView>();
        }

        void SetupLocalizationOptions()
        {
            if (!ServiceLocator.TryGet<LocalizationManager>(out var localization)) return;

            _localization = localization;
            _localization.OnLanguageChanged += OnLanguageChanged;

            var entries = _localization.GetAvailableLocales();
            if (entries == null || entries.Count == 0) return;

            _localeCodes.Clear();
            var labels = new List<string>(entries.Count);
            int selectedIndex = 0;

            for (int i = 0; i < entries.Count; i++)
            {
                var entry = entries[i];
                if (entry == null || string.IsNullOrWhiteSpace(entry.Code))
                    continue;

                _localeCodes.Add(entry.Code);
                labels.Add(string.IsNullOrWhiteSpace(entry.DisplayName) ? entry.Code : entry.DisplayName);

                if (LocalizationSettings.NormalizeLocale(entry.Code) == LocalizationSettings.NormalizeLocale(_localization.CurrentLocale))
                    selectedIndex = _localeCodes.Count - 1;
            }

            _view.ConfigureLanguageOptions(labels);
            _view.SetLanguageSelectionWithoutNotify(selectedIndex);
        }

        void OnLanguageSelected(int index)
        {
            if (_localization == null) return;
            if (index < 0 || index >= _localeCodes.Count) return;
            _ = _localization.SetLanguageAsync(_localeCodes[index]);
        }

        void OnLanguageChanged(string localeCode)
        {
            for (int i = 0; i < _localeCodes.Count; i++)
            {
                if (LocalizationSettings.NormalizeLocale(_localeCodes[i]) != LocalizationSettings.NormalizeLocale(localeCode))
                    continue;

                _view.SetLanguageSelectionWithoutNotify(i);
                return;
            }
        }
    }
}
