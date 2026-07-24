using UnityEngine;
using TWR.Core;

namespace TWR.Battle
{
    public class CastleSystem : MonoBehaviour
    {
        BattleStateManager _battleManager;
        BattleRuntimeState _state;

        public float PlayerCastleHPRatio => _state != null ? _state.playerCastleHP / _state.stage.playerCastleMaxHP : 1f;
        public float EnemyCastleHPRatio  => _state != null ? _state.enemyCastleHP  / _state.stage.enemyCastleMaxHP  : 1f;

        public void Initialize(BattleStateManager manager)
        {
            _battleManager = manager;
            _state = manager.Runtime;
        }

        public void DamagePlayerCastle(float amount)
        {
            if (_state == null) return;
            _state.playerCastleHP = Mathf.Max(0f, _state.playerCastleHP - amount);
            if (_state.playerCastleHP <= 0f)
                _battleManager.TriggerDefeat();
        }

        public void DamageEnemyCastle(float amount)
        {
            if (_state == null) return;
            _state.enemyCastleHP = Mathf.Max(0f, _state.enemyCastleHP - amount);

            float ratio = _state.enemyCastleHP / _state.stage.enemyCastleMaxHP;

            if (!_state.wave70PctTriggered && ratio <= 0.70f)
            {
                _state.wave70PctTriggered = true;
                EventBus<Castle70PctEvent>.Publish(new Castle70PctEvent());
            }

            if (!_state.wave30PctTriggered && ratio <= 0.30f)
            {
                _state.wave30PctTriggered = true;
                EventBus<Castle30PctEvent>.Publish(new Castle30PctEvent());
            }

            if (_state.enemyCastleHP <= 0f)
                _battleManager.TriggerVictory();
        }
    }
}