using TWR.Battle;
using UnityEngine;

namespace TWR.Units.Behaviors
{
    public class MagicController : WarriorController
    {
        [SerializeField] float _aoeRadius = 2f;

        protected override void PerformAttack(UnitController target)
        {
            var combatSystem = FindFirstObjectByType<CombatSystem>();
            if (combatSystem == null) { base.PerformAttack(target); return; }

            float multiplier = CounterSystem.GetDamageMultiplier(Faction, target.Faction);
            float damage     = _currentATK * multiplier;

            foreach (var enemy in combatSystem.EnemyUnits)
            {
                if (!enemy.IsAlive) continue;
                if (Vector3.Distance(target.transform.position, enemy.transform.position) <= _aoeRadius)
                    enemy.TakeDamage(damage);
            }
        }
    }
}
