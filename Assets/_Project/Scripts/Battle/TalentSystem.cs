using System.Collections.Generic;
using UnityEngine;
using TWR.Core;
using TWR.Data;

namespace TWR.Battle
{
    public class TalentCardOption
    {
        public TalentDefinitionSO  talent;            // null when isWarriorSelection
        public WarriorDefinitionSO warrior;
        public bool                isWarriorSelection;
    }

    public class TalentSystem : MonoBehaviour
    {
        const int CardCount = 3;

        BattleStateManager _battleManager;
        BuffSystem         _buffSystem;
        int                _waveNumber;

        public List<TalentCardOption> CurrentOptions { get; } = new();

        public void Initialize(BattleStateManager battleManager, BuffSystem buffSystem)
        {
            _battleManager = battleManager;
            _buffSystem    = buffSystem;
            EventBus<WaveStartedEvent>.Subscribe(OnWaveStarted);
            EventBus<PlayerLeveledUpEvent>.Subscribe(OnPlayerLeveledUp);
        }

        void OnDestroy()
        {
            EventBus<WaveStartedEvent>.Unsubscribe(OnWaveStarted);
            EventBus<PlayerLeveledUpEvent>.Unsubscribe(OnPlayerLeveledUp);
        }

        void OnWaveStarted(WaveStartedEvent evt)
        {
            _waveNumber = evt.waveNumber;
            CurrentOptions.Clear();

            if (_waveNumber == 1)
                GenerateWarriorSelectOptions();
            else
                GenerateOptions();
        }

        void OnPlayerLeveledUp(PlayerLeveledUpEvent evt)
        {
            CurrentOptions.Clear();
            GenerateOptions();
        }

        void GenerateWarriorSelectOptions()
        {
            var state   = _battleManager.Runtime;
            var locked  = new List<ActiveWarriorState>();

            foreach (var ws in state.warriors)
                if (!ws.isUnlocked) locked.Add(ws);

            Shuffle(locked);

            for (int i = 0; i < Mathf.Min(CardCount, locked.Count); i++)
                CurrentOptions.Add(new TalentCardOption
                {
                    warrior            = locked[i].def,
                    isWarriorSelection = true
                });
        }

        void GenerateOptions()
        {
            var state    = _battleManager.Runtime;
            var pool     = BuildCardPool(state);
            var shuffled = Shuffle(pool);

            for (int i = 0; i < Mathf.Min(CardCount, shuffled.Count); i++)
                CurrentOptions.Add(shuffled[i]);
        }

        List<TalentCardOption> BuildCardPool(BattleRuntimeState state)
        {
            var pool = new List<TalentCardOption>();
            foreach (var ws in state.warriors)
            {
                if (!ws.isUnlocked) continue;
                if (ws.def.talents == null) continue;
                int nextTalentIdx = ws.talentsPicked;
                if (nextTalentIdx >= ws.def.talents.Length) continue;

                pool.Add(new TalentCardOption
                {
                    talent  = ws.def.talents[nextTalentIdx],
                    warrior = ws.def
                });
            }
            return pool;
        }

        public void PickOption(int index)
        {
            if (index < 0 || index >= CurrentOptions.Count) return;

            var option = CurrentOptions[index];

            if (option.isWarriorSelection)
                UnlockWarrior(option);
            else
                ApplyTalent(option);

            _battleManager.OnTalentSelectionComplete();
        }

        void UnlockWarrior(TalentCardOption option)
        {
            var state = _battleManager.Runtime;
            var ws    = state.warriors.Find(w => w.def == option.warrior);
            if (ws == null) return;

            ws.isUnlocked = true;
            ws.spawnTimer = 0f;
            EventBus<WarriorUnlockedEvent>.Publish(
                new WarriorUnlockedEvent { warrior = option.warrior });
        }

        void ApplyTalent(TalentCardOption option)
        {
            var state = _battleManager.Runtime;
            var ws    = state.warriors.Find(w => w.def == option.warrior);
            if (ws == null) return;

            ws.talentsPicked++;

            switch (option.talent.type)
            {
                case TalentType.WarriorUnlock:
                    if (!ws.isUnlocked)
                    {
                        ws.isUnlocked = true;
                        ws.spawnTimer = 0f;
                        EventBus<WarriorUnlockedEvent>.Publish(
                            new WarriorUnlockedEvent { warrior = option.warrior });
                    }
                    break;

                case TalentType.StatBoost:
                    ApplyStatToWarrior(ws, option.talent);
                    break;

                case TalentType.SkillUnlock:
                    break;
            }

            EventBus<TalentPickedEvent>.Publish(
                new TalentPickedEvent { talent = option.talent, warrior = option.warrior });

            CheckEvolution(ws);
        }

        void ApplyStatToWarrior(ActiveWarriorState ws, TalentDefinitionSO talent)
        {
            float value = talent.value;
            bool  isPct = talent.isPercentage;
            float mult  = isPct ? (1f + value / 100f) : 1f;
            float flat  = isPct ? 0f : value;

            switch (talent.statType)
            {
                case StatType.ATK:           ws.currentATK           = ws.currentATK * mult + flat; break;
                case StatType.HP:            ws.currentHP            = ws.currentHP  * mult + flat; break;
                case StatType.Range:         ws.currentRange         = ws.currentRange * mult + flat; break;
                case StatType.AttackSpeed:   ws.currentAttackSpeed   = ws.currentAttackSpeed * mult + flat; break;
                case StatType.SpawnInterval: ws.currentSpawnCooldown = Mathf.Max(0.5f, ws.currentSpawnCooldown - flat); break;
            }
        }

        void CheckEvolution(ActiveWarriorState ws)
        {
            if (ws.isEvolved) return;
            if (ws.def.evolvedForm == null) return;
            if (ws.def.talents == null) return;
            if (ws.talentsPicked < ws.def.talents.Length) return;

            ws.isEvolved = true;
            EventBus<WarriorEvolvedEvent>.Publish(
                new WarriorEvolvedEvent { newForm = ws.def.evolvedForm });
        }

        static List<T> Shuffle<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
            return list;
        }
    }
}
