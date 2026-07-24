using System;
using UnityEngine;

namespace TWR.Data
{
    [CreateAssetMenu(menuName = "TWR/WaveDefinition", fileName = "Wave_New")]
    public class WaveDefinitionSO : ScriptableObject
    {
        public WaveSpawnEntry[] entries;
    }

    [Serializable]
    public struct WaveSpawnEntry
    {
        public EnemyDefinitionSO enemy;
        public int               count;
        public float             spawnInterval;
        public float             delayFromWaveStart;
    }
}