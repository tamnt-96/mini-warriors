using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CoreKit.UI
{
    // Drop-in feedback layer for UI Buttons: click SFX, haptic vibration, and a press-scale animation.
    [RequireComponent(typeof(Button))]
    public class UIButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        [SerializeField] AudioClip _clickSfx;
        [SerializeField] bool _enableSfx = true;
        [SerializeField] bool _enableHaptics = true;
        [SerializeField] bool _enableAnimation = true;

        static AudioSource _sfxSource;

        Button _button;
        Transform _scaleTarget;
        Vector3 _originalScale;
        Coroutine _scaleRoutine;

        void Awake()
        {
            _button = GetComponent<Button>();
            _scaleTarget = transform;
            _originalScale = _scaleTarget.localScale;
            _button.onClick.AddListener(OnClicked);
        }

        void OnDestroy()
        {
            if (_button != null) _button.onClick.RemoveListener(OnClicked);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_enableAnimation || !_button.interactable) return;
            PlayScale(_originalScale * PressScale());
        }

        public void OnPointerUp(PointerEventData eventData) => ResetScale();
        public void OnPointerExit(PointerEventData eventData) => ResetScale();

        void ResetScale()
        {
            if (!_enableAnimation) return;
            PlayScale(_originalScale);
        }

        void OnClicked()
        {
            if (_enableSfx) PlaySfx();
            if (_enableHaptics) PlayHaptic();
        }

        static float PressScale() => UIButtonSettings.Instance != null ? UIButtonSettings.Instance.pressScale : 0.94f;
        static float AnimDuration() => UIButtonSettings.Instance != null ? UIButtonSettings.Instance.animDuration : 0.08f;

        void PlayScale(Vector3 target)
        {
            if (_scaleRoutine != null) StopCoroutine(_scaleRoutine);
            _scaleRoutine = StartCoroutine(ScaleRoutine(target));
        }

        IEnumerator ScaleRoutine(Vector3 target)
        {
            float duration = AnimDuration();
            var start = _scaleTarget.localScale;
            float time = 0f;
            while (time < duration)
            {
                time += Time.unscaledDeltaTime;
                _scaleTarget.localScale = Vector3.Lerp(start, target, duration > 0f ? time / duration : 1f);
                yield return null;
            }
            _scaleTarget.localScale = target;
            _scaleRoutine = null;
        }

        void PlaySfx()
        {
            var clip = _clickSfx != null ? _clickSfx : UIButtonSettings.Instance?.defaultClickSfx;
            if (clip == null) return;

            if (_sfxSource == null)
            {
                var go = new GameObject("[UIButton SFX]");
                Object.DontDestroyOnLoad(go);
                _sfxSource = go.AddComponent<AudioSource>();
                _sfxSource.playOnAwake = false;
            }
            _sfxSource.PlayOneShot(clip);
        }

        void PlayHaptic()
        {
            var settings = UIButtonSettings.Instance;
            if (settings != null && !settings.hapticsEnabled) return;
            Haptics.Vibrate();
        }
    }
}
