using System.Collections.Generic;
using TWR.Data;

namespace TWR.Battle
{
    public static class CounterSystem
    {
        static readonly Dictionary<WarriorFaction, WarriorFaction> _counters = new()
        {
            { WarriorFaction.Magic,    WarriorFaction.Tanker    },
            { WarriorFaction.Tanker,   WarriorFaction.Spearman  },
            { WarriorFaction.Spearman, WarriorFaction.Cavalry   },
            { WarriorFaction.Cavalry,  WarriorFaction.Ranged    },
            { WarriorFaction.Ranged,   WarriorFaction.Infantry  },
            { WarriorFaction.Infantry, WarriorFaction.Magic     },
        };

        const float CounterMultiplier = 1.5f;

        public static float GetDamageMultiplier(WarriorFaction attackerFaction, WarriorFaction defenderFaction)
        {
            if (_counters.TryGetValue(attackerFaction, out var countered) && countered == defenderFaction)
                return CounterMultiplier;
            return 1f;
        }

        public static bool Counters(WarriorFaction attacker, WarriorFaction defender)
        {
            return _counters.TryGetValue(attacker, out var countered) && countered == defender;
        }
    }
}