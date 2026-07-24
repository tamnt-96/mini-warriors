using System.Collections.Generic;
using UnityEngine;
using TWR.Core;
using TWR.Data;
using TWR.Meta;
using TWR.Save;

namespace TWR.Battle
{
    public class BattleCoordinator : MonoBehaviour
    {
        [Header("Stage")]
        [SerializeField] StageDefinitionSO _stageOverride;

        [Header("Systems")]
        [SerializeField] BattleStateManager _battleManager;
        [SerializeField] CastleSystem       _castleSystem;
        [SerializeField] SpawnSystem        _spawnSystem;
        [SerializeField] CombatSystem       _combatSystem;
        [SerializeField] ExpSystem          _expSystem;
        [SerializeField] TalentSystem       _talentSystem;
        [SerializeField] HeroSystem         _heroSystem;
        [SerializeField] BattleSpeedSystem  _speedSystem;

        Dictionary<string, WarriorDefinitionSO> _warriorDefsById;

        public void StartNewGame()
        {
            var stage = _stageOverride;
            if (stage == null)
            {
                Debug.LogError("BattleCoordinator: No stage assigned.");
                return;
            }

            _battleManager.StartBattle(stage);
            WireUpSystems(stage);
            RegisterDeckWarriors(stage);
            _spawnSystem.StartEnemyWaves(stage.normalWaves);

            if (ServiceLocator.TryGet<ProgressService>(out var progress) &&
                progress.Data.highestChapterCleared >= 2)
            {
                _speedSystem.IsSpeed15Unlocked = true;
            }
        }

        void WireUpSystems(StageDefinitionSO stage)
        {
            var buffSystem = new BuffSystem(_battleManager.Runtime);

            _castleSystem.Initialize(_battleManager);
            _spawnSystem.Initialize(_battleManager, _combatSystem, _castleSystem);
            _expSystem.Initialize(_battleManager);
            _talentSystem.Initialize(_battleManager, buffSystem);
            _heroSystem.Initialize(_battleManager, _combatSystem);
            // _heroSystem.DeployHero();
        }

        void RegisterDeckWarriors(StageDefinitionSO stage)
        {
            if (!ServiceLocator.TryGet<ProgressService>(out var progress)) return;
            ServiceLocator.TryGet<TroopUpgradeSystem>(out var troopUpgrade);

            var deck  = progress.Data.activeDeck;
            var state = _battleManager.Runtime;

            _warriorDefsById ??= BuildWarriorDefinitionLookup();

            foreach (var id in deck.warriorIds)
            {
                if (!_warriorDefsById.TryGetValue(id, out var def)) continue;
                var ws = new ActiveWarriorState(def);
                ws.isUnlocked = false;

                if (troopUpgrade != null)
                {
                    ws.currentHP  += troopUpgrade.GetHpBonus(id);
                    ws.currentATK += troopUpgrade.GetAtkBonus(id);
                }

                state.warriors.Add(ws);
            }
        }

        static Dictionary<string, WarriorDefinitionSO> BuildWarriorDefinitionLookup()
        {
            var defs = Resources.LoadAll<WarriorDefinitionSO>("Warriors");
            var map  = new Dictionary<string, WarriorDefinitionSO>(defs.Length);
            foreach (var def in defs)
            {
                if (def == null || string.IsNullOrEmpty(def.warriorId)) continue;
                map[def.warriorId] = def;
            }
            return map;
        }
    }
}