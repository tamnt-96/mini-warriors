using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace TWR.Core.EditorTools
{
    [InitializeOnLoad]
    public static class PlayFromBootstrap
    {
        const string BootstrapScenePath = "Assets/_Project/Scenes/Bootstrap.unity";

        static PlayFromBootstrap()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        [MenuItem("Tools/Mini Warriors/Play From Bootstrap #p")] // Shift + P
        static void Toggle()
        {
            if (EditorApplication.isPlaying)
            {
                EditorApplication.isPlaying = false;
                return;
            }

            var bootstrapScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(BootstrapScenePath);
            if (bootstrapScene == null)
            {
                Debug.LogError($"Bootstrap scene not found at {BootstrapScenePath}");
                return;
            }

            EditorSceneManager.playModeStartScene = bootstrapScene;
            EditorApplication.isPlaying = true;
        }

        static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredEditMode)
                EditorSceneManager.playModeStartScene = null;
        }
    }
}
