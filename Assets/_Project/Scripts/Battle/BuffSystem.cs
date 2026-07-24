using TWR.Core;
using TWR.Data;

namespace TWR.Battle
{
    public class BuffSystem
    {
        readonly BattleRuntimeState _state;

        public BuffSystem(BattleRuntimeState state)
        {
            _state = state;
        }

        public void ApplyBuff(BuffDefinitionSO buff)
        {
            _state.buffs.Add(new ActiveBuffState { def = buff, value = buff.value });

            foreach (var ws in _state.warriors)
            {
                if (!ws.isUnlocked) continue;
                if (!buff.appliesToAllFactions && ws.def.faction != buff.targetFaction) continue;
                ApplyStatBoost(ws, buff.statType, buff.value, buff.isPercentage);
            }
        }

        static void ApplyStatBoost(ActiveWarriorState ws, StatType stat, float value, bool isPercent)
        {
            float multiplier = isPercent ? (1f + value / 100f) : 1f;
            float flat       = isPercent ? 0f : value;

            switch (stat)
            {
                case StatType.ATK:           ws.currentATK           = ws.currentATK * multiplier + flat; break;
                case StatType.HP:            ws.currentHP            = ws.currentHP  * multiplier + flat; break;
                case StatType.Range:         ws.currentRange         = ws.currentRange * multiplier + flat; break;
                case StatType.AttackSpeed:   ws.currentAttackSpeed   = ws.currentAttackSpeed * multiplier + flat; break;
                case StatType.SpawnInterval: ws.currentSpawnCooldown = ws.currentSpawnCooldown * (isPercent ? (1f - value / 100f) : 1f) - flat; break;
            }
        }
    }
}