using UnityEngine;

namespace MisprintDepartment.Game
{
    /// <summary>
    /// Tiny service locator. Created once by Bootstrap and kept alive for the
    /// session. No network, no cloud — purely local.
    /// </summary>
    public static class Services
    {
        public static SaveService Save { get; private set; }
        public static AudioController Audio { get; private set; }
        public static LevelDatabase Levels { get; private set; }

        public static bool Ready => Save != null;

        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics()
        {
            Save = null; Audio = null; Levels = null;
        }

        public static void Init(LevelDatabase levels, AudioController audio)
        {
            Levels = levels;
            Save = new SaveService();
            Save.Load();

            Audio = audio;
            if (Audio != null) Audio.ApplyFromSave(Save.Data);
        }

        /// <summary>Editor/test convenience: ensure something is present.</summary>
        public static void EnsureForEditor()
        {
            if (Save == null) { Save = new SaveService(); Save.Load(); }
        }
    }
}
