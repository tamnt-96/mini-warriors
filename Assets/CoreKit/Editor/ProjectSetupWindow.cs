using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace CoreKit.Editor
{
    public class ProjectSetupWindow : EditorWindow
    {
        [MenuItem("CoreKit/Project Setup")]
        public static void OpenWindow()
        {
            var window = GetWindow<ProjectSetupWindow>("CoreKit - Project Setup");
            window.minSize = new Vector2(300, 120);
        }

        private void OnGUI()
        {
            GUILayout.Space(10);
            GUILayout.Label("Project Setup", EditorStyles.boldLabel);
            GUILayout.Space(6);

            if (GUILayout.Button("Setup New Project", GUILayout.Height(36)))
                SetupProject();
        }

        private static void SetupProject()
        {
            CreateFolder("Assets", "_Project");
            CreateFolder("Assets/_Project", "Scenes");
            CreateScene("Assets/_Project/Scenes", "Bootstrap");
            CreateScene("Assets/_Project/Scenes", "Gameplay");
            AssetDatabase.Refresh();
            Debug.Log("[CoreKit] Project setup complete!");
        }

        private static void CreateFolder(string parent, string folderName)
        {
            string path = $"{parent}/{folderName}";
            if (!AssetDatabase.IsValidFolder(path))
                AssetDatabase.CreateFolder(parent, folderName);
        }

        private static void CreateScene(string folderPath, string sceneName)
        {
            string scenePath = $"{folderPath}/{sceneName}.unity";
            string fullPath = Path.Combine(Application.dataPath.Replace("Assets", ""), scenePath);
            if (File.Exists(fullPath))
                return;

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
            EditorSceneManager.SaveScene(scene, scenePath);
            EditorSceneManager.CloseScene(scene, true);
        }
    }
}
