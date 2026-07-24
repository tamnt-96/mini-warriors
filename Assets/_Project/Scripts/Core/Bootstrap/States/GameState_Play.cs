using UnityEngine;
using UnityEngine.SceneManagement;
using TWR.Battle;

namespace TWR.Core
{
    public class GameState_Play : IState
    {
        readonly string _battleSceneName;

        public GameState_Play(string battleSceneName)
        {
            _battleSceneName = battleSceneName;
        }

        public void OnEnter()
        {
            SceneManager.sceneLoaded += OnBattleSceneLoaded;
            SceneManager.LoadScene(_battleSceneName, LoadSceneMode.Single);
        }

        void OnBattleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name != _battleSceneName) return;

            SceneManager.sceneLoaded -= OnBattleSceneLoaded;

            var coordinator = Object.FindFirstObjectByType<BattleCoordinator>();
            if (coordinator != null)
                coordinator.StartNewGame();
            else
                Debug.LogError("GameState_Play: No BattleCoordinator found in battle scene.");
        }

        public void OnUpdate() { }

        public void OnExit() { }
    }
}
