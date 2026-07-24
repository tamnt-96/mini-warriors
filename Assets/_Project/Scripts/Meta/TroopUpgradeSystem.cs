using System.Collections.Generic;
using TWR.Data;
using TWR.Save;

namespace TWR.Meta
{
    public class TroopUpgradeSystem
    {
        readonly ProgressService     _progress;
        readonly TroopUpgradeTableSO _table;

        public TroopUpgradeSystem(ProgressService progress, TroopUpgradeTableSO table)
        {
            _progress = progress;
            _table    = table;
        }

        public int GetLevel(string warriorId) => FindSave(warriorId)?.level ?? 1;

        public bool IsMaxLevel(string warriorId) => GetLevel(warriorId) >= TroopUpgradeTableSO.MaxLevel;

        public bool TryGetUpgradeCost(string warriorId, out TroopLevelRequirement cost)
        {
            cost = default;
            var save = FindSave(warriorId);
            if (save == null || !save.isOwned) return false;
            return _table.TryGetRequirement(save.level + 1, out cost);
        }

        public bool CanUpgrade(string warriorId)
        {
            var save = FindSave(warriorId);
            if (save == null || !save.isOwned) return false;
            if (save.level >= TroopUpgradeTableSO.MaxLevel) return false;
            if (!_table.TryGetRequirement(save.level + 1, out var cost)) return false;
            return _progress.Data.gold >= cost.goldCost && save.pieces >= cost.fragmentCost;
        }

        public bool TryUpgrade(string warriorId)
        {
            var save = FindSave(warriorId);
            if (save == null || !save.isOwned) return false;
            if (save.level >= TroopUpgradeTableSO.MaxLevel) return false;
            if (!_table.TryGetRequirement(save.level + 1, out var cost)) return false;
            if (save.pieces < cost.fragmentCost) return false;
            if (!_progress.SpendGold(cost.goldCost)) return false;

            save.pieces -= cost.fragmentCost;
            save.level  += 1;
            _progress.Save();
            return true;
        }

        public float GetHpBonus(string warriorId)
        {
            var growth = _table.GetGrowthTable(warriorId);
            return growth == null ? 0f : growth.GetHpBonusAtLevel(GetLevel(warriorId));
        }

        public float GetAtkBonus(string warriorId)
        {
            var growth = _table.GetGrowthTable(warriorId);
            return growth == null ? 0f : growth.GetAtkBonusAtLevel(GetLevel(warriorId));
        }

        public List<int> GetUnlockedSkillIds(string warriorId)
        {
            var result = new List<int>();
            var growth = _table.GetGrowthTable(warriorId);
            growth?.CollectUnlockedSkillIds(GetLevel(warriorId), result);
            return result;
        }

        public List<int> GetUnlockedPassiveIds(string warriorId)
        {
            var result = new List<int>();
            var growth = _table.GetGrowthTable(warriorId);
            growth?.CollectUnlockedPassiveIds(GetLevel(warriorId), result);
            return result;
        }

        WarriorSaveData FindSave(string warriorId)
        {
            foreach (var ws in _progress.Data.ownedWarriors)
                if (ws.warriorId == warriorId) return ws;
            return null;
        }
    }
}
