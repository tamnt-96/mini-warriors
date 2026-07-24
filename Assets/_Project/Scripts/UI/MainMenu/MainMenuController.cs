using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace TWR
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private RectTransform _content;
        [SerializeField] private MenuTabBase[] _tabs;
        [SerializeField] private MenuTabButton[] _tabButtons;
        [SerializeField] private MenuTabType _defaultTab;
        [SerializeField] private float _scrollDuration = 0.5f;
        [SerializeField] private Ease _scrollEase = Ease.InOutSine;
        
        private int _currentTabIndex = 0;
        private float _tabWidth;
        private Tween _scrollTween;

        private void Start()
        {
            RefreshLayout();
            BindTabButtons();

            if (_tabs == null || _tabs.Length == 0)
            {
                return;
            }

            _currentTabIndex = Mathf.Clamp((int)_defaultTab, 0, _tabs.Length - 1);
            GoToTab(_currentTabIndex, true);
        }

        private void RefreshLayout()
        {
            _tabWidth = ((RectTransform)_scrollRect.transform).rect.width;
            for (int i = 0; i < _tabs.Length; i++)
            {
                _tabs[i].SetTabPreferredWidth(_tabWidth);
            }
        }

        public void GoToTab(MenuTabType tabType) => GoToTab((int)tabType);

        public void GoToTab(int tabIndex, bool instant = false)
        {
            if (_tabs == null || _tabs.Length == 0)
            {
                return;
            }

            tabIndex = Mathf.Clamp(tabIndex, 0, _tabs.Length - 1);

            if (_currentTabIndex >= 0 && _currentTabIndex < _tabs.Length)
            {
                _tabs[_currentTabIndex].OnTabHidden();
            }

            _currentTabIndex = tabIndex;

            _scrollTween?.Kill();

            float targetX = -tabIndex * _tabWidth;
            if (instant)
            {
                _content.anchoredPosition = new Vector2(targetX, _content.anchoredPosition.y);
            }
            else
            {
                _scrollTween = _content
                    .DOAnchorPosX(targetX, _scrollDuration)
                    .SetEase(_scrollEase)
                    .SetUpdate(true);
            }

            _tabs[_currentTabIndex].OnTabShown();
            UpdateTabButtonVisuals();
        }

        private void BindTabButtons()
        {
            if (_tabButtons == null)
            {
                return;
            }

            for (int i = 0; i < _tabButtons.Length; i++)
            {
                if (_tabButtons[i] == null)
                {
                    continue;
                }

                _tabButtons[i].Bind(this, GoToTab);
            }
        }

        private void UpdateTabButtonVisuals()
        {
            if (_tabButtons == null)
            {
                return;
            }

            for (int i = 0; i < _tabButtons.Length; i++)
            {
                if (_tabButtons[i] == null)
                {
                    continue;
                }

                bool isSelected = (int)_tabButtons[i].TargetTab == _currentTabIndex;
                _tabButtons[i].SetSelectedVisual(isSelected);
            }
        }

        public void SnapToNerestTab()
        {
            if (_tabs == null || _tabs.Length == 0)
            {
                return;
            }

            float currentX = _content.anchoredPosition.x;
            int nearestTabIndex = Mathf.RoundToInt(-currentX / _tabWidth);
            nearestTabIndex = Mathf.Clamp(nearestTabIndex, 0, _tabs.Length - 1);

            GoToTab(nearestTabIndex);
        }
    }
}
