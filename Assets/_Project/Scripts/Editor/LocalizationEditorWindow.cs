using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using TWR.Data;
using TWR.Localization;

namespace TWR.Core.EditorTools
{
    /// <summary>
    /// Provides localization preview and missing-key diagnostics for configured locales.
    /// </summary>
    public class LocalizationEditorWindow : EditorWindow
    {
        LocalizationSettings _settings;
        Vector2 _scroll;
        string _previewKey = "ui.menu.start";
        int _selectedLocaleIndex;
        string _previewValue = string.Empty;
        string _statusMessage = string.Empty;

        readonly Dictionary<string, Dictionary<string, string>> _loadedTables = new();
        readonly Dictionary<string, List<string>> _missingKeysPerLocale = new();

        [MenuItem("Tools/Mini Warriors/Localization Checker")]
        public static void OpenWindow()
        {
            var window = GetWindow<LocalizationEditorWindow>("Localization Checker");
            window.minSize = new Vector2(540f, 360f);
            window.RefreshTables();
        }

        void OnEnable()
        {
            RefreshTables();
        }

        void OnGUI()
        {
            DrawHeader();
            GUILayout.Space(6f);
            DrawPreviewSection();
            GUILayout.Space(8f);
            DrawMissingKeysSection();
        }

        void DrawHeader()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Reload Localization Data", GUILayout.Height(24f)))
                RefreshTables();

            if (_settings != null)
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label($"Default: {_settings.DefaultLocale} | Fallback: {_settings.FallbackLocale}");
            }
            EditorGUILayout.EndHorizontal();

            if (!string.IsNullOrEmpty(_statusMessage))
                EditorGUILayout.HelpBox(_statusMessage, MessageType.Info);
        }

        void DrawPreviewSection()
        {
            EditorGUILayout.LabelField("Preview Translation", EditorStyles.boldLabel);

            if (_settings == null || _settings.AvailableLocales.Count == 0)
            {
                EditorGUILayout.HelpBox("No localization settings or locales found.", MessageType.Warning);
                return;
            }

            var localeLabels = new string[_settings.AvailableLocales.Count];
            for (int i = 0; i < _settings.AvailableLocales.Count; i++)
            {
                var entry = _settings.AvailableLocales[i];
                localeLabels[i] = string.IsNullOrWhiteSpace(entry.DisplayName) ? entry.Code : entry.DisplayName;
            }

            _selectedLocaleIndex = Mathf.Clamp(_selectedLocaleIndex, 0, _settings.AvailableLocales.Count - 1);
            _selectedLocaleIndex = EditorGUILayout.Popup("Locale", _selectedLocaleIndex, localeLabels);
            _previewKey = EditorGUILayout.TextField("Key", _previewKey);

            if (GUILayout.Button("Preview", GUILayout.Width(120f)))
                RefreshPreview();

            EditorGUILayout.TextArea(_previewValue, GUILayout.MinHeight(44f));
        }

        void DrawMissingKeysSection()
        {
            EditorGUILayout.LabelField("Missing Keys", EditorStyles.boldLabel);

            if (GUILayout.Button("Scan Missing Keys", GUILayout.Width(150f)))
                ScanMissingKeys();

            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            foreach (var pair in _missingKeysPerLocale)
            {
                string locale = pair.Key;
                var missing = pair.Value;
                if (missing.Count == 0)
                {
                    EditorGUILayout.HelpBox($"{locale}: no missing keys.", MessageType.None);
                    continue;
                }

                EditorGUILayout.LabelField($"{locale}: {missing.Count} missing keys", EditorStyles.boldLabel);
                for (int i = 0; i < missing.Count; i++)
                    EditorGUILayout.LabelField("- " + missing[i]);
            }
            EditorGUILayout.EndScrollView();
        }

        void RefreshTables()
        {
            _settings = LocalizationSettings.LoadOrCreateDefaults();
            _loadedTables.Clear();
            _missingKeysPerLocale.Clear();

            if (_settings == null)
            {
                _statusMessage = "Localization settings could not be loaded.";
                return;
            }

            var locales = _settings.AvailableLocales;
            for (int i = 0; i < locales.Count; i++)
            {
                var entry = locales[i];
                if (entry == null || string.IsNullOrWhiteSpace(entry.Code))
                    continue;

                var table = LoadLocaleTable(entry.Code);
                _loadedTables[LocalizationSettings.NormalizeLocale(entry.Code)] = table;
            }

            _statusMessage = $"Loaded {_loadedTables.Count} locale table(s).";
            RefreshPreview();
            ScanMissingKeys();
        }

        void RefreshPreview()
        {
            if (_settings == null || _settings.AvailableLocales.Count == 0)
            {
                _previewValue = string.Empty;
                return;
            }

            var localeCode = LocalizationSettings.NormalizeLocale(_settings.AvailableLocales[_selectedLocaleIndex].Code);
            if (!_loadedTables.TryGetValue(localeCode, out var table))
            {
                _previewValue = $"Locale '{localeCode}' is not loaded.";
                return;
            }

            if (table.TryGetValue(_previewKey, out var value))
                _previewValue = value;
            else
                _previewValue = $"Missing key: {_previewKey}";
        }

        void ScanMissingKeys()
        {
            _missingKeysPerLocale.Clear();
            if (_settings == null || _loadedTables.Count == 0)
                return;

            var baselineLocale = LocalizationSettings.NormalizeLocale(_settings.FallbackLocale);
            if (!_loadedTables.TryGetValue(baselineLocale, out var baselineTable))
            {
                baselineLocale = LocalizationSettings.NormalizeLocale(_settings.DefaultLocale);
                if (!_loadedTables.TryGetValue(baselineLocale, out baselineTable))
                    return;
            }

            foreach (var tablePair in _loadedTables)
            {
                var locale = tablePair.Key;
                var table = tablePair.Value;
                var missing = new List<string>();

                foreach (var baselineKey in baselineTable.Keys)
                {
                    if (!table.ContainsKey(baselineKey))
                        missing.Add(baselineKey);
                }

                _missingKeysPerLocale[locale] = missing;
            }
        }

        Dictionary<string, string> LoadLocaleTable(string localeCode)
        {
            string normalized = LocalizationSettings.NormalizeLocale(localeCode);
            string folder = _settings == null ? "Localization" : _settings.ResourcesFolder;
            if (string.IsNullOrWhiteSpace(folder)) folder = "Localization";
            folder = folder.Trim().Trim('/');

            string assetPath = $"Assets/_Project/Resources/{folder}/{normalized}.json";
            var textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);
            if (textAsset == null)
            {
                string sharedPath = $"Assets/Resources/{folder}/{normalized}.json";
                textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(sharedPath);
            }

            if (textAsset == null)
                return new Dictionary<string, string>();

            if (!LocalizationJsonParser.TryParseStringMap(textAsset.text, out var table, out _))
                return new Dictionary<string, string>();

            return table;
        }
    }
}
