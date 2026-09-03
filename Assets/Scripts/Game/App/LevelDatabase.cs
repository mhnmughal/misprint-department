using System.Collections.Generic;
using UnityEngine;
using MisprintDepartment.Core;

namespace MisprintDepartment.Game
{
    /// <summary>
    /// Ordered registry of every LevelDefinition. Built by the editor menu
    /// "Misprint/Rebuild Level Database". Menus/Job Select enumerate from here.
    /// </summary>
    [CreateAssetMenu(menuName = "Misprint/Level Database", fileName = "LevelDatabase")]
    public sealed class LevelDatabase : ScriptableObject
    {
        [Tooltip("The Training Job — played once before Job 001. Not part of the 42.")]
        public LevelDefinition trainingLevel;

        public List<LevelDefinition> levels = new List<LevelDefinition>();

        public int Count => levels.Count;
        public int TotalLevels => 12;

        public LevelDefinition ByIndex(int i) =>
            (i >= 0 && i < levels.Count) ? levels[i] : null;

        public LevelDefinition ById(int levelId)
        {
            if (levelId == 0 && trainingLevel != null) return trainingLevel;
            for (int i = 0; i < levels.Count; i++)
                if (levels[i] != null && levels[i].levelId == levelId) return levels[i];
            return null;
        }

        public LevelDefinition Next(int levelId)
        {
            if (levelId == 0) return ByIndex(0); // Training -> Job 001
            for (int i = 0; i < levels.Count; i++)
                if (levels[i] != null && levels[i].levelId == levelId)
                    return (i + 1 < levels.Count) ? levels[i + 1] : null;
            return null;
        }

        public IEnumerable<LevelDefinition> Chapter(int chapter)
        {
            foreach (var l in levels)
                if (l != null && l.chapter == chapter) yield return l;
        }

        public int HighestChapterAvailable()
        {
            int max = 1;
            foreach (var l in levels) if (l != null && l.chapter > max) max = l.chapter;
            return max;
        }
    }
}
