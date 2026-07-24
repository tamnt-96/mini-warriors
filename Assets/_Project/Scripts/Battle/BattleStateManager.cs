using UnityEngine;
using TWR.Core;
using TWR.Data;

namespace TWR.Battle
{
    public enum BattlePhase
    {
        Idle,
        TalentSelect,
        AutoBattle,
        LevelUpPause,
        VictoryDelay,
        ResultScreen
    }

    public class BattleStateManager : MonoBehaviour
    {
        public BattlePhase    CurrentPhase  { get; private set; } = BattlePhase.Idle;
        public BattleRuntimeState Runtime  { get; private set; }

        [SerializeField] float _victoryDelaySeconds = 1.5f;
        float _victoryTimer;

        public void StartBattle(StageDefinitionSO stage)
        {
            Runtime = new BattleRuntimeState();
            Runtime.Initialize(stage);
            EventBus<BattleStartedEvent>.Publish(new BattleStartedEvent { stage = stage });
            TransitionTo(BattlePhase.AutoBattle);
        }

        public void OnTalentSelectionComplete()
        {
            if (CurrentPhase != BattlePhase.TalentSelect &&
                CurrentPhase != BattlePhase.LevelUpPause) return;
            TransitionTo(BattlePhase.AutoBattle);
        }

        public void TriggerLevelUpPause()
        {
            if (CurrentPhase != BattlePhase.AutoBattle) return;
            TransitionTo(BattlePhase.LevelUpPause);
        }

        public void TriggerVictory()
        {
            if (CurrentPhase == BattlePhase.VictoryDelay ||
                CurrentPhase == BattlePhase.ResultScreen) return;
            EventBus<BattleVictoryEvent>.Publish(new BattleVictoryEvent());
            _victoryTimer = _victoryDelaySeconds;
            TransitionTo(BattlePhase.VictoryDelay);
        }

        public void TriggerDefeat()
        {
            if (CurrentPhase == BattlePhase.ResultScreen) return;
            EventBus<BattleDefeatEvent>.Publish(new BattleDefeatEvent());
            TransitionTo(BattlePhase.ResultScreen);
        }

        void Update()
        {
            if (CurrentPhase == BattlePhase.VictoryDelay)
            {
                _victoryTimer -= Time.deltaTime;
                if (_victoryTimer <= 0f)
                    TransitionTo(BattlePhase.ResultScreen);
            }
        }

        void TransitionTo(BattlePhase next)
        {
            var previous = CurrentPhase;
            CurrentPhase = next;
            EventBus<PhaseChangedEvent>.Publish(new PhaseChangedEvent { previous = previous, next = next });
        }
    }
}