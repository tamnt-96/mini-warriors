using System.Collections.Generic;
using UnityEngine;
using TWR.Save;

namespace TWR.Meta
{
    public class ChestOpenResult
    {
        public string       warriorId;
        public int          piecesGained;
        public bool         newlyOwned;
    }

    public class GachaSystem
    {
        const int KeysPerChest         = 1;
        const int PiecesPerDrop        = 5;
        const int PiecesToUnlock       = 20;
        const int PiecesToStar2        = 40;

        readonly ProgressService _progress;
        readonly List<string>    _warriorPool;

        public GachaSystem(ProgressService progress, List<string> warriorPool)
        {
            _progress    = progress;
            _warriorPool = warriorPool;
        }

        public bool CanOpen => _progress.Data.keys >= KeysPerChest;

        public ChestOpenResult OpenChest()
        {
            if (!CanOpen) return null;

            _progress.Data.keys -= KeysPerChest;

            var result     = new ChestOpenResult();
            result.warriorId = _warriorPool[Random.Range(0, _warriorPool.Count)];
            result.piecesGained = PiecesPerDrop;

            var save = GetOrCreateSave(result.warriorId);
            save.pieces += result.piecesGained;

            if (!save.isOwned && save.pieces >= PiecesToUnlock)
            {
                save.isOwned     = true;
                result.newlyOwned = true;
            }

            _progress.Save();
            return result;
        }

        WarriorSaveData GetOrCreateSave(string warriorId)
        {
            foreach (var ws in _progress.Data.ownedWarriors)
                if (ws.warriorId == warriorId) return ws;

            var newSave = new WarriorSaveData { warriorId = warriorId };
            _progress.Data.ownedWarriors.Add(newSave);
            return newSave;
        }
    }
}