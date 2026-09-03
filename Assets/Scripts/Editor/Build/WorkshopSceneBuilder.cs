using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MisprintDepartment.Game.UI;
using W = MisprintDepartment.Editor.Build.MisprintBuild;

namespace MisprintDepartment.Editor.Build
{
    public static class WorkshopSceneBuilder
    {
        const string Path = "Assets/Scenes/MainWorkshop.unity";

        [MenuItem("Misprint/Build/Scene · MainWorkshop")]
        public static void Build()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            new GameObject("Systems");
            EnvBuilder.CameraRig(new Vector3(0, 8.8f, -8.6f), new Vector3(44, 0, 0), 42f);
            EnvBuilder.Lighting(withLamp: true);
            EnvBuilder.Workstation("workshop");

            // proof sheet on the bench (the physical Start control's real-world anchor)
            ArtBuilder.Prim(PrimitiveType.Quad, "ProofSheet", GameObject.Find("Environment").transform,
                new Vector3(0, 0.02f, 0.6f), new Vector3(2.6f, 3.4f, 1f), new Vector3(90, 0, 0), ArtBuilder.M.Paper);
            ArtBuilder.Prim(PrimitiveType.Cube, "JobTicketRack", GameObject.Find("Environment").transform,
                new Vector3(3.2f, 0.35f, 0.2f), new Vector3(1.8f, 0.7f, 1.1f), new Vector3(0, -14, 0), ArtBuilder.M.CharcoalLight);
            ArtBuilder.Prim(PrimitiveType.Cube, "MachineManual", GameObject.Find("Environment").transform,
                new Vector3(-3.0f, 0.12f, 1.4f), new Vector3(0.9f, 0.14f, 1.2f), new Vector3(0, 12, 0), ArtBuilder.M.Ink);
            ArtBuilder.Prim(PrimitiveType.Cube, "SettingsControl", GameObject.Find("Environment").transform,
                new Vector3(-2.0f, 0.16f, -0.4f), new Vector3(0.8f, 0.3f, 0.5f), Vector3.zero, ArtBuilder.M.Metal);

            var (canvas, safe) = UiKit.BuildCanvas("WorkshopCanvas");

            // header
            PanelKit.Heading("Header", safe, "MISPRINT DEPARTMENT   ·   PROOF BENCH",
                new Vector2(0.06f, 0.9f), new Vector2(0.94f, 0.98f), 30, UiKit.Paper, TextAlignmentOptions.Left);

            // ---- Main interaction layer ----
            var mainGo = PanelKit.Plain("MainInteractionLayer", safe, new Color(0, 0, 0, 0));
            var workshop = mainGo.AddComponent<WorkshopScreen>();

            var startBtn = PanelKit.TextButton("StartInspectionButton", mainGo.transform, "START INSPECTION",
                new Vector2(0.06f, 0.14f), new Vector2(0.06f, 0.14f), new Vector2(0, 0), new Vector2(0, 0),
                new Vector2(620, 340), UiKit.Paper, UiKit.Ink, 42, out var startLabel);
            var sRt = (RectTransform)startBtn.transform;
            sRt.anchorMin = new Vector2(0.06f, 0.14f); sRt.anchorMax = new Vector2(0.06f, 0.14f);
            sRt.pivot = new Vector2(0, 0); sRt.anchoredPosition = Vector2.zero;

            var startCap = UiKit.Text("StartCaption", startBtn.transform, "PULL A JOB AND BEGIN INSPECTION",
                20, TextAlignmentOptions.Center, new Color(0.35f, 0.32f, 0.28f));
            var scRt = (RectTransform)startCap.transform;
            scRt.anchorMin = new Vector2(0, 0); scRt.anchorMax = new Vector2(1, 0.22f);
            scRt.offsetMin = new Vector2(20, 16); scRt.offsetMax = new Vector2(-20, 0);

            var currentJob = UiKit.Text("CurrentJobLabel", mainGo.transform, "NEXT: JOB 001 — FIRST PROOF",
                24, TextAlignmentOptions.Left, UiKit.LampWarm, FontStyles.Bold);
            var cjRt = (RectTransform)currentJob.transform;
            cjRt.anchorMin = new Vector2(0.06f, 0.08f); cjRt.anchorMax = new Vector2(0.6f, 0.14f);
            cjRt.offsetMin = Vector2.zero; cjRt.offsetMax = Vector2.zero;

            var jobFilesBtn = SideButton("JobFilesButton", mainGo.transform, "JOB FILES", 0);
            var manualBtn = SideButton("ManualButton", mainGo.transform, "MACHINE MANUAL", 1);
            var settingsBtn = SideButton("SettingsButton", mainGo.transform, "SETTINGS", 2);
            var creditsBtn = SideButton("CreditsButton", mainGo.transform, "CREDITS", 3, small: true);

            W.Wire(workshop, "startButton", startBtn);
            W.Wire(workshop, "startLabel", startLabel);
            W.Wire(workshop, "currentJobLabel", currentJob);
            W.Wire(workshop, "jobFilesButton", jobFilesBtn);
            W.Wire(workshop, "manualButton", manualBtn);
            W.Wire(workshop, "settingsButton", settingsBtn);
            W.Wire(workshop, "creditsButton", creditsBtn);

            // ---- Job Files layer ----
            var jobGo = PanelKit.Plain("JobFilesLayer", safe, UiKit.PanelBg);
            var jobSelect = jobGo.AddComponent<JobSelectScreen>();
            var jobHeader = PanelKit.Heading("BatchHeader", jobGo.transform, "JOB FILES — 6 BATCHES",
                new Vector2(0.06f, 0.86f), new Vector2(0.94f, 0.95f), 34, UiKit.Paper);
            var rack = UiKit.Rect("Rack", jobGo.transform);
            rack.anchorMin = new Vector2(0.05f, 0.13f); rack.anchorMax = new Vector2(0.95f, 0.83f);
            rack.offsetMin = Vector2.zero; rack.offsetMax = Vector2.zero;
            UiKit.Grid(rack.gameObject, new Vector2(300, 168), new Vector2(20, 18), 4);
            var jobBack = PanelKit.TextButton("BackButton", jobGo.transform, "BACK TO BENCH",
                new Vector2(0.08f, 0.03f), new Vector2(0.08f, 0.03f), new Vector2(0, 0), Vector2.zero,
                new Vector2(280, 64), UiKit.CharcoalUp, UiKit.Paper, 22, out _);
            var ticketPrefab = AssetDatabase.LoadAssetAtPath<JobTicket>("Assets/Prefabs/UI/JobTicket.prefab");
            W.Wire(jobSelect, "batchHeader", jobHeader);
            W.Wire(jobSelect, "rack", rack);
            W.Wire(jobSelect, "ticketPrefab", ticketPrefab);
            W.Wire(jobSelect, "backButton", jobBack);

            // ---- Settings layer ----
            var (setGo, settings) = BuildSettings(safe);

            // ---- Manual layer ----
            var manualGo = PanelKit.Plain("ManualLayer", safe, UiKit.Paper);
            var manualPanel = manualGo.AddComponent<UIPanel>();
            PanelKit.Heading("Title", manualGo.transform, "MACHINE MANUAL",
                new Vector2(0.1f, 0.84f), new Vector2(0.9f, 0.94f), 40, UiKit.Ink);
            var manualBody = UiKit.Text("Body", manualGo.transform, ManualText(), 24,
                TextAlignmentOptions.TopLeft, UiKit.Ink);
            var mbRt = (RectTransform)manualBody.transform;
            mbRt.anchorMin = new Vector2(0.12f, 0.16f); mbRt.anchorMax = new Vector2(0.88f, 0.82f);
            mbRt.offsetMin = Vector2.zero; mbRt.offsetMax = Vector2.zero;
            var manualBack = PanelKit.TextButton("BackButton", manualGo.transform, "CLOSE MANUAL",
                new Vector2(0.5f, 0.05f), new Vector2(0.5f, 0.05f), new Vector2(0.5f, 0), Vector2.zero,
                new Vector2(300, 64), UiKit.CharcoalUp, UiKit.Paper, 22, out _);

            // ---- Credits layer ----
            var creditsGo = PanelKit.Plain("CreditsLayer", safe, UiKit.PanelBg);
            var creditsPanel = creditsGo.AddComponent<UIPanel>();
            PanelKit.Heading("Title", creditsGo.transform, "CREDITS",
                new Vector2(0.1f, 0.82f), new Vector2(0.9f, 0.92f), 40, UiKit.Paper);
            var creditsBody = UiKit.Text("Body", creditsGo.transform, CreditsText(), 26,
                TextAlignmentOptions.Center, UiKit.Paper);
            var cbRt = (RectTransform)creditsBody.transform;
            cbRt.anchorMin = new Vector2(0.15f, 0.25f); cbRt.anchorMax = new Vector2(0.85f, 0.78f);
            cbRt.offsetMin = Vector2.zero; cbRt.offsetMax = Vector2.zero;
            var creditsBack = PanelKit.TextButton("BackButton", creditsGo.transform, "BACK",
                new Vector2(0.5f, 0.06f), new Vector2(0.5f, 0.06f), new Vector2(0.5f, 0), Vector2.zero,
                new Vector2(240, 60), UiKit.CharcoalUp, UiKit.Paper, 22, out _);

            // ---- Dialog layer ----
            var dialog = BuildDialog(safe);
            W.Wire(settings, "dialog", dialog);

            // ---- Nav ----
            var nav = GameObject.Find("Systems").AddComponent<WorkshopNav>();
            W.Wire(nav, "workshop", workshop);
            W.Wire(nav, "jobSelect", jobSelect);
            W.Wire(nav, "settings", settings);
            W.Wire(nav, "manual", manualPanel);
            W.Wire(nav, "credits", creditsPanel);
            W.Wire(nav, "manualBackButton", manualBack);
            W.Wire(nav, "creditsBackButton", creditsBack);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, Path);
            Debug.Log("[Misprint] MainWorkshop scene built.");
        }

        static Button SideButton(string name, Transform parent, string label, int index, bool small = false)
        {
            var btn = UiKit.Button(name, parent, label, small ? new Color(0,0,0,0) : UiKit.CharcoalUp,
                UiKit.Paper, small ? 20 : 26, out var lbl);
            var rt = (RectTransform)btn.transform;
            rt.anchorMin = new Vector2(1, 0.62f); rt.anchorMax = new Vector2(1, 0.62f);
            rt.pivot = new Vector2(1, 1);
            rt.sizeDelta = new Vector2(460, small ? 56 : 96);
            rt.anchoredPosition = new Vector2(-60, -index * 116);
            if (small) lbl.color = UiKit.LampWarm;
            return btn;
        }

        static (GameObject, SettingsScreen) BuildSettings(Transform safe)
        {
            var go = PanelKit.Plain("SettingsLayer", safe, UiKit.PanelBg);
            var settings = go.AddComponent<SettingsScreen>();
            PanelKit.Heading("Title", go.transform, "MAINTENANCE CARD",
                new Vector2(0.1f, 0.84f), new Vector2(0.9f, 0.94f), 38, UiKit.Paper);

            var col = UiKit.Rect("Rows", go.transform);
            col.anchorMin = new Vector2(0.22f, 0.2f); col.anchorMax = new Vector2(0.78f, 0.82f);
            col.offsetMin = Vector2.zero; col.offsetMax = Vector2.zero;
            UiKit.VLayout(col.gameObject, 22, new RectOffset(0, 0, 0, 0), TextAnchor.UpperCenter);

            var music = SliderRow(col, "Music");
            var sfx = SliderRow(col, "Sound Effects");
            var ui = SliderRow(col, "Interface");

            var hapticsRow = UiKit.Rect("HapticsRow", col);
            UiKit.Size(hapticsRow, 640, 48);
            var haptics = UiKit.Toggle("HapticsToggle", hapticsRow, "Haptics", UiKit.CharcoalUp, UiKit.Teal, UiKit.Paper);
            UiKit.Stretch((RectTransform)haptics.transform);

            var resetRow = UiKit.Rect("ResetRow", col);
            UiKit.Size(resetRow, 640, 64);
            var resetBtn = UiKit.Button("ResetButton", resetRow, "RESET ALL PROGRESS", UiKit.WarnRed, UiKit.Paper, 22, out _);
            UiKit.Stretch((RectTransform)resetBtn.transform);

            var creditsRow = UiKit.Rect("CreditsRow", col);
            UiKit.Size(creditsRow, 640, 56);
            var creditsBtn = UiKit.Button("CreditsButton", creditsRow, "CREDITS", UiKit.CharcoalUp, UiKit.Paper, 20, out _);
            UiKit.Stretch((RectTransform)creditsBtn.transform);

            var back = PanelKit.TextButton("BackButton", go.transform, "BACK TO BENCH",
                new Vector2(0.5f, 0.05f), new Vector2(0.5f, 0.05f), new Vector2(0.5f, 0), Vector2.zero,
                new Vector2(300, 64), UiKit.CharcoalUp, UiKit.Paper, 22, out _);

            W.Wire(settings, "musicSlider", music);
            W.Wire(settings, "sfxSlider", sfx);
            W.Wire(settings, "uiSlider", ui);
            W.Wire(settings, "hapticsToggle", haptics);
            W.Wire(settings, "resetButton", resetBtn);
            W.Wire(settings, "creditsButton", creditsBtn);
            W.Wire(settings, "backButton", back);
            return (go, settings);
        }

        static Slider SliderRow(RectTransform parent, string label)
        {
            var row = UiKit.Rect(label.Replace(" ", "") + "Row", parent);
            UiKit.Size(row, 720, 64);
            var lab = UiKit.Text("Label", row, label.ToUpperInvariant(), 22, TextAlignmentOptions.MidlineLeft, UiKit.Paper, FontStyles.Bold);
            var labRt = (RectTransform)lab.transform;
            labRt.anchorMin = new Vector2(0, 0); labRt.anchorMax = new Vector2(0.4f, 1);
            labRt.offsetMin = Vector2.zero; labRt.offsetMax = Vector2.zero;
            var slider = UiKit.Slider(label.Replace(" ", "") + "Slider", row, UiKit.CharcoalUp, UiKit.Teal, UiKit.Paper);
            var sRt = (RectTransform)slider.transform;
            sRt.anchorMin = new Vector2(0.42f, 0.5f); sRt.anchorMax = new Vector2(1f, 0.5f);
            sRt.pivot = new Vector2(0.5f, 0.5f);
            sRt.offsetMin = new Vector2(0, -20); sRt.offsetMax = new Vector2(0, 20);
            return slider;
        }

        static ConfirmDialog BuildDialog(Transform safe)
        {
            var go = PanelKit.Plain("DialogLayer", safe, new Color(0, 0, 0, 0.6f));
            var dialog = go.AddComponent<ConfirmDialog>();
            var card = UiKit.Image("Card", go.transform, UiKit.Paper);
            var cardRt = (RectTransform)card.transform;
            cardRt.anchorMin = new Vector2(0.5f, 0.5f); cardRt.anchorMax = new Vector2(0.5f, 0.5f);
            cardRt.pivot = new Vector2(0.5f, 0.5f); UiKit.Size(cardRt, 820, 420);

            var title = PanelKit.Heading("Title", card.transform, "RESET ALL PROGRESS?",
                new Vector2(0.06f, 0.72f), new Vector2(0.94f, 0.92f), 34, UiKit.Ink);
            var body = UiKit.Text("Body", card.transform, "This cannot be undone.", 24,
                TextAlignmentOptions.Center, UiKit.Ink);
            var bodyRt = (RectTransform)body.transform;
            bodyRt.anchorMin = new Vector2(0.08f, 0.36f); bodyRt.anchorMax = new Vector2(0.92f, 0.7f);
            bodyRt.offsetMin = Vector2.zero; bodyRt.offsetMax = Vector2.zero;

            var confirm = PanelKit.TextButton("ConfirmButton", card.transform, "RESET",
                new Vector2(0.28f, 0.12f), new Vector2(0.28f, 0.12f), new Vector2(0.5f, 0), Vector2.zero,
                new Vector2(300, 72), UiKit.WarnRed, UiKit.Paper, 24, out var confirmLabel);
            var cancel = PanelKit.TextButton("CancelButton", card.transform, "KEEP",
                new Vector2(0.72f, 0.12f), new Vector2(0.72f, 0.12f), new Vector2(0.5f, 0), Vector2.zero,
                new Vector2(300, 72), UiKit.CharcoalUp, UiKit.Paper, 24, out _);

            W.Wire(dialog, "titleText", title);
            W.Wire(dialog, "bodyText", body);
            W.Wire(dialog, "confirmLabel", confirmLabel);
            W.Wire(dialog, "confirmButton", confirm);
            W.Wire(dialog, "cancelButton", cancel);
            return dialog;
        }

        static string ManualText() =>
            "The command printer prints a route as a strip of instructions:\n\n" +
            "  MOVE  — advance one cell        JUMP  — leap two cells\n" +
            "  TURN LEFT / TURN RIGHT          REVERSE — turn about\n" +
            "  WAIT  — hold for one step\n\n" +
            "Each job the plate has ONE known defect — read the Defect card.\n" +
            "Use the bench tools to correct the misprinted commands:\n\n" +
            "  ROTATE  — step a command around the wheel  M > R > REV > L\n" +
            "  REPLACE — set a command to the one it should be\n\n" +
            "You have a limited correction budget. Pull RUN PROOF to send the\n" +
            "corrected route. If it reaches the destination, the proof is approved.";

        static string CreditsText() =>
            "MISPRINT DEPARTMENT\n\n" +
            "Design, code, art direction, audio\nby the Misprint Department team\n\n" +
            "All sound is original and generated in-project.\n" +
            "Type: Liberation Sans (SIL Open Font License).\n\n" +
            "Made offline. No ads, no tracking, no accounts.";
    }
}
