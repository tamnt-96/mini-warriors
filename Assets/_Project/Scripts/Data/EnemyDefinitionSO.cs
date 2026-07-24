using UnityEngine;

namespace TWR.Data
{
    [CreateAssetMenu(menuName = "TWR/EnemyDefinition", fileName = "Enemy_New")]
    public class EnemyDefinitionSO : ScriptableObject
    {
        public string         enemyId;
        public string         displayName;
        public WarriorFaction faction;
        public TargetPriority targetPriority;
        public float          baseHP;
        public float          baseATK;
        public float          baseRange;
        public float          attackSpeed;
        public float          moveSpeed;
        public int            expReward;
        public GameObject     prefab;
        public Sprite         icon;
    }
}