using UnityEngine;

namespace TWR.Data
{
    public enum TalentType { StatBoost, WarriorUnlock, SkillUnlock }
    public enum StatType   { ATK, HP, Range, AttackSpeed, SpawnInterval }

    [CreateAssetMenu(menuName = "TWR/TalentDefinition", fileName = "Talent_New")]
    public class TalentDefinitionSO : ScriptableObject
    {
        public string      talentId;
        public string      displayName;
        public Sprite      icon;
        public TalentType  type;
        public StatType    statType;
        public float       value;
        public bool        isPercentage;
        public string      targetWarriorId;
    }
}