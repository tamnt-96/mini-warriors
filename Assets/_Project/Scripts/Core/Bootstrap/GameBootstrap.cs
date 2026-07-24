using System.Collections.Generic;
using UnityEngine;
using TWR.Audio;
using TWR.Data;
using TWR.Localization;
using TWR.Meta;
using TWR.Save;

namespace TWR.Core
{
    public class GameBootstrap : MonoBehaviour
    {
        [SerializeField] string      _lobbySceneName = "Lobby";
        [SerializeField] string      _battleSceneName = "Battle";
        [SerializeField] List<string> _gachaWarriorPool = new();

        StateMachine _stateMachine;

        void Awake()
        {
            var saveSystem  = new SaveSystem();
            var progress    = new ProgressService(saveSystem);
            var deckBuilder = new DeckBuilderSystem(progress);
            var gacha       = new GachaSystem(progress, _gachaWarriorPool);

            var upgradeTable = Resources.Load<TroopUpgradeTableSO>("TroopUpgradeTable");
            var troopUpgrade = new TroopUpgradeSystem(progress, upgradeTable);

            ServiceLocator.Register<SaveSystem>(saveSystem);
            ServiceLocator.Register<ProgressService>(progress);
            ServiceLocator.Register<DeckBuilderSystem>(deckBuilder);
            ServiceLocator.Register<GachaSystem>(gacha);
            ServiceLocator.Register<TroopUpgradeSystem>(troopUpgrade);

            var localizationSettings = LocalizationSettings.LoadOrCreateDefaults();
            var localizationManager  = new LocalizationManager(localizationSettings);
            localizationManager.Initialize();
            ServiceLocator.Register<LocalizationManager>(localizationManager);

            var musicSource = gameObject.AddComponent<AudioSource>();
            var sfxSource   = gameObject.AddComponent<AudioSource>();
            var audioManager = new AudioManager();
            audioManager.Initialize(musicSource, sfxSource);
            ServiceLocator.Register<AudioManager>(audioManager);

            var hapticManager = new HapticManager();
            ServiceLocator.Register<HapticManager>(hapticManager);

            _stateMachine = new StateMachine();
            _stateMachine.AddState(new GameState_Init());
            _stateMachine.AddState(new GameState_Lobby(_lobbySceneName));
            _stateMachine.AddState(new GameState_Play(_battleSceneName));

            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            _stateMachine.ChangeState<GameState_Init>();
        }

        void Update()
        {
            _stateMachine.Update();
        }
    }
}
