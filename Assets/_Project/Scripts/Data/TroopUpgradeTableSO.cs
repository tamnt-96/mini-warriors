using System;
using UnityEngine;

namespace TWR.Data
{
    [Serializable]
    public struct TroopLevelRequirement
    {
        public int level;
        public int goldCost;
        public int fragmentCost;
    }

    [Serializable]
    public struct TroopSkillUnlock
    {
        public int level;
        public int skillId;
    }

    [Serializable]
    public class TroopGrowthTable
    {
        public string             warriorId;
        public float              hpPerLevel;
        public float              atkPerLevel;
        public TroopSkillUnlock[] skillUnlocks;
        public TroopSkillUnlock[] passiveUnlocks;

        public float GetHpBonusAtLevel(int level) => level <= 1 ? 0f : hpPerLevel * (level - 1);
        public float GetAtkBonusAtLevel(int level) => level <= 1 ? 0f : atkPerLevel * (level - 1);

        public void CollectUnlockedSkillIds(int level, System.Collections.Generic.List<int> result)
        {
            if (skillUnlocks == null) return;
            foreach (var u in skillUnlocks)
                if (u.level <= level) result.Add(u.skillId);
        }

        public void CollectUnlockedPassiveIds(int level, System.Collections.Generic.List<int> result)
        {
            if (passiveUnlocks == null) return;
            foreach (var u in passiveUnlocks)
                if (u.level <= level) result.Add(u.skillId);
        }
    }

    [CreateAssetMenu(menuName = "TWR/TroopUpgradeTable", fileName = "TroopUpgradeTable")]
    public class TroopUpgradeTableSO : ScriptableObject
    {
        public const int MaxLevel = 40;

        [Tooltip("Gold/fragment cost to reach each level (2..40), shared across every troop.")]
        public TroopLevelRequirement[] requirements;

        public TroopGrowthTable[] troops;

        public bool TryGetRequirement(int level, out TroopLevelRequirement requirement)
        {
            if (requirements != null)
            {
                foreach (var r in requirements)
                {
                    if (r.level == level)
                    {
                        requirement = r;
                        return true;
                    }
                }
            }
            requirement = default;
            return false;
        }

        public TroopGrowthTable GetGrowthTable(string warriorId)
        {
            if (troops == null) return null;
            foreach (var t in troops)
                if (t.warriorId == warriorId) return t;
            return null;
        }
    }
}
