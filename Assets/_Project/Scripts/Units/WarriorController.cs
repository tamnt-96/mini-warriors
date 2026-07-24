using TWR.Battle;
using TWR.Core;
using TWR.Data;
using UnityEngine;

namespace TWR.Units
{
    public class WarriorController : UnitController
    {
        public WarriorDefinitionSO Definition { get; private set; }

        protected CombatSystem _combatSystem;
        protected CastleSystem _castleSystem;
        TargetPriority         _targetPriority;

        [SerializeField] float _castleReachDamage = 50f;

        public void Initialize(WarriorDefinitionSO def, CombatSystem combatSystem, CastleSystem castleSystem)
        {
            Definition      = def;
            _combatSystem   = combatSystem;
            _castleSystem   = castleSystem;
            _targetPriority = def != null ? def.targetPriority : TargetPriority.Nearest;
        }

        protected override UnitController FindTarget()
            => _combatSystem?.FindTarget(this, _targetPriority);

        protected override void MoveForward()
        {
            base.MoveForward();
            if (transform.position.y >= BattleConstants.EnemyCastleY)
                HitEnemyCastle();
        }

        void HitEnemyCastle()
        {
            _castleSystem?.DamageEnemyCastle(_castleReachDamage);
            State = UnitState.Dead;
            _combatSystem?.UnregisterUnit(this);
            ReturnToPool();
        }

        protected override void OnDeath()
        {
            if (Definition != null)
                EventBus<WarriorDiedEvent>.Publish(new WarriorDiedEvent { def = Definition });
            _combatSystem?.UnregisterUnit(this);
            Invoke(nameof(ReturnToPool), 0.5f);
        }
    }
}