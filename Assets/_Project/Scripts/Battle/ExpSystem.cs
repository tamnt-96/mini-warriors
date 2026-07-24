using UnityEngine;
using TWR.Core;

namespace TWR.Battle
{
    public class ExpSystem : MonoBehaviour
    {
        BattleStateManager _battleManager;

        public void Initialize(BattleStateManager battleManager)
        {
            _battleManager = battleManager;
            EventBus<EnemyDiedEvent>.Subscribe(OnEnemyDied);
        }

        void OnDestroy()
        {
            EventBus<EnemyDiedEvent>.Unsubscribe(OnEnemyDied);
        }

        void OnEnemyDied(EnemyDiedEvent evt)
        {
            if (_battleManager.CurrentPhase != BattlePhase.AutoBattle) return;

            var state = _battleManager.Runtime;
            state.currentExp += evt.def.expReward;

            if (state.currentExp >= state.expToNextLevel)
            {
                state.currentExp      -= state.expToNextLevel;
                state.playerLevel     += 1;
                state.expToNextLevel   = CalculateExpThreshold(state.playerLevel);

                EventBus<PlayerLeveledUpEvent>.Publish(
                    new PlayerLeveledUpEvent { newLevel = state.playerLevel });
                _battleManager.TriggerLevelUpPause();
            }
        }

        static int CalculateExpThreshold(int level)
        {
            return 10 + (level - 1) * 5;
        }
    }
}