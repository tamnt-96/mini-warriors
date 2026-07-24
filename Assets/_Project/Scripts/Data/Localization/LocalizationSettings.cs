using System;
using System.Collections.Generic;
using UnityEngine;

namespace TWR.Data
{
    /// <summary>
    /// Defines the localization runtime configuration and the list of supported locales.
    /// </summary>
    [CreateAssetMenu(menuName = "TWR/Localization/Settings", fileName = "LocalizationSettings")]
    public class LocalizationSettings : ScriptableObject
    {
        [SerializeField] string _defaultLocale = "en";
        [SerializeField] string _fallbackLocale = "en";
        [SerializeField] string _resourcesFolder = "Localization";
        [SerializeField] List<LocaleEntry> _availableLocales = new()
        {
            new LocaleEntry("en", "English"),
            new LocaleEntry("vi", "Vietnamese"),
            new LocaleEntry("ja", "Japanese"),
            new LocaleEntry("fr", "French")
        };

        /// <summary>
        /// Gets the locale code used when no user preference is found.
        /// </summary>
        public string DefaultLocale => NormalizeLocale(_defaultLocale);

        /// <summary>
        /// Gets the locale code used when a key is missing in the active locale.
        /// </summary>
        public string FallbackLocale => NormalizeLocale(_fallbackLocale);

        /// <summary>
        /// Gets the Resources subfolder where locale JSON files are stored.
        /// </summary>
        public string ResourcesFolder => _resourcesFolder;

        /// <summary>
        /// Gets the configured locale entries.
        /// </summary>
        public IReadOnlyList<LocaleEntry> AvailableLocales => _availableLocales;

        /// <summary>
        /// Loads localization settings from Resources, or returns runtime defaults when missing.
        /// </summary>
        public static LocalizationSettings LoadOrCreateDefaults()
        {
            var settings = Resources.Load<LocalizationSettings>("Localization/LocalizationSettings");
            if (settings != null) return settings;

            settings = Resources.Load<LocalizationSettings>("LocalizationSettings");
            if (settings != null) return settings;

            return CreateInstance<LocalizationSettings>();
        }

        /// <summary>
        /// Returns true when the provided locale code exists in available locales.
        /// </summary>
        public bool IsLocaleSupported(string localeCode)
        {
            var normalized = NormalizeLocale(localeCode);
            for (int i = 0; i < _availableLocales.Count; i++)
            {
                if (NormalizeLocale(_availableLocales[i].Code) == normalized)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns the display name for a locale code.
        /// </summary>
        public string GetLocaleDisplayName(string localeCode)
        {
            var normalized = NormalizeLocale(localeCode);
            for (int i = 0; i < _availableLocales.Count; i++)
            {
                var entry = _availableLocales[i];
                if (NormalizeLocale(entry.Code) == normalized)
                    return string.IsNullOrWhiteSpace(entry.DisplayName) ? entry.Code : entry.DisplayName;
            }

            return normalized;
        }

        /// <summary>
        /// Normalizes locale values into lowercase language tags.
        /// </summary>
        public static string NormalizeLocale(string localeCode)
        {
            if (string.IsNullOrWhiteSpace(localeCode))
                return "en";
            return localeCode.Trim().ToLowerInvariant();
        }
    }

    /// <summary>
    /// Represents one supported locale entry.
    /// </summary>
    [Serializable]
    public class LocaleEntry
    {
        [SerializeField] string _code;
        [SerializeField] string _displayName;

        /// <summary>
        /// Creates a locale entry.
        /// </summary>
        public LocaleEntry(string code, string displayName)
        {
            _code = code;
            _displayName = displayName;
        }

        /// <summary>
        /// Gets the locale code.
        /// </summary>
        public string Code => _code;

        /// <summary>
        /// Gets the locale display name.
        /// </summary>
        public string DisplayName => _displayName;
    }
}
