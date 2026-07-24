using System.Collections.Generic;
using TWR.Data;
using TWR.Save;

namespace TWR.Meta
{
    public class DeckBuilderSystem
    {
        public const int MaxWarriorSlots = 4;
        public const int MaxSkillSlots   = 2;

        readonly ProgressService _progress;

        public IReadOnlyList<string> WarriorIds => _progress.Data.activeDeck.warriorIds;
        public IReadOnlyList<string> SkillIds   => _progress.Data.activeDeck.skillIds;

        public DeckBuilderSystem(ProgressService progress)
        {
            _progress = progress;
        }

        public bool EquipWarrior(string warriorId)
        {
            var ids = new List<string>(_progress.Data.activeDeck.warriorIds);
            if (ids.Contains(warriorId)) return false;
            if (ids.Count >= MaxWarriorSlots) return false;
            if (!IsWarriorOwned(warriorId)) return false;

            ids.Add(warriorId);
            _progress.Data.activeDeck.warriorIds = ids.ToArray();
            _progress.Save();
            return true;
        }

        public bool UnequipWarrior(string warriorId)
        {
            var ids = new List<string>(_progress.Data.activeDeck.warriorIds);
            if (!ids.Remove(warriorId)) return false;
            _progress.Data.activeDeck.warriorIds = ids.ToArray();
            _progress.Save();
            return true;
        }

        public bool EquipSkill(string skillId)
        {
            var ids = new List<string>(_progress.Data.activeDeck.skillIds);
            if (ids.Contains(skillId)) return false;
            if (ids.Count >= MaxSkillSlots) return false;

            ids.Add(skillId);
            _progress.Data.activeDeck.skillIds = ids.ToArray();
            _progress.Save();
            return true;
        }

        public bool UnequipSkill(string skillId)
        {
            var ids = new List<string>(_progress.Data.activeDeck.skillIds);
            if (!ids.Remove(skillId)) return false;
            _progress.Data.activeDeck.skillIds = ids.ToArray();
            _progress.Save();
            return true;
        }

        bool IsWarriorOwned(string warriorId)
        {
            foreach (var ws in _progress.Data.ownedWarriors)
                if (ws.warriorId == warriorId && ws.isOwned) return true;
            return false;
        }
    }
}