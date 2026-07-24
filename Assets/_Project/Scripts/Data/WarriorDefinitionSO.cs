using UnityEngine;

namespace TWR.Data
{
    public enum WarriorFaction { Infantry, Tanker, Ranged, Cavalry, Magic, Spearman }
    public enum WarriorRole    { Infantry, Tanker, Ranged, Cavalry, Magic, Spearman }
    public enum TargetPriority { Nearest, Rearmost, HighestATK, LargestCluster }

    [CreateAssetMenu(menuName = "TWR/WarriorDefinition", fileName = "Warrior_New")]
    public class WarriorDefinitionSO : ScriptableObject
    {
        public string               warriorId;
        public string               displayName;
        public WarriorFaction       faction;
        public WarriorRole          role;
        public TargetPriority       targetPriority;
        public float                baseHP;
        public float                baseATK;
        public float                baseRange;
        public float                baseAttackSpeed;
        public float                spawnCooldown;
        public TalentDefinitionSO[] talents;
        public WarriorDefinitionSO  evolvedForm;
        public GameObject           prefab;
        public GameObject           evolvedPrefab;
        public Sprite               icon;
    }
}