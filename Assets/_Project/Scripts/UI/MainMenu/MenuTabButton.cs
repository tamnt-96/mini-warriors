using UnityEngine;
using UnityEngine.UI;

namespace TWR
{
    [RequireComponent(typeof(Button))]
    public class MenuTabButton : MonoBehaviour
    {
        [SerializeField] private MenuTabType _targetTab;
        [SerializeField] private GameObject _selectedVisual;
        [SerializeField] private GameObject _unselectedVisual;

        private Button _button;
        private MainMenuController _mainMenuController;
        private System.Action<MenuTabType> _onClickCallback;

        public MenuTabType TargetTab => _targetTab;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(HandleClick);
        }

        private void OnDestroy()
        {
            if (_button != null)
            {
                _button.onClick.RemoveListener(HandleClick);
            }
        }

        public void Bind(MainMenuController mainMenuController, System.Action<MenuTabType> onClickCallback)
        {
            _mainMenuController = mainMenuController;
            _onClickCallback = onClickCallback;
        }

        public void SetSelectedVisual(bool isSelected)
        {
            if (_selectedVisual != null)
            {
                _selectedVisual.SetActive(isSelected);
            }

            if (_unselectedVisual != null)
            {
                _unselectedVisual.SetActive(!isSelected);
            }
        }

        private void HandleClick()
        {
            if (_onClickCallback != null)
            {
                _onClickCallback.Invoke(_targetTab);
                return;
            }

            _mainMenuController?.GoToTab(_targetTab);
        }

        
    }
}
