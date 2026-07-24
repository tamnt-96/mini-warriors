using TWR.Core;

namespace TWR.Save
{
    public class ProgressService
    {
        readonly SaveSystem _saveSystem;
        PlayerProgress _current;

        public PlayerProgress Data => _current;

        public ProgressService(SaveSystem saveSystem)
        {
            _saveSystem = saveSystem;
            _current = saveSystem.Load();
        }

        public void Save() => _saveSystem.Save(_current);

        public void AddGold(int amount)
        {
            _current.gold += amount;
            Save();
        }

        public bool SpendGold(int amount)
        {
            if (_current.gold < amount) return false;
            _current.gold -= amount;
            Save();
            return true;
        }

        public void AddKeys(int amount) { _current.keys += amount; Save(); }
        public void AddGems(int amount) { _current.gems += amount; Save(); }
    }
}