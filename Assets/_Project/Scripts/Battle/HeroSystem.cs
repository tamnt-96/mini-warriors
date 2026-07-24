using UnityEngine;
using TWR.Core;
using TWR.Data;
using TWR.Units;

namespace TWR.Battle
{
    public class HeroSystem : MonoBehaviour
    {
        [SerializeField] GameObject    _heroPrefab;
        [SerializeField] Transform     _heroSpawnPoint;
        [SerializeField] float         _heroHP          = 500f;
        [SerializeField] float         _heroATK         = 80f;
        [SerializeField] float         _heroRange       = 1.5f;
        [SerializeField] float         _heroAttackSpeed = 1f;
        [SerializeField] WarriorFaction _heroFaction     = WarriorFaction.Infantry;

        BattleStateManager _battleManager;
        CombatSystem       _combatSystem;
        HeroController     _heroInstance;

        public bool CanRevive => _battleManager != null &&
                                 !_battleManager.Runtime.heroReviveUsed;

        public void Initialize(BattleStateManager battleManager, CombatSystem combatSystem)
        {
            _battleManager = battleManager;
            _combatSystem  = combatSystem;
            EventBus<HeroDiedEvent>.Subscribe(OnHeroDied);
        }

        void OnDestroy()
        {
            EventBus<HeroDiedEvent>.Unsubscribe(OnHeroDied);
        }

        public void DeployHero()
        {
            if (_heroInstance != null && _heroInstance.IsAlive) return;
            if (_heroPrefab == null) return;

            var go = Instantiate(_heroPrefab, _heroSpawnPoint.position, Quaternion.identity);
            _heroInstance = go.GetComponent<HeroController>();
            if (_heroInstance == null) return;

            _heroInstance.Initialize(_combatSystem, _heroHP, _heroATK, _heroRange, _heroAttackSpeed, _heroFaction);
            _combatSystem.RegisterUnit(_heroInstance);
        }

        public void ReviveHero()
        {
            if (!CanRevive) return;
            _battleManager.Runtime.heroReviveUsed = true;
            DeployHero();
        }

        void OnHeroDied(HeroDiedEvent _)
        {
            _battleManager.Runtime.heroAlive = false;
        }
    }
}