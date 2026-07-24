using System;
using System.Collections.Generic;

namespace TWR.Save
{
    [Serializable]
    public class PlayerProgress
    {
        public int gold;
        public int keys;
        public int gems;
        public int energy;
        public long energyLastRegenTimestamp;
        public int highestChapterCleared;
        public bool heroUnlocked;
        public List<WarriorSaveData> ownedWarriors = new();
        public DeckConfig activeDeck = new();
        public List<string> unlockedSkillTreeNodes = new();

        public static PlayerProgress Default() => new()
        {
            gold = 500,
            keys = 3,
            gems = 0,
            energy = 10,
            energyLastRegenTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };
    }

    [Serializable]
    public class WarriorSaveData
    {
        public string warriorId;
        public int pieces;
        public int starLevel = 1;
        public int level = 1;
        public bool isOwned;
    }

    [Serializable]
    public class DeckConfig
    {
        public string[] warriorIds = Array.Empty<string>();
        public string[] skillIds = Array.Empty<string>();
    }
}