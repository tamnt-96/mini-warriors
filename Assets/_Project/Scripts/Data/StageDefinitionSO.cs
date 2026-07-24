using System;
using UnityEngine;

namespace TWR.Data
{
    [CreateAssetMenu(menuName = "TWR/StageDefinition", fileName = "Stage_New")]
    public class StageDefinitionSO : ScriptableObject
    {
        public string             stageId;
        public int                chapter;
        public int                stageIndex;
        public int                energyCost;
        public float              enemyCastleMaxHP;
        public float              playerCastleMaxHP;
        public WaveDefinitionSO[] normalWaves;
        public WaveDefinitionSO   wave70Pct;
        public WaveDefinitionSO   wave30Pct;
        public RewardConfig       rewards;
    }

    [Serializable]
    public struct RewardConfig
    {
        public int gold;
        public int keys;
    }
}