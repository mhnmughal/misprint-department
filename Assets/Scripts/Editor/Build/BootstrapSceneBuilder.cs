using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
using MisprintDepartment.Game;

namespace MisprintDepartment.Editor.Build
{
    public static class BootstrapSceneBuilder
    {
        const string Path = "Assets/Scenes/Bootstrap.unity";

        [MenuItem("Misprint/Build/Scene · Bootstrap")]
        public static void Build()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var cam = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
            cam.tag = "MainCamera";
            cam.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
            cam.GetComponent<Camera>().backgroundColor = new Color(0.14f, 0.13f, 0.12f);
            cam.transform.position = new Vector3(0, 0, -10);

            // persistent boot object
            var boot = new GameObject("Boot");
            var audioGo = new GameObject("AudioController");
            audioGo.transform.SetParent(boot.transform, false);
            var audio = audioGo.AddComponent<AudioController>();

            var musicGo = new GameObject("MusicSource");
            musicGo.transform.SetParent(audioGo.transform, false);
            var musicSource = musicGo.AddComponent<AudioSource>();
            musicSource.playOnAwake = false;
            musicSource.loop = true;

            var mixer = AssetDatabase.LoadAssetAtPath<AudioMixer>("Assets/Audio/MisprintMixer.mixer");
            var bank = AssetDatabase.LoadAssetAtPath<MisprintDepartment.Game.SoundBank>("Assets/Audio/SoundBank.asset");
            var db = AssetDatabase.LoadAssetAtPath<LevelDatabase>(MisprintBuild.LevelDbPath);

            AudioMixerGroup music = null, sfx = null, ui = null;
            if (mixer != null)
            {
                musicSource.outputAudioMixerGroup = Group(mixer, "Music");
                music = Group(mixer, "Music");
                sfx = Group(mixer, "SFX");
                ui = Group(mixer, "UI");
            }

            MisprintBuild.Wire(audio, "mixer", mixer);
            MisprintBuild.Wire(audio, "bank", bank);
            MisprintBuild.Wire(audio, "musicSource", musicSource);
            MisprintBuild.Wire(audio, "sfxGroup", sfx);
            MisprintBuild.Wire(audio, "uiGroup", ui);

            var bootComp = boot.AddComponent<Bootstrap>();
            MisprintBuild.Wire(bootComp, "levelDatabase", db);
            MisprintBuild.Wire(bootComp, "audioController", audio);

            // splash
            var (canvas, safe) = UiKit.BuildCanvas("SplashCanvas");
            var bg = UiKit.Image("Backplate", safe, UiKit.Charcoal);
            UiKit.Stretch((RectTransform)bg.transform);

            var title = UiKit.Text("Title", safe, "MISPRINT DEPARTMENT", 84,
                TextAlignmentOptions.Center, UiKit.Paper, FontStyles.Bold);
            var trt = (RectTransform)title.transform;
            trt.anchorMin = new Vector2(0.1f, 0.5f); trt.anchorMax = new Vector2(0.9f, 0.66f);
            trt.offsetMin = Vector2.zero; trt.offsetMax = Vector2.zero;

            var sub = UiKit.Text("Subtitle", safe, "PRINT   QUALITY   CONTROL", 30,
                TextAlignmentOptions.Center, UiKit.LampWarm, FontStyles.Normal);
            var srt = (RectTransform)sub.transform;
            srt.anchorMin = new Vector2(0.1f, 0.4f); srt.anchorMax = new Vector2(0.9f, 0.5f);
            srt.offsetMin = Vector2.zero; srt.offsetMax = Vector2.zero;

            var bar = UiKit.Image("RegistrationBar", safe, UiKit.WarnRed);
            var barrt = (RectTransform)bar.transform;
            barrt.anchorMin = new Vector2(0.35f, 0.36f); barrt.anchorMax = new Vector2(0.65f, 0.372f);
            barrt.offsetMin = Vector2.zero; barrt.offsetMax = Vector2.zero;

            EditorSceneManager.MarkSceneDirty(scene);
            EnsureDir("Assets/Scenes");
            EditorSceneManager.SaveScene(scene, Path);
            Debug.Log("[Misprint] Bootstrap scene built.");
        }

        static AudioMixerGroup Group(AudioMixer m, string name)
        {
            var groups = m.FindMatchingGroups(name);
            return groups != null && groups.Length > 0 ? groups[0] : null;
        }

        static void EnsureDir(string dir)
        {
            if (!AssetDatabase.IsValidFolder(dir))
                AssetDatabase.CreateFolder("Assets", "Scenes");
        }
    }
}
