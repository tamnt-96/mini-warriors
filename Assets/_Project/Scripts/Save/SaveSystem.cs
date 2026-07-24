using System.IO;
using UnityEngine;

namespace TWR.Save
{
    public class SaveSystem
    {
        const string FileName = "player_progress.json";
        readonly string _savePath;

        public SaveSystem()
        {
            _savePath = Path.Combine(Application.persistentDataPath, FileName);
        }

        public void Save(PlayerProgress progress)
        {
            var json = JsonUtility.ToJson(progress, prettyPrint: true);
            File.WriteAllText(_savePath, json);
        }

        public PlayerProgress Load()
        {
            if (!File.Exists(_savePath))
                return PlayerProgress.Default();

            var json = File.ReadAllText(_savePath);
            return JsonUtility.FromJson<PlayerProgress>(json) ?? PlayerProgress.Default();
        }

        public void Delete()
        {
            if (File.Exists(_savePath))
                File.Delete(_savePath);
        }
    }
}