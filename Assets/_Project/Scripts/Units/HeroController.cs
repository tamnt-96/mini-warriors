using TWR.Battle;
using TWR.Core;
using TWR.Data;
using UnityEngine;

namespace TWR.Units
{
    public class HeroController : UnitController
    {
        CombatSystem _combatSystem;

        public void Initialize(CombatSystem combatSystem,
            float hp, float atk, float range, float atkSpeed,
            WarriorFaction faction)
        {
            _combatSystem = combatSystem;
            InitializeStats(hp, atk, range, atkSpeed, faction, isPlayer: true);
        }

        protected override UnitController FindTarget()
            => _combatSystem?.FindTarget(this, TargetPriority.Nearest);

        protected override void OnDeath()
        {
            EventBus<HeroDiedEvent>.Publish(new HeroDiedEvent());
            _combatSystem?.UnregisterUnit(this);
        }
    }
}