using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TWR.Core;

namespace TWR.Localization
{
    /// <summary>
    /// Provides named formatting arguments to a <see cref="LocalizedText"/> component.
    /// </summary>
    public interface ILocalizedNamedArgumentProvider
    {
        /// <summary>
        /// Populates named arguments used by localization templates.
        /// </summary>
        void PopulateNamedArguments(Dictionary<string, object> namedArguments);
    }

    /// <summary>
    /// Represents a named localization argument.
    /// </summary>
    [Serializable]
    public class LocalizedNamedArgument
    {
        [SerializeField] string _name;
        [SerializeField] string _value;

        /// <summary>
        /// Gets the argument name.
        /// </summary>
        public string Name => _name;

        /// <summary>
        /// Gets the argument value.
        /// </summary>
        public string Value => _value;
    }

    /// <summary>
    /// Updates TMP_Text or legacy UI Text content when language changes.
    /// </summary>
    [DisallowMultipleComponent]
    public class LocalizedText : MonoBehaviour
    {
        [SerializeField] string _key;
        [SerializeField] string[] _indexedArguments = Array.Empty<string>();
        [SerializeField] List<LocalizedNamedArgument> _namedArguments = new();
        [SerializeField] MonoBehaviour _namedArgumentProvider;

        TMP_Text _tmpText;
        Text _legacyText;
        LocalizationManager _manager;

        /// <summary>
        /// Gets or sets localization key.
        /// </summary>
        public string Key
        {
            get => _key;
            set
            {
                _key = value;
                Refresh();
            }
        }

        void Awake()
        {
            CacheTextComponent();
            TryResolveManager();
        }

        void OnEnable()
        {
            CacheTextComponent();
            TryResolveManager();

            if (_manager != null)
                _manager.OnLanguageChanged += OnLanguageChanged;

            Refresh();
        }

        void OnDisable()
        {
            if (_manager != null)
                _manager.OnLanguageChanged -= OnLanguageChanged;
        }

        void OnValidate()
        {
            CacheTextComponent();
            if (!Application.isPlaying)
                RefreshEditorPreview();
        }

        /// <summary>
        /// Rebuilds and applies localized text immediately.
        /// </summary>
        public void Refresh()
        {
            if (!TryResolveManager())
            {
                ApplyText(_key);
                return;
            }

            ApplyText(ResolveValue());
        }

        void OnLanguageChanged(string _)
        {
            Refresh();
        }

        bool TryResolveManager()
        {
            if (_manager != null) return true;

            if (!ServiceLocator.TryGet<LocalizationManager>(out var manager))
                return false;

            _manager = manager;
            return true;
        }

        string ResolveValue()
        {
            if (_manager == null)
                return _key;

            var hasNamedArgs = HasNamedArguments();
            if (hasNamedArgs)
            {
                var named = new Dictionary<string, object>(StringComparer.Ordinal);
                for (int i = 0; i < _namedArguments.Count; i++)
                {
                    var argument = _namedArguments[i];
                    if (argument == null || string.IsNullOrWhiteSpace(argument.Name))
                        continue;

                    named[argument.Name] = argument.Value;
                }

                if (_namedArgumentProvider is ILocalizedNamedArgumentProvider provider)
                    provider.PopulateNamedArguments(named);

                return _manager.GetNamed(_key, named);
            }

            if (_indexedArguments != null && _indexedArguments.Length > 0)
            {
                object[] args = new object[_indexedArguments.Length];
                for (int i = 0; i < _indexedArguments.Length; i++)
                    args[i] = _indexedArguments[i];

                return _manager.Get(_key, args);
            }

            return _manager.Get(_key);
        }

        bool HasNamedArguments()
        {
            if (_namedArgumentProvider is ILocalizedNamedArgumentProvider)
                return true;

            if (_namedArguments == null || _namedArguments.Count == 0)
                return false;

            for (int i = 0; i < _namedArguments.Count; i++)
            {
                var arg = _namedArguments[i];
                if (arg != null && !string.IsNullOrWhiteSpace(arg.Name))
                    return true;
            }

            return false;
        }

        void CacheTextComponent()
        {
            if (_tmpText == null)
                _tmpText = GetComponent<TMP_Text>();

            if (_legacyText == null)
                _legacyText = GetComponent<Text>();

            if (_tmpText == null && _legacyText == null)
            {
                Debug.LogWarning($"[Localization] {nameof(LocalizedText)} on '{name}' requires TMP_Text or Text component.", this);
            }
        }

        void ApplyText(string value)
        {
            if (_tmpText != null)
                _tmpText.text = value;
            else if (_legacyText != null)
                _legacyText.text = value;
        }

        void RefreshEditorPreview()
        {
            if (string.IsNullOrWhiteSpace(_key))
                return;

            ApplyText(_key);
        }
    }
}
