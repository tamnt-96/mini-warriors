using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CoreKit.UI
{
    [AddComponentMenu("CoreKit/UI/UI Switch")]
    public class UISwitch : MonoBehaviour, IPointerClickHandler
    {
        [Header("Visuals")]
        [SerializeField] Image _offStateImage;
        [SerializeField] Image _onStateImage;
        [SerializeField] RectTransform _knob;

        [Header("Knob Position")]
        [SerializeField] float _offKnobAnchoredX = -26f;
        [SerializeField] float _onKnobAnchoredX = 26f;

        [Header("Animation")]
        [SerializeField] float _moveDuration = 0.18f;
        [SerializeField] float _fadeDuration = 0.14f;
        [SerializeField] Ease _moveEase = Ease.OutCubic;
        [SerializeField] Ease _fadeEase = Ease.Linear;

        [Header("State")]
        [SerializeField] bool _isOn = true;
        [SerializeField] bool _interactable = true;

        Tween _knobTween;
        Tween _offFadeTween;
        Tween _onFadeTween;

        public event Action<bool> ValueChanged;

        public bool IsOn => _isOn;

        void Awake()
        {
            ApplyStateImmediate();
        }

        void OnDisable()
        {
            KillTweens();
        }

        void OnDestroy()
        {
            KillTweens();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_interactable) return;
            SetIsOn(!_isOn);
        }

        public void SetInteractable(bool value)
        {
            _interactable = value;
        }

        public void SetIsOn(bool value)
        {
            SetValue(value, sendEvent: true);
        }

        public void SetIsOnWithoutNotify(bool value)
        {
            SetValue(value, sendEvent: false);
        }

        void SetValue(bool value, bool sendEvent)
        {
            if (_isOn == value)
            {
                ApplyStateImmediate();
                return;
            }

            _isOn = value;
            AnimateToState();

            if (sendEvent)
                ValueChanged?.Invoke(_isOn);
        }

        void ApplyStateImmediate()
        {
            KillTweens();

            if (_knob != null)
            {
                var anchored = _knob.anchoredPosition;
                anchored.x = _isOn ? _onKnobAnchoredX : _offKnobAnchoredX;
                _knob.anchoredPosition = anchored;
            }

            SetImageAlpha(_offStateImage, _isOn ? 0f : 1f);
            SetImageAlpha(_onStateImage, _isOn ? 1f : 0f);
        }

        void AnimateToState()
        {
            KillTweens();

            float targetKnobX = _isOn ? _onKnobAnchoredX : _offKnobAnchoredX;
            float targetOffAlpha = _isOn ? 0f : 1f;
            float targetOnAlpha = _isOn ? 1f : 0f;

            if (_knob != null)
                _knobTween = _knob.DOAnchorPosX(targetKnobX, _moveDuration).SetEase(_moveEase).SetUpdate(true);

            if (_offStateImage != null)
                _offFadeTween = _offStateImage.DOFade(targetOffAlpha, _fadeDuration).SetEase(_fadeEase).SetUpdate(true);

            if (_onStateImage != null)
                _onFadeTween = _onStateImage.DOFade(targetOnAlpha, _fadeDuration).SetEase(_fadeEase).SetUpdate(true);
        }

        void KillTweens()
        {
            _knobTween?.Kill();
            _offFadeTween?.Kill();
            _onFadeTween?.Kill();
            _knobTween = null;
            _offFadeTween = null;
            _onFadeTween = null;
        }

        static void SetImageAlpha(Image image, float alpha)
        {
            if (image == null) return;
            var color = image.color;
            color.a = alpha;
            image.color = color;
        }
    }
}