using UnityEngine;

namespace CoreKit.UI
{
    public abstract class BasePanel : MonoBehaviour
    {
        [SerializeField] bool _isPopup = true;

        public bool IsVisible => gameObject.activeSelf;
        public bool IsPopup => _isPopup;

        public void Show() => gameObject.SetActive(true);
        public void Hide() => gameObject.SetActive(false);

        // Tied to Unity's lifecycle so panels that start active also call OnShow on scene load.
        void OnEnable() => OnShow();
        void OnDisable() => OnHide();

        protected virtual void OnShow() { }
        protected virtual void OnHide() { }
    }
}
