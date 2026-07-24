using TWR.Save;

namespace TWR.Meta
{
    public class WarriorProgressionSystem
    {
        const int PiecesToStar2 = 40;

        readonly ProgressService _progress;

        public WarriorProgressionSystem(ProgressService progress)
        {
            _progress = progress;
        }

        public bool TryUpgradeToStar2(string warriorId)
        {
            var save = FindSave(warriorId);
            if (save == null || !save.isOwned) return false;
            if (save.starLevel >= 2) return false;
            if (save.pieces < PiecesToStar2) return false;

            save.pieces    -= PiecesToStar2;
            save.starLevel  = 2;
            _progress.Save();
            return true;
        }

        public int GetPiecesForWarrior(string warriorId)
        {
            return FindSave(warriorId)?.pieces ?? 0;
        }

        public int GetStarLevel(string warriorId)
        {
            return FindSave(warriorId)?.starLevel ?? 0;
        }

        WarriorSaveData FindSave(string warriorId)
        {
            foreach (var ws in _progress.Data.ownedWarriors)
                if (ws.warriorId == warriorId) return ws;
            return null;
        }
    }
}