using System;
using System.Collections.Generic;
using UnityEngine;

namespace CoreKit.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [SerializeField] List<BasePanel> _panels;

        readonly Dictionary<Type, BasePanel> _panelMap = new();
        readonly List<BasePanel> _popupStack = new();

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            foreach (var panel in _panels)
                if (panel != null)
                    _panelMap[panel.GetType()] = panel;
        }

        void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        // Shows the panel. Popups are pushed onto the back-navigation stack.
        public T ShowPanel<T>() where T : BasePanel
        {
            if (!_panelMap.TryGetValue(typeof(T), out var panel))
            {
                Debug.LogWarning($"[UIManager] Panel not registered: {typeof(T).Name}");
                return null;
            }

            panel.Show();

            if (panel.IsPopup)
                _popupStack.Add(panel);

            return (T)panel;
        }

        public void HidePanel<T>() where T : BasePanel
        {
            if (!_panelMap.TryGetValue(typeof(T), out var panel))
                return;

            _popupStack.Remove(panel);
            panel.Hide();
        }

        // Closes the topmost popup (back-button behavior). Returns false if stack is empty.
        public bool CloseTopPopup()
        {
            if (_popupStack.Count == 0) return false;

            var top = _popupStack[^1];
            _popupStack.RemoveAt(_popupStack.Count - 1);
            top.Hide();
            return true;
        }

        public void CloseAllPopups()
        {
            for (int i = _popupStack.Count - 1; i >= 0; i--)
                _popupStack[i].Hide();
            _popupStack.Clear();
        }

        // Returns a panel reference without showing it.
        public T GetPanel<T>() where T : BasePanel
        {
            _panelMap.TryGetValue(typeof(T), out var panel);
            return (T)panel;
        }

        public bool IsVisible<T>() where T : BasePanel
        {
            return _panelMap.TryGetValue(typeof(T), out var panel) && panel.IsVisible;
        }
    }
}
