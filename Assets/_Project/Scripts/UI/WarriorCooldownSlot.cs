using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TWR.Battle;

namespace TWR.UI
{
    public class WarriorCooldownSlot : MonoBehaviour
    {
        [SerializeField] TMP_Text     _nameText;
        [SerializeField] Image        _cooldownFill;
        [SerializeField] TMP_Text     _timerText;
        [SerializeField] Image        _icon;
        [SerializeField] RectTransform _dimCover;

        ActiveWarriorState _ws;

        public void Setup(ActiveWarriorState ws)
        {
            _ws = ws;
            if (_nameText != null) _nameText.text = ws.def.displayName;
            if (_icon     != null) _icon.sprite   = ws.def.icon;
            Refresh();
        }

        public void Refresh()
        {
            if (_ws == null) return;

            float ratio = _ws.currentSpawnCooldown > 0f
                ? 1f - (_ws.spawnTimer / _ws.currentSpawnCooldown)
                : 1f;
            ratio = Mathf.Clamp01(ratio);

            if (_cooldownFill != null) _cooldownFill.fillAmount = ratio;
            if (_timerText    != null) _timerText.text = _ws.spawnTimer > 0f
                ? $"{_ws.spawnTimer:F1}s"
                : "Ready";

            if (_dimCover != null)
            {
                float remaining = 1f - ratio;
                _dimCover.localScale = new Vector3(1f, remaining, 1f);
                _dimCover.gameObject.SetActive(remaining > 0f);
            }
        }
    }
}
