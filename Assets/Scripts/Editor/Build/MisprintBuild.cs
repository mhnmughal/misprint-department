using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
using MisprintDepartment.Core;
using MisprintDepartment.Game;

namespace MisprintDepartment.Editor.Build
{
    /// <summary>Master build menu — runs every builder in order and configures the project.</summary>
    public static class MisprintBuild
    {
        public const string LevelDbPath = "Assets/Content/LevelDatabase.asset";

        [MenuItem("Misprint/Build/0 · REBUILD EVERYTHING", priority = 0)]
        public static void RebuildEverything()
        {
            ArtBuilder.BuildPalette();
            AudioBuilder.BuildAll();
            PrefabBuilder.BuildAll();
            IconBuilder.BuildMenu();
            ChapterBuilders.BuildChapter1();
            BuildLevelDatabase();
            BootstrapSceneBuilder.Build();
            WorkshopSceneBuilder.Build();
            GameplaySceneBuilder.Build();
            ConfigureBuildSettings();
            ConfigurePlayerSettings();
            AssetDatabase.SaveAssets();
            Debug.Log("[Misprint] FULL REBUILD complete.");
        }

        [MenuItem("Misprint/Build/4 · Level Database")]
        public static LevelDatabase BuildLevelDatabase()
        {
            var db = AssetDatabase.LoadAssetAtPath<LevelDatabase>(LevelDbPath);
            if (db == null)
            {
                db = ScriptableObject.CreateInstance<LevelDatabase>();
                AssetDatabase.CreateAsset(db, LevelDbPath);
            }
            var all = AssetDatabase.FindAssets("t:LevelDefinition")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<LevelDefinition>)
                .Where(l => l != null)
                .OrderBy(l => l.levelId)
                .ToList();
            db.trainingLevel = all.FirstOrDefault(l => l.levelId == 0 && l.chapter == 0);
            db.levels = all.Where(l => !(l.levelId == 0 && l.chapter == 0)).ToList();
            EditorUtility.SetDirty(db);
            AssetDatabase.SaveAssets();
            Debug.Log($"[Misprint] LevelDatabase: {db.levels.Count} levels.");
            return db;
        }

        [MenuItem("Misprint/Build/5 · Build + Player Settings")]
        public static void ConfigureAll()
        {
            ConfigureBuildSettings();
            ConfigurePlayerSettings();
        }

        public static void ConfigureBuildSettings()
        {
            string[] scenes =
            {
                "Assets/Scenes/Bootstrap.unity",
                "Assets/Scenes/MainWorkshop.unity",
                "Assets/Scenes/Gameplay.unity",
            };
            EditorBuildSettings.scenes = scenes
                .Where(s => System.IO.File.Exists(s))
                .Select(s => new EditorBuildSettingsScene(s, true))
                .ToArray();

            var sample = "Assets/Scenes/SampleScene.unity";
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(sample) != null)
                AssetDatabase.DeleteAsset(sample);
            Debug.Log("[Misprint] Build settings set: Bootstrap, MainWorkshop, Gameplay.");
        }

        public static void ConfigurePlayerSettings()
        {
            PlayerSettings.productName = "Misprint Department";
            PlayerSettings.companyName = "Misprint Department team";

            PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
            PlayerSettings.allowedAutorotateToLandscapeLeft = true;
            PlayerSettings.allowedAutorotateToLandscapeRight = true;
            PlayerSettings.allowedAutorotateToPortrait = false;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;

            // reverse-DNS, lowercase, no spaces: "Misprint Department team" -> misprintdepartmentteam
            const string bundleId = "com.misprintdepartmentteam.misprintdepartment";
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, bundleId);
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, bundleId);

            PlayerSettings.SetScriptingBackend(NamedBuildTarget.iOS, ScriptingImplementation.IL2CPP);
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;

            PlayerSettings.muteOtherAudioSources = false;
            Debug.Log($"[Misprint] Player settings configured — company '{PlayerSettings.companyName}', " +
                      $"bundle '{PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android)}', landscape, IL2CPP/ARM64.");
        }

        // shared helpers for scene builders -----------------------------------

        public static void Wire(Component c, string field, Object value)
        {
            var so = new SerializedObject(c);
            var p = so.FindProperty(field);
            if (p == null) { Debug.LogWarning($"[wire] {c.GetType().Name}.{field} missing"); return; }
            p.objectReferenceValue = value;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        public static void WireList(Component c, string field, IEnumerable<Object> values)
        {
            var so = new SerializedObject(c);
            var p = so.FindProperty(field);
            if (p == null) { Debug.LogWarning($"[wire] {c.GetType().Name}.{field} missing"); return; }
            var list = values.ToList();
            p.arraySize = list.Count;
            for (int i = 0; i < list.Count; i++)
                p.GetArrayElementAtIndex(i).objectReferenceValue = list[i];
            so.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
