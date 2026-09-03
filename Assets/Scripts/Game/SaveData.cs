using System;
using System.Collections.Generic;
using MisprintDepartment.Core;

namespace MisprintDepartment.Game
{
    [Serializable]
    public sealed class LevelRecord
    {
        public int levelId;
        public bool completed;
        public int rating;          // ProofRating as int
        public int bestCorrections = int.MaxValue;
    }

    /// <summary>Versioned local save. Plain fields for Unity JsonUtility.</summary>
    [Serializable]
    public sealed class SaveData
    {
        public const int CurrentVersion = 1;

        public int version = CurrentVersion;

        // progression
        public int highestUnlockedLevel = 1;   // 1..42
        public List<LevelRecord> levels = new List<LevelRecord>();
        public List<string> tutorialsSeen = new List<string>();

        // settings
        public float musicVolume = 0.8f;
        public float sfxVolume = 0.9f;
        public float uiVolume = 0.9f;
        public bool hapticsEnabled = true;

        public LevelRecord GetOrCreate(int levelId)
        {
            for (int i = 0; i < levels.Count; i++)
                if (levels[i].levelId == levelId) return levels[i];
            var rec = new LevelRecord { levelId = levelId };
            levels.Add(rec);
            return rec;
        }

        public LevelRecord Find(int levelId)
        {
            for (int i = 0; i < levels.Count; i++)
                if (levels[i].levelId == levelId) return levels[i];
            return null;
        }

        public bool IsUnlocked(int levelId) => levelId <= highestUnlockedLevel;

        /// <summary>Record a proof result; returns true if anything changed.</summary>
        public bool RecordResult(int levelId, bool approved, ProofRating rating, int correctionsUsed, int totalLevels)
        {
            if (!approved) return false;
            var rec = GetOrCreate(levelId);
            bool changed = false;

            if (!rec.completed) { rec.completed = true; changed = true; }
            if ((int)rating > rec.rating) { rec.rating = (int)rating; changed = true; }
            if (correctionsUsed < rec.bestCorrections) { rec.bestCorrections = correctionsUsed; changed = true; }

            int next = Math.Min(levelId + 1, totalLevels);
            if (next > highestUnlockedLevel) { highestUnlockedLevel = next; changed = true; }
            return changed;
        }
    }
}
