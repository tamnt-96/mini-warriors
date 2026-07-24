using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TWR.Battle;
using TWR.Core;
using TWR.Localization;
using TWR.Save;
using CoreKit.UI;

namespace TWR.UI
{
    public class BattleHUD : BasePanel
    {
        [Header("Systems")]
        [SerializeField] CastleSystem       _castleSystem;
        [SerializeField] BattleStateManager _battleManager;

        [Header("HUD Elements")]
        [SerializeField] Slider _playerHPSlider;
        [SerializeField] Slider _enemyHPSlider;
        [SerializeField] Slider _expSlider;

        [Header("Warrior Cooldown")]
        [SerializeField] Transform           _warriorCooldownContainer;
        [SerializeField] WarriorCooldownSlot _cooldownSlotPrefab;

        readonly List<WarriorCooldownSlot> _activeSlots = new();

        void Update()
        {
            if (_castleSystem == null || _battleManager == null) return;

            _playerHPSlider.value = _castleSystem.PlayerCastleHPRatio;
            _enemyHPSlider.value  = _castleSystem.EnemyCastleHPRatio;

            var runtime = _battleManager.Runtime;
            if (runtime != null && runtime.expToNextLevel > 0)
                _expSlider.value = (float)runtime.currentExp / runtime.expToNextLevel;

            foreach (var slot in _activeSlots)
                slot.Refresh();
        }

        protected override void OnShow()
        {
            EventBus<PhaseChangedEvent>.Subscribe(OnPhaseChanged);
            EventBus<BattleVictoryEvent>.Subscribe(OnVictory);
            EventBus<BattleDefeatEvent>.Subscribe(OnDefeat);
            EventBus<WarriorUnlockedEvent>.Subscribe(OnWarriorUnlocked);
        }

        protected override void OnHide()
        {
            EventBus<PhaseChangedEvent>.Unsubscribe(OnPhaseChanged);
            EventBus<BattleVictoryEvent>.Unsubscribe(OnVictory);
            EventBus<BattleDefeatEvent>.Unsubscribe(OnDefeat);
            EventBus<WarriorUnlockedEvent>.Unsubscribe(OnWarriorUnlocked);
        }

        void OnWarriorUnlocked(WarriorUnlockedEvent evt)
        {
            if (_cooldownSlotPrefab == null || _warriorCooldownContainer == null) return;

            if (_battleManager == null) return;
            var runtime = _battleManager.Runtime;
            if (runtime == null) return;

            var ws = runtime.warriors.Find(w => w.def == evt.warrior);
            if (ws == null) return;

            var slot = Instantiate(_cooldownSlotPrefab, _warriorCooldownContainer);
            slot.Setup(ws);
            _activeSlots.Add(slot);
        }

        void OnPhaseChanged(PhaseChangedEvent evt)
        {
            if (evt.next == BattlePhase.LevelUpPause)
                UIManager.Instance?.ShowPanel<TalentSelectionUI>();
            else
                UIManager.Instance?.HidePanel<TalentSelectionUI>();
        }

        void OnVictory(BattleVictoryEvent _)
        {
            if (ServiceLocator.TryGet<ProgressService>(out var progress))
            {
                var runtime = _battleManager != null ? _battleManager.Runtime : null;
                if (runtime != null && runtime.stage != null)
                {
                    progress.AddGold(runtime.stage.rewards.gold);
                    progress.AddKeys(runtime.stage.rewards.keys);
                }
            }

            var message = "Victory!";
            if (ServiceLocator.TryGet<LocalizationManager>(out var localization))
                message = localization.Get("ui.battle.result.victory");

            UIManager.Instance?.ShowPanel<ResultScreenUI>()?.Setup(message);
        }

        void OnDefeat(BattleDefeatEvent _)
        {
            var message = "Defeat";
            if (ServiceLocator.TryGet<LocalizationManager>(out var localization))
                message = localization.Get("ui.battle.result.defeat");

            UIManager.Instance?.ShowPanel<ResultScreenUI>()?.Setup(message);
        }

        public void OnSettingsPressed()
        {
            UIManager.Instance?.ShowPanel<SettingsView>();
        }
    }
}
