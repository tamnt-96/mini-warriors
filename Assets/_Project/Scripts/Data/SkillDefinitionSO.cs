using UnityEngine;

namespace TWR.Data
{
    [CreateAssetMenu(menuName = "TWR/SkillDefinition", fileName = "Skill_New")]
    public class SkillDefinitionSO : ScriptableObject
    {
        public string     skillId;
        public string     displayName;
        public float      cooldown;
        public Sprite     icon;
        public GameObject effectPrefab;
    }
}