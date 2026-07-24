using TWR.Battle;
using TWR.Core;
using TWR.Data;
using UnityEngine;

namespace TWR.Units
{
    public class EnemyController : UnitController
    {
        public EnemyDefinitionSO Definition { get; private set; }

        CombatSystem   _combatSystem;
        CastleSystem   _castleSystem;
        TargetPriority _targetPriority;

        [SerializeField] float _castleReachDamage = 50f;

        public void Initialize(EnemyDefinitionSO def, CombatSystem combatSystem, CastleSystem castleSystem)
        {
            Definition      = def;
            _combatSystem   = combatSystem;
            _castleSystem   = castleSystem;
            _targetPriority = def.targetPriority;
        }

        protected override UnitController FindTarget()
            => _combatSystem?.FindTarget(this, _targetPriority);

        protected override void MoveForward()
        {
            base.MoveForward();
            if (transform.position.y <= BattleConstants.PlayerCastleY)
                HitPlayerCastle();
        }

        void HitPlayerCastle()
        {
            _castleSystem?.DamagePlayerCastle(_castleReachDamage);
            State = UnitState.Dead;
            _combatSystem?.UnregisterUnit(this);
            ReturnToPool();
        }

        protected override void OnDeath()
        {
            EventBus<EnemyDiedEvent>.Publish(new EnemyDiedEvent
            {
                def      = Definition,
                position = transform.position
            });
            _combatSystem?.UnregisterUnit(this);
            Invoke(nameof(ReturnToPool), 0.5f);
        }
    }
}