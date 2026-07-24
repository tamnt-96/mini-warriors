using UnityEditor;

namespace CoreKit.Editor
{
    public interface ISpriteInspectorFeature
    {
        bool IsApplicable(TextureImporter importer);
        void OnGUI(TextureImporter importer);
    }
}
