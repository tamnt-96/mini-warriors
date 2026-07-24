using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.ShortcutManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TWR.Core.EditorTools
{
    public static class DevSceneShortcuts
    {
        const string BootstrapScenePath = "Assets/_Project/Scenes/Bootstrap.unity";
        const string LobbyScenePath = "Assets/_Project/Scenes/Lobby.unity";
        const string BattleScenePath = "Assets/_Project/Scenes/Battle.unity";

        [Shortcut("Mini Warriors/Switch To Bootstrap Scene", KeyCode.Alpha1)]
        static void SwitchToBootstrap() => SwitchScene(BootstrapScenePath, "Bootstrap");

        [Shortcut("Mini Warriors/Switch To Lobby Scene", KeyCode.Alpha2)]
        static void SwitchToLobby() => SwitchScene(LobbyScenePath, "Lobby");

        [Shortcut("Mini Warriors/Switch To Battle Scene", KeyCode.Alpha3)]
        static void SwitchToBattle() => SwitchScene(BattleScenePath, "Battle");

        static void SwitchScene(string scenePath, string sceneName)
        {
            if (Application.isPlaying)
            {
                return;
            }

            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }

            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        }
    }
}
