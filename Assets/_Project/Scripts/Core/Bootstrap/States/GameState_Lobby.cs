using UnityEngine.SceneManagement;

namespace TWR.Core
{
    public class GameState_Lobby : IState
    {
        readonly string _lobbySceneName;

        public GameState_Lobby(string lobbySceneName)
        {
            _lobbySceneName = lobbySceneName;
        }

        public void OnEnter()
        {
            SceneManager.LoadScene(_lobbySceneName, LoadSceneMode.Single);
        }

        public void OnUpdate() { }

        public void OnExit() { }
    }
}
