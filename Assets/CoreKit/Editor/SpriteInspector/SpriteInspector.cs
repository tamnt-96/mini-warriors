using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.AssetImporters;

namespace CoreKit.Editor
{
    [CustomEditor(typeof(TextureImporter))]
    [CanEditMultipleObjects]
    public class SpriteInspector : UnityEditor.Editor
    {
        const string ShowToolsSessionKey = "CoreKit.Editor.SpriteInspector.ShowTools";

        static readonly Type DefaultInspectorType = Type.GetType("UnityEditor.TextureImporterInspector, UnityEditor.CoreModule");

        // AssetImporterEditor only enables its Apply/Revert bar (needsApplyRevert) when Unity's
        // native ActiveEditorTracker wires up a companion editor for the imported asset via this
        // internal method. CreateEditor() alone never calls it, so we replicate the wiring by hand.
        static readonly MethodInfo SetAssetImporterTargetEditorMethod =
            typeof(AssetImporterEditor).GetMethod("InternalSetAssetImporterTargetEditor", BindingFlags.NonPublic | BindingFlags.Instance);

        // AssetImporterEditor.OnDisable() logs "OnEnable must call base.OnEnable" if this flag isn't
        // true. It's only ever used for that diagnostic pair, but constructing/destroying the nested
        // editor manually (outside ActiveEditorTracker's native OnEnable/InternalSetAssetImporterTargetEditor
        // ordering) leaves it unset, so we set it directly to suppress the false warning.
        static readonly FieldInfo OnEnableCalledField =
            typeof(AssetImporterEditor).GetField("m_OnEnableCalled", BindingFlags.NonPublic | BindingFlags.Instance);

        static readonly ISpriteInspectorFeature[] Features =
        {
            new SpriteCornerCropFeature(),
            new SpriteBorderCropFeature(),
            new SpriteTrimFeature(),
        };

        UnityEditor.Editor _defaultEditor;
        UnityEditor.Editor _assetEditor;

        void OnEnable()
        {
            DestroyNestedEditors();

            _defaultEditor = CreateEditor(targets, DefaultInspectorType);
            OnEnableCalledField?.SetValue(_defaultEditor, true);

            var mainAssets = targets
                .OfType<TextureImporter>()
                .Select(importer => AssetDatabase.LoadMainAssetAtPath(importer.assetPath))
                .Where(asset => asset != null)
                .ToArray();

            if (mainAssets.Length == targets.Length)
            {
                _assetEditor = CreateEditor(mainAssets);
                SetAssetImporterTargetEditorMethod?.Invoke(_defaultEditor, new object[] { _assetEditor });
            }
        }

        void OnDisable()
        {
            DestroyNestedEditors();
        }

        void DestroyNestedEditors()
        {
            if (_defaultEditor != null)
                DestroyImmediate(_defaultEditor);

            if (_assetEditor != null)
                DestroyImmediate(_assetEditor);

            _defaultEditor = null;
            _assetEditor = null;
        }

        public override void OnInspectorGUI()
        {
            if (target is TextureImporter importer)
            {
                var applicableFeatures = Features.Where(f => f.IsApplicable(importer)).ToArray();
                if (applicableFeatures.Length > 0)
                {
                    var showTools = SessionState.GetBool(ShowToolsSessionKey, true);
                    var newShowTools = EditorGUILayout.BeginFoldoutHeaderGroup(showTools, "Custom Tools");
                    if (newShowTools != showTools)
                        SessionState.SetBool(ShowToolsSessionKey, newShowTools);

                    if (newShowTools)
                    {
                        EditorGUI.indentLevel++;
                        foreach (var feature in applicableFeatures)
                            feature.OnGUI(importer);
                        EditorGUI.indentLevel--;
                    }

                    EditorGUILayout.EndFoldoutHeaderGroup();
                    EditorGUILayout.Space();
                }
            }

            _defaultEditor.OnInspectorGUI();
        }
    }
}
