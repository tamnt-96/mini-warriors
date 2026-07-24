using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using UnityEngine;
using TWR.Core;
using TWR.Data;

namespace TWR.Localization
{
    /// <summary>
    /// Event fired when the active language changes.
    /// </summary>
    public readonly struct LanguageChangedEvent : IEvent
    {
        /// <summary>
        /// Creates the language changed event.
        /// </summary>
        public LanguageChangedEvent(string localeCode)
        {
            LocaleCode = localeCode;
        }

        /// <summary>
        /// Gets the newly active locale code.
        /// </summary>
        public string LocaleCode { get; }
    }

    /// <summary>
    /// Loads and serves localized strings at runtime with locale fallback support.
    /// </summary>
    public class LocalizationManager
    {
        const string SelectedLocalePlayerPrefsKey = "twr.localization.selected_locale";

        static readonly Dictionary<string, string> EmptyTable = new(StringComparer.Ordinal);

        readonly object _syncRoot = new();
        readonly HashSet<string> _missingKeyWarnings = new(StringComparer.Ordinal);
        readonly Dictionary<string, Dictionary<string, string>> _localeCache = new(StringComparer.Ordinal);

        readonly LocalizationSettings _settings;

        Dictionary<string, string> _activeTable = EmptyTable;
        Dictionary<string, string> _fallbackTable = EmptyTable;
        string _currentLocale = "en";
        Task _loadTask = Task.CompletedTask;

        /// <summary>
        /// Initializes a new localization manager.
        /// </summary>
        public LocalizationManager(LocalizationSettings settings)
        {
            _settings = settings == null ? LocalizationSettings.LoadOrCreateDefaults() : settings;
        }

        /// <summary>
        /// Fired after the active locale has changed and locale content has been loaded.
        /// </summary>
        public event Action<string> OnLanguageChanged;

        /// <summary>
        /// Gets the settings currently used by the manager.
        /// </summary>
        public LocalizationSettings Settings => _settings;

        /// <summary>
        /// Gets the current locale code.
        /// </summary>
        public string CurrentLocale
        {
            get
            {
                lock (_syncRoot)
                {
                    return _currentLocale;
                }
            }
        }

        /// <summary>
        /// Gets the locale load task currently in progress.
        /// </summary>
        public Task LoadTask
        {
            get
            {
                lock (_syncRoot)
                {
                    return _loadTask;
                }
            }
        }

        /// <summary>
        /// Initializes the manager and starts loading the initial locale asynchronously.
        /// </summary>
        public void Initialize()
        {
            string preferred = LocalizationSettings.NormalizeLocale(
                PlayerPrefs.GetString(SelectedLocalePlayerPrefsKey, _settings.DefaultLocale));

            if (!_settings.IsLocaleSupported(preferred))
                preferred = _settings.DefaultLocale;

            Task loadTask = SetLanguageAsync(preferred, suppressEvent: true);
            lock (_syncRoot)
            {
                _currentLocale = preferred;
                _loadTask = loadTask;
            }
        }

        /// <summary>
        /// Changes the active locale and loads localization data asynchronously.
        /// </summary>
        public Task SetLanguageAsync(string localeCode)
        {
            return SetLanguageAsync(localeCode, suppressEvent: false);
        }

        /// <summary>
        /// Gets a localized value for a key.
        /// </summary>
        public string Get(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return string.Empty;

            var normalizedKey = key.Trim();
            string locale;
            string fallbackLocale = _settings.FallbackLocale;

            lock (_syncRoot)
            {
                locale = _currentLocale;

                if (_activeTable.TryGetValue(normalizedKey, out var value))
                    return value;

                if (_fallbackTable.TryGetValue(normalizedKey, out value))
                    return value;

                if (_missingKeyWarnings.Add(normalizedKey))
                {
                    Debug.LogWarning(
                        $"[Localization] Missing key '{normalizedKey}' in locale '{locale}' and fallback '{fallbackLocale}'.");
                }
            }

            return normalizedKey;
        }

