using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TWR.Core;
using TWR.Data;
using TWR.Units;

namespace TWR.Battle
{
    public class SpawnSystem : MonoBehaviour
    {
        [SerializeField] Transform _playerSpawnPoint;
        [SerializeField] Transform _enemySpawnPoint;

        BattleStateManager _battleManager;
        CombatSystem       _combatSystem;
        CastleSystem       _castleSystem;

        readonly Dictionary<string, ObjectPool> _unitPools = new();
        readonly Dictionary<GameObject, ObjectPool> _poolLookup = new();

        static readonly WaitForSeconds WaitBetweenWaves   = new(3f);
        static readonly WaitForSeconds WaitEmptyWaveFallback = new(5f);

        public void Initialize(BattleStateManager battleManager, CombatSystem combatSystem, CastleSystem castleSystem)
        {
            _battleManager = battleManager;
            _combatSystem  = combatSystem;
            _castleSystem  = castleSystem;

            EventBus<Castle70PctEvent>.Subscribe(OnCastle70Pct);
            EventBus<Castle30PctEvent>.Subscribe(OnCastle30Pct);
        }

        void OnDestroy()
        {
            EventBus<Castle70PctEvent>.Unsubscribe(OnCastle70Pct);
            EventBus<Castle30PctEvent>.Unsubscribe(OnCastle30Pct);
        }

        void Update()
        {
            if (_battleManager == null ||
                _battleManager.CurrentPhase != BattlePhase.AutoBattle) return;

            var state = _battleManager.Runtime;
            float dt = Time.deltaTime;

            foreach (var ws in state.warriors)
            {
                if (!ws.isUnlocked) continue;
                ws.spawnTimer -= dt;
                if (ws.spawnTimer <= 0f)
                {
                    ws.spawnTimer = ws.currentSpawnCooldown;
                    SpawnWarrior(ws);
                }
            }
        }

        void SpawnWarrior(ActiveWarriorState ws)
        {
            var def = ws.isEvolved && ws.def.evolvedForm != null ? ws.def.evolvedForm : ws.def;
            if (def.prefab == null) return;

            var pool    = GetOrCreatePool(def.prefab);
            var go      = pool.Get(_playerSpawnPoint.position, Quaternion.identity);
            var warrior = go.GetComponent<WarriorController>();
            if (warrior == null) return;

            warrior.Initialize(def, _combatSystem, _castleSystem);
            warrior.InitializeStats(
                ws.currentHP, ws.currentATK, ws.currentRange,
                ws.currentAttackSpeed, def.faction, isPlayer: true);

            _combatSystem.RegisterUnit(warrior);
        }

        public void StartEnemyWaves(WaveDefinitionSO[] normalWaves)
        {
            StartCoroutine(RunNormalWaves(normalWaves));
        }

        public void SpawnThresholdWave(WaveDefinitionSO wave)
        {
            if (wave != null) StartCoroutine(SpawnWave(wave));
        }

        IEnumerator RunNormalWaves(WaveDefinitionSO[] waves)
        {
            int waveNumber = 0;
            while (_battleManager.CurrentPhase != BattlePhase.VictoryDelay &&
                   _battleManager.CurrentPhase != BattlePhase.ResultScreen)
            {
                waveNumber++;
                EventBus<WaveStartedEvent>.Publish(new WaveStartedEvent { waveNumber = waveNumber });
                _battleManager.TriggerLevelUpPause();
                yield return new WaitUntil(
                    () => _battleManager.CurrentPhase == BattlePhase.AutoBattle ||
                          _battleManager.CurrentPhase == BattlePhase.VictoryDelay ||
                          _battleManager.CurrentPhase == BattlePhase.ResultScreen);

                if (_battleManager.CurrentPhase != BattlePhase.AutoBattle) yield break;

                if (waves == null || waves.Length == 0) { yield return WaitEmptyWaveFallback; continue; }
                yield return StartCoroutine(SpawnWave(waves[(waveNumber - 1) % waves.Length]));
                yield return WaitBetweenWaves;
            }
        }

        IEnumerator SpawnWave(WaveDefinitionSO wave)
        {
            foreach (var entry in wave.entries)
            {
                yield return new WaitForSeconds(entry.delayFromWaveStart);
                for (int i = 0; i < entry.count; i++)
                {
                    SpawnEnemy(entry.enemy);
                    if (i < entry.count - 1) yield return new WaitForSeconds(entry.spawnInterval);
                }
            }
        }

        void SpawnEnemy(EnemyDefinitionSO def)
        {
            if (def == null || def.prefab == null) return;

            var pool  = GetOrCreatePool(def.prefab);
            var go    = pool.Get(_enemySpawnPoint.position, Quaternion.identity);
            var enemy = go.GetComponent<EnemyController>();
            if (enemy == null) return;

            enemy.Initialize(def, _combatSystem, _castleSystem);
            enemy.InitializeStats(
                def.baseHP, def.baseATK, def.baseRange,
                def.attackSpeed, def.faction, isPlayer: false, def.moveSpeed);

            _combatSystem.RegisterUnit(enemy);
        }

        ObjectPool GetOrCreatePool(GameObject prefab)
        {
            var key = prefab.name;
            if (!_unitPools.TryGetValue(key, out var pool))
            {
                pool = new ObjectPool(prefab, transform, prewarm: 3);
                _unitPools[key] = pool;
            }
            return pool;
        }

        void OnCastle70Pct(Castle70PctEvent _)
        {
            SpawnThresholdWave(_battleManager.Runtime.stage.wave70Pct);
        }

        void OnCastle30Pct(Castle30PctEvent _)
        {
            SpawnThresholdWave(_battleManager.Runtime.stage.wave30Pct);
        }
    }
}