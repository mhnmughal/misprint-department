using System;
using System.IO;
using UnityEngine;

namespace MisprintDepartment.Game
{
    /// <summary>
    /// Local, offline, versioned save. Plain class (not a MonoBehaviour) so it
    /// is unit-testable with a temp path. No network, no cloud.
    /// </summary>
    public sealed class SaveService
    {
        public const int TotalLevels = 12;

        readonly string _path;
        public SaveData Data { get; private set; }

        /// <summary>Default: Application.persistentDataPath/misprint_save.json.</summary>
        public SaveService() : this(System.IO.Path.Combine(Application.persistentDataPath, "misprint_save.json")) { }

        public SaveService(string path)
        {
            _path = path;
            Data = new SaveData();
        }

        public string FilePath => _path;

        /// <summary>Load from disk. Missing or corrupt -> fresh default (bad file backed up).</summary>
        public void Load()
        {
            if (!File.Exists(_path)) { Data = new SaveData(); return; }

            string json;
            try { json = File.ReadAllText(_path); }
            catch (Exception e) { Debug.LogWarning($"[Save] read failed: {e.Message}"); Data = new SaveData(); return; }

            Data = Deserialize(json, out string problem);
            if (problem != null)
            {
                Debug.LogWarning($"[Save] {problem} — starting fresh, bad file backed up.");
                TryBackup(json);
            }
        }

        public void Save()
        {
            Data.version = SaveData.CurrentVersion;
            try
            {
                var dir = Path.GetDirectoryName(_path);
                if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
                File.WriteAllText(_path, JsonUtility.ToJson(Data, true));
            }
            catch (Exception e)
            {
                Debug.LogError($"[Save] write failed: {e.Message}");
            }
        }

        public void ResetProgress()
        {
            var keepMusic = Data.musicVolume;
            var keepSfx = Data.sfxVolume;
            var keepUi = Data.uiVolume;
            var keepHaptics = Data.hapticsEnabled;
            Data = new SaveData
            {
                musicVolume = keepMusic, sfxVolume = keepSfx,
                uiVolume = keepUi, hapticsEnabled = keepHaptics,
            };
            Save();
        }

        /// <summary>Pure parse: returns valid data, sets <paramref name="problem"/> non-null if the input was unusable.</summary>
        public static SaveData Deserialize(string json, out string problem)
        {
            problem = null;
            if (string.IsNullOrWhiteSpace(json)) { problem = "empty save"; return new SaveData(); }

            SaveData data;
            try { data = JsonUtility.FromJson<SaveData>(json); }
            catch (Exception e) { problem = "unparseable save (" + e.GetType().Name + ")"; return new SaveData(); }

            if (data == null) { problem = "null save"; return new SaveData(); }

            if (data.version <= 0 || data.version > SaveData.CurrentVersion)
            {
                problem = $"unknown save version {data.version}";
                return new SaveData();
            }

            // sanity clamps (defends against hand-edited files)
            data.levels ??= new System.Collections.Generic.List<LevelRecord>();
            data.tutorialsSeen ??= new System.Collections.Generic.List<string>();
            data.highestUnlockedLevel = Mathf.Clamp(data.highestUnlockedLevel, 1, TotalLevels);
            data.musicVolume = Mathf.Clamp01(data.musicVolume);
            data.sfxVolume = Mathf.Clamp01(data.sfxVolume);
            data.uiVolume = Mathf.Clamp01(data.uiVolume);
            // future: migrate older versions here
            data.version = SaveData.CurrentVersion;
            return data;
        }

        void TryBackup(string json)
        {
            try { File.WriteAllText(_path + ".bak", json); } catch { /* best effort */ }
        }
    }
}
