using MisprintDepartment.Core;

namespace MisprintDepartment.Game
{
    /// <summary>
    /// Static handoff between scenes. MainWorkshop sets the pending level, then
    /// loads the Gameplay scene; LevelController reads it on load. No
    /// persistence, no engine deps — just the in-memory bridge.
    /// </summary>
    public static class GameSession
    {
        public static LevelDefinition PendingLevel;

        /// <summary>Where to return after the Gameplay scene ("MainWorkshop").</summary>
        public static string ReturnScene = "MainWorkshop";

        public static bool HasPendingLevel => PendingLevel != null;

        public static void Clear() => PendingLevel = null;
    }
}
