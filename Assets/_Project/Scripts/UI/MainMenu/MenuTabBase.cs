using UnityEngine;
using UnityEngine.UI;

namespace TWR
{
    [RequireComponent(typeof(LayoutElement))]
    public class MenuTabBase : MonoBehaviour
    {
        public virtual void OnTabShown() { }
        public virtual void OnTabHidden() { }

        private LayoutElement _layoutElement;

        private void Awake()
        {
            _layoutElement = GetComponent<LayoutElement>();
        }

        public void SetTabPreferredWidth(float width)
        {
            if (_layoutElement != null)
            {
                _layoutElement.preferredWidth = width;
            }
        }
    }
}
