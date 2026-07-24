using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using TWR.Data;

namespace TWR.Core.EditorTools
{
    // Bakes the GDD "troop_upgrading" sheet into a TroopUpgradeTableSO asset.
    // Only Archer had a designer-authored gold/fragment cost curve in the sheet;
    // per user decision that shared curve is reused as the cost-to-reach-level
    // table for every troop. HP/ATK growth and skill/passive unlock levels are
    // per-troop and taken verbatim from the sheet.
    public static class TroopUpgradeTableGenerator
    {
        const string AssetPath = "Assets/_Project/Resources/TroopUpgradeTable.asset";

        struct TroopDef
        {
            public string warriorId;
            public float  hpPerLevel;
            public float  atkPerLevel;
            public int[]  skillLevels;
            public int[]  skillIds;
            public int    passiveId;
        }

        static readonly TroopDef[] Troops =
        {
            new TroopDef { warriorId = "Archer",    hpPerLevel = 2,  atkPerLevel = 1, skillLevels = new[] { 10, 25 }, skillIds = new[] { 703, 704 }, passiveId = 723 },
            new TroopDef { warriorId = "Warrior",   hpPerLevel = 6,  atkPerLevel = 2, skillLevels = new[] { 10, 25 }, skillIds = new[] { 701, 702 }, passiveId = 722 },
            new TroopDef { warriorId = "Spearman",  hpPerLevel = 15, atkPerLevel = 3, skillLevels = new[] { 10, 25 }, skillIds = new[] { 705, 706 }, passiveId = 722 },
            new TroopDef { warriorId = "Lancer",    hpPerLevel = 6,  atkPerLevel = 2, skillLevels = new[] { 10, 25 }, skillIds = new[] { 708, 709 }, passiveId = 725 },
            new TroopDef { warriorId = "Alchemist", hpPerLevel = 8,  atkPerLevel = 3, skillLevels = new[] { 10, 25 }, skillIds = new[] { 710, 711 }, passiveId = 727 },
            new TroopDef { warriorId = "Shieldman", hpPerLevel = 11, atkPerLevel = 2, skillLevels = new[] { 10, 25 }, skillIds = new[] { 712, 713 }, passiveId = 728 },
            new TroopDef { warriorId = "Berserker", hpPerLevel = 8,  atkPerLevel = 3, skillLevels = new[] { 1, 10, 25 }, skillIds = new[] { 714, 715, 716 }, passiveId = 722 },
            new TroopDef { warriorId = "Witch",     hpPerLevel = 4,  atkPerLevel = 3, skillLevels = new[] { 10, 25 }, skillIds = new[] { 717, 718 }, passiveId = 722 },
            new TroopDef { warriorId = "Arbalist",  hpPerLevel = 4,  atkPerLevel = 2, skillLevels = new[] { 10, 25 }, skillIds = new[] { 719, 720 }, passiveId = 723 },
            new TroopDef { warriorId = "Lubu",      hpPerLevel = 8,  atkPerLevel = 2, skillLevels = new[] { 10, 25 }, skillIds = new[] { 721, 722 }, passiveId = 727 },
        };

        // Shared gold/fragment cost curve (levels 2..40), taken from Archer's row —
        // the only troop the designer filled in.
        static readonly int[] GoldCostByLevel =
        {
            /*2*/200, 400, 500, 600, 700, 800, 860, 920, 1000, 1100,
            /*12*/1200, 1300, 1400, 1500, 1600, 1700, 1800, 1900, 2000, 2200,
            /*22*/2400, 2600, 2800, 3000, 3200, 3400, 3600, 3800, 4000, 4200,
            /*32*/4400, 4600, 4800, 5000, 5200, 5400, 5600, 5800, 6000
        };

        static readonly int[] FragmentCostByLevel =
        {
            /*2*/2, 2, 2, 4, 4, 4, 4, 4, 6, 6,
            /*12*/6, 6, 6, 8, 8, 8, 8, 8, 10, 10,
            /*22*/10, 10, 10, 12, 12, 12, 12, 12, 14, 14,
            /*32*/14, 14, 16, 16, 16, 16, 16, 18
        };

        [MenuItem("TWR/Generate Troop Upgrade Table")]
        public static void Generate()
        {
            var asset = AssetDatabase.LoadAssetAtPath<TroopUpgradeTableSO>(AssetPath);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<TroopUpgradeTableSO>();
                AssetDatabase.CreateAsset(asset, AssetPath);
            }

            var requirements = new List<TroopLevelRequirement>();
            for (int level = 2; level <= TroopUpgradeTableSO.MaxLevel; level++)
            {
                int idx = level - 2;
                requirements.Add(new TroopLevelRequirement
                {
                    level        = level,
                    goldCost     = idx < GoldCostByLevel.Length ? GoldCostByLevel[idx] : 0,
                    fragmentCost = idx < FragmentCostByLevel.Length ? FragmentCostByLevel[idx] : 0
                });
            }
            asset.requirements = requirements.ToArray();

            var growthTables = new List<TroopGrowthTable>();
            foreach (var t in Troops)
            {
                var skillUnlocks = new TroopSkillUnlock[t.skillLevels.Length];
                for (int i = 0; i < t.skillLevels.Length; i++)
                    skillUnlocks[i] = new TroopSkillUnlock { level = t.skillLevels[i], skillId = t.skillIds[i] };

                var passiveUnlocks = new[]
                {
                    new TroopSkillUnlock { level = 5,  skillId = t.passiveId },
                    new TroopSkillUnlock { level = 15, skillId = t.passiveId },
                    new TroopSkillUnlock { level = 35, skillId = t.passiveId },
                };

                growthTables.Add(new TroopGrowthTable
                {
                    warriorId      = t.warriorId,
                    hpPerLevel     = t.hpPerLevel,
                    atkPerLevel    = t.atkPerLevel,
                    skillUnlocks   = skillUnlocks,
                    passiveUnlocks = passiveUnlocks
                });
            }
            asset.troops = growthTables.ToArray();

            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"TroopUpgradeTable generated at {AssetPath} with {asset.troops.Length} troops.");
        }
    }
}