        /// <summary>
        /// Gets a localized value for a key and applies indexed formatting placeholders.
        /// </summary>
        public string Get(string key, params object[] args)
        {
            var template = Get(key);
            if (args == null || args.Length == 0)
                return template;

            try
            {
                return string.Format(CultureInfo.InvariantCulture, template, args);
            }
            catch (FormatException ex)
            {
                Debug.LogWarning($"[Localization] Format error for key '{key}': {ex.Message}");
                return template;
            }
        }

        /// <summary>
        /// Gets a localized value for a key and applies named placeholders such as {name}.
        /// </summary>
        public string GetNamed(string key, IReadOnlyDictionary<string, object> namedArgs)
        {
            var template = Get(key);
            if (namedArgs == null || namedArgs.Count == 0)
                return template;

            var formatted = template;
            foreach (var pair in namedArgs)
            {
                if (string.IsNullOrEmpty(pair.Key))
                    continue;

                var replacement = pair.Value == null ? string.Empty : Convert.ToString(pair.Value, CultureInfo.InvariantCulture);
                formatted = formatted.Replace("{" + pair.Key + "}", replacement);
            }

            return formatted;
        }

        /// <summary>
        /// Gets a read-only list of locales configured in settings.
        /// </summary>
        public IReadOnlyList<LocaleEntry> GetAvailableLocales()
        {
            return _settings.AvailableLocales;
        }

        async Task SetLanguageAsync(string localeCode, bool suppressEvent)
        {
            var requestedLocale = LocalizationSettings.NormalizeLocale(localeCode);
            if (!_settings.IsLocaleSupported(requestedLocale))
            {
                Debug.LogWarning($"[Localization] Unsupported locale '{requestedLocale}', using default '{_settings.DefaultLocale}'.");
                requestedLocale = _settings.DefaultLocale;
            }

            var fallbackLocale = _settings.FallbackLocale;
            if (!_settings.IsLocaleSupported(fallbackLocale))
                fallbackLocale = _settings.DefaultLocale;

            var activeTable = await GetOrLoadLocaleTableAsync(requestedLocale);
            var fallbackTable = requestedLocale == fallbackLocale
                ? activeTable
                : await GetOrLoadLocaleTableAsync(fallbackLocale);

            lock (_syncRoot)
            {
                _currentLocale = requestedLocale;
                _activeTable = activeTable;
                _fallbackTable = fallbackTable;
            }

            PlayerPrefs.SetString(SelectedLocalePlayerPrefsKey, requestedLocale);
            PlayerPrefs.Save();

            if (suppressEvent) return;

            OnLanguageChanged?.Invoke(requestedLocale);
            EventBus<LanguageChangedEvent>.Publish(new LanguageChangedEvent(requestedLocale));
        }

        async Task<Dictionary<string, string>> GetOrLoadLocaleTableAsync(string localeCode)
        {
            lock (_syncRoot)
            {
                if (_localeCache.TryGetValue(localeCode, out var cached))
                    return cached;
            }

            var path = BuildResourcesPath(localeCode);
            var request = Resources.LoadAsync<TextAsset>(path);
            while (!request.isDone)
                await Task.Yield();

            var textAsset = request.asset as TextAsset;
            if (textAsset == null)
            {
                Debug.LogWarning($"[Localization] Locale file not found at Resources/{path}.json.");
                return EmptyTable;
            }

            Dictionary<string, string> parsedTable = EmptyTable;
            string parseError = string.Empty;

            await Task.Run(() =>
            {
                if (!LocalizationJsonParser.TryParseStringMap(textAsset.text, out parsedTable, out parseError))
                {
                    parsedTable = EmptyTable;
                }
            });

            if (!string.IsNullOrEmpty(parseError))
            {
                Debug.LogWarning($"[Localization] Failed to parse locale '{localeCode}': {parseError}");
            }

            lock (_syncRoot)
            {
                _localeCache[localeCode] = parsedTable;
                return parsedTable;
            }
        }

        string BuildResourcesPath(string localeCode)
        {
            string folder = _settings.ResourcesFolder;
            if (string.IsNullOrWhiteSpace(folder))
                return localeCode;

            folder = folder.Trim().Trim('/');
            return folder + "/" + localeCode;
        }
    }
}
