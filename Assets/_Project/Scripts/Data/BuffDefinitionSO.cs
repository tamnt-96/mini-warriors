using UnityEngine;

namespace TWR.Data
{
    [CreateAssetMenu(menuName = "TWR/BuffDefinition", fileName = "Buff_New")]
    public class BuffDefinitionSO : ScriptableObject
    {
        public string         buffId;
        public string         displayName;
        public Sprite         icon;
        public WarriorFaction targetFaction;
        public bool           appliesToAllFactions;
        public StatType       statType;
        public float          value;
        public bool           isPercentage;
    }
}