using System.Collections.Generic;
using UnityEngine;
using TWR.Units;
using TWR.Data;

namespace TWR.Battle
{
    public class CombatSystem : MonoBehaviour
    {
        readonly List<UnitController> _playerUnits = new();
        readonly List<UnitController> _enemyUnits  = new();

        public IReadOnlyList<UnitController> PlayerUnits => _playerUnits;
        public IReadOnlyList<UnitController> EnemyUnits  => _enemyUnits;

        public void RegisterUnit(UnitController unit)
        {
            if (unit.IsPlayer) _playerUnits.Add(unit);
            else               _enemyUnits.Add(unit);
        }

        public void UnregisterUnit(UnitController unit)
        {
            if (unit.IsPlayer) _playerUnits.Remove(unit);
            else               _enemyUnits.Remove(unit);
        }

        public UnitController FindTarget(UnitController attacker, TargetPriority priority)
        {
            var targets = attacker.IsPlayer ? _enemyUnits : _playerUnits;
            return priority switch
            {
                TargetPriority.Nearest        => GetNearest(attacker.transform.position, targets),
                TargetPriority.Rearmost       => GetRearmost(attacker.IsPlayer, targets),
                TargetPriority.HighestATK     => GetHighestATK(targets),
                TargetPriority.LargestCluster => GetClusterCenter(attacker.transform.position, targets),
                _                             => GetNearest(attacker.transform.position, targets)
            };
        }

        UnitController GetNearest(Vector3 from, IReadOnlyList<UnitController> targets)
        {
            UnitController best = null;
            float bestDist = float.MaxValue;
            foreach (var t in targets)
            {
                if (!t.IsAlive) continue;
                float d = Vector3.Distance(from, t.transform.position);
                if (d < bestDist) { bestDist = d; best = t; }
            }
            return best;
        }

        UnitController GetRearmost(bool attackerIsPlayer, IReadOnlyList<UnitController> targets)
        {
            UnitController best = null;
            // Vertical layout: player moves up (+Y), enemy moves down (-Y).
            // Rearmost enemy = highest Y (furthest from player, near enemy castle).
            // Rearmost player unit = lowest Y (furthest from enemy, near player castle).
            float rearmost = attackerIsPlayer ? float.MinValue : float.MaxValue;
            foreach (var t in targets)
            {
                if (!t.IsAlive) continue;
                float y = t.transform.position.y;
                if (attackerIsPlayer ? y > rearmost : y < rearmost)
                {
                    rearmost = y;
                    best = t;
                }
            }
            return best;
        }

        UnitController GetHighestATK(IReadOnlyList<UnitController> targets)
        {
            UnitController best = null;
            float highest = float.MinValue;
            foreach (var t in targets)
            {
                if (!t.IsAlive) continue;
                if (t.CurrentATK > highest) { highest = t.CurrentATK; best = t; }
            }
            return best;
        }

        UnitController GetClusterCenter(Vector3 from, IReadOnlyList<UnitController> targets)
        {
            const float clusterRadius = 2f;
            UnitController best = null;
            int bestCount = -1;
            foreach (var candidate in targets)
            {
                if (!candidate.IsAlive) continue;
                int count = 0;
                foreach (var t in targets)
                    if (t.IsAlive && Vector3.Distance(candidate.transform.position, t.transform.position) <= clusterRadius)
                        count++;
                if (count > bestCount) { bestCount = count; best = candidate; }
            }
            return best;
        }

        void Update()
        {
            _playerUnits.RemoveAll(u => u == null || !u.IsAlive);
            _enemyUnits.RemoveAll(u => u == null || !u.IsAlive);
        }
    }
}