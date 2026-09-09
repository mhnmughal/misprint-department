using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MisprintDepartment.Core;
using MisprintDepartment.Game;
using MisprintDepartment.Game.UI;
using W = MisprintDepartment.Editor.Build.MisprintBuild;

namespace MisprintDepartment.Editor.Build
{
    public static class GameplaySceneBuilder
    {
        const string Path = "Assets/Scenes/Gameplay.unity";

        [MenuItem("Misprint/Build/Scene · Gameplay")]
        public static void Build()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var systems = new GameObject("Systems");
            EnvBuilder.CameraRig(new Vector3(0, 10.6f, -6.2f), new Vector3(62, 0, 0), 40f);
            EnvBuilder.Lighting(withLamp: true);
            var env = EnvBuilder.Workstation("gameplay");
            env.transform.position = new Vector3(0, 0, 4.0f); // machine sits well behind the proof board

            // ---- Proof board (world) — the focal surface, in front of the machine ----
            // nudged slightly left to balance the right-side defect card
            var board = new GameObject("ProofBoard");
            board.transform.position = new Vector3(-0.3f, 0.05f, -0.55f);
            var routeArea = new GameObject("RouteArea");
            routeArea.transform.SetParent(board.transform, false);

            var startMarker = ArtBuilder.Prim(PrimitiveType.Quad, "StartMarker", board.transform,
                Vector3.zero, new Vector3(0.7f, 0.7f, 1f), new Vector3(90, 0, 0), ArtBuilder.M.Teal).transform;
            var destMarker = ArtBuilder.Prim(PrimitiveType.Quad, "DestinationMarker", board.transform,
                Vector3.zero, new Vector3(0.7f, 0.7f, 1f), new Vector3(90, 0, 0), ArtBuilder.M.WarnRed).transform;

            var carriagePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Board/Carriage.prefab");
            var carriage = (GameObject)PrefabUtility.InstantiatePrefab(carriagePrefab);
            carriage.transform.SetParent(board.transform, false);

            var cellPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Board/CellTile.prefab");
            var blockedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Board/BlockedTile.prefab");

            var routeView = board.AddComponent<RouteView>();
            W.Wire(routeView, "gridRoot", routeArea.transform);
            W.Wire(routeView, "cellTilePrefab", cellPrefab);
            W.Wire(routeView, "blockedTilePrefab", blockedPrefab);
            W.Wire(routeView, "startMarker", startMarker);
            W.Wire(routeView, "destinationMarker", destMarker);
            W.Wire(routeView, "carriage", carriage.transform);

            // ---- systems ----
            var levelCtrl = systems.AddComponent<LevelController>();
            var fallback = AssetDatabase.LoadAssetAtPath<LevelDefinition>("Assets/Content/Levels/Batch01/L01_First_Proof.asset")
                        ?? AssetDatabase.LoadAssetAtPath<LevelDefinition>("Assets/Content/Levels/Training/L00_Training_Job.asset");
            W.Wire(levelCtrl, "fallbackLevel", fallback);
            W.Wire(levelCtrl, "routeView", routeView);

            var flow = systems.AddComponent<GameplayFlow>();

            // ---- UI ----
            var (canvas, safe) = UiKit.BuildCanvas("GameplayCanvas");

            var hud = BuildHud(safe, out var pauseButton);
            var defect = BuildDefectCard(safe);
            var (stripUI, stripContent) = BuildStrip(safe);
            var (tray, toolButtons, undoButton) = BuildTray(safe);
            var proof = BuildLever(safe);
            var popup = BuildPopup(safe);
            var toast = BuildToast(safe);
            var tutorial = BuildTutorial(safe, stripContent, ((RectTransform)tray.transform), ((RectTransform)proof.transform));
            var pause = BuildPause(safe);
            var result = BuildResult(safe);
            var instructions = BuildInstructions(safe, out var instructionsClose);
            var dialog = WorkshopDialogClone(safe);

            // wire flow
            W.Wire(flow, "level", levelCtrl);
            W.Wire(flow, "hud", hud);
            W.Wire(flow, "defectCard", defect);
            W.Wire(flow, "strip", stripUI);
            W.Wire(flow, "tray", tray);
            W.Wire(flow, "popup", popup);
            W.Wire(flow, "proofControls", proof);
            W.Wire(flow, "result", result);
            W.Wire(flow, "pause", pause);
            W.Wire(flow, "tutorial", tutorial);
            W.Wire(flow, "dialog", dialog);
            W.Wire(flow, "toast", toast);
            W.Wire(flow, "instructionsPanel", instructions);
            W.Wire(flow, "instructionsCloseButton", instructionsClose);

            W.WireList(tray, "toolButtons", toolButtons);
            W.Wire(tray, "undoButton", undoButton);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, Path);
            Debug.Log("[Misprint] Gameplay scene built.");
        }

        static GameplayHUD BuildHud(Transform safe, out Button pauseButton)
        {
            var go = UiKit.Rect("TopHUD", safe);
            go.anchorMin = new Vector2(0, 1); go.anchorMax = new Vector2(1, 1); go.pivot = new Vector2(0.5f, 1);
            UiKit.Size(go, 0, 96); UiKit.Pos(go, 0, 0);
            var bar = go.gameObject.AddComponent<Image>();
            bar.color = new Color(0.14f, 0.13f, 0.12f, 0.92f);

            var hud = go.gameObject.AddComponent<GameplayHUD>();
            var jobId = UiKit.Text("JobId", go, "JOB 001", 30, TextAlignmentOptions.MidlineLeft, UiKit.Paper, FontStyles.Bold);
            Anchor(jobId, 0.02f, 0, 0.2f, 1);
            var jobTitle = UiKit.Text("JobTitle", go, "FIRST PROOF", 24, TextAlignmentOptions.Midline, UiKit.LampWarm);
            Anchor(jobTitle, 0.2f, 0, 0.62f, 1);
            var budget = UiKit.Text("Budget", go, "CORRECTIONS  2/2", 24, TextAlignmentOptions.MidlineRight, UiKit.Paper, FontStyles.Bold);
            Anchor(budget, 0.62f, 0, 0.9f, 1);

            pauseButton = UiKit.Button("PauseButton", go, "II", UiKit.CharcoalUp, UiKit.Paper, 26, out _);
            var pRt = (RectTransform)pauseButton.transform;
            pRt.anchorMin = new Vector2(1, 0.5f); pRt.anchorMax = new Vector2(1, 0.5f); pRt.pivot = new Vector2(1, 0.5f);
            UiKit.Size(pRt, 76, 76); UiKit.Pos(pRt, -12, 0);

            W.Wire(hud, "jobIdText", jobId);
            W.Wire(hud, "jobTitleText", jobTitle);
            W.Wire(hud, "budgetText", budget);
            W.Wire(hud, "pauseButton", pauseButton);
            return hud;
        }

        static DefectCardUI BuildDefectCard(Transform safe)
        {
            var go = UiKit.Rect("DefectRuleCard", safe);
            go.anchorMin = new Vector2(1, 1); go.anchorMax = new Vector2(1, 1); go.pivot = new Vector2(1, 1);
            UiKit.Size(go, 540, 208); UiKit.Pos(go, -28, -320);
            var bg = go.gameObject.AddComponent<Image>();
            bg.color = UiKit.Paper;
            UiKit.EdgeFrame("Edge", go, UiKit.Ink, 3f, 0f);
            var edge = UiKit.Image("RedTab", go, UiKit.WarnRed, false);
            var eRt = (RectTransform)edge.transform;
            eRt.anchorMin = new Vector2(0, 0); eRt.anchorMax = new Vector2(0, 1);
            eRt.sizeDelta = new Vector2(12, 0); eRt.anchoredPosition = new Vector2(6, 0);
            UiKit.RegistrationMark(go, new Vector2(0.94f, 0.86f), UiKit.PaperDeep, 16f);
            var card = go.gameObject.AddComponent<DefectCardUI>();

            var head = UiKit.Text("Headline", go, "TODAY'S DEFECT", 20, TextAlignmentOptions.BottomLeft, UiKit.WarnRed, FontStyles.Bold);
            head.characterSpacing = 4f;
            Anchor(head, 0.07f, 0.74f, 0.95f, 0.93f);
            var rule = UiKit.Image("HeadRule", go, UiKit.WarnRed, false);
            var rlRt = (RectTransform)rule.transform;
            rlRt.anchorMin = new Vector2(0.07f, 0.72f); rlRt.anchorMax = new Vector2(0.5f, 0.72f);
            rlRt.sizeDelta = new Vector2(0, 3); rlRt.anchoredPosition = Vector2.zero;
            var body = UiKit.Text("Body", go, "…", 25, TextAlignmentOptions.TopLeft, UiKit.Ink);
            Anchor(body, 0.07f, 0.10f, 0.94f, 0.64f);

            W.Wire(card, "headline", head);
            W.Wire(card, "body", body);
            return card;
        }

        static (InstructionStripUI, RectTransform) BuildStrip(Transform safe)
        {
            var holder = UiKit.Rect("InstructionStrip", safe);
            holder.anchorMin = new Vector2(0.5f, 1f); holder.anchorMax = new Vector2(0.5f, 1f);
            holder.pivot = new Vector2(0.5f, 1f);
            UiKit.Size(holder, 1180, 176); UiKit.Pos(holder, -120, -112);
            var backing = holder.gameObject.AddComponent<Image>();
            backing.color = new Color(0.10f, 0.09f, 0.08f, 0.86f);
            UiKit.EdgeFrame("Edge", holder, UiKit.InkSoft, 2f, 0f);
            var tabBg = UiKit.Image("TabBg", holder, UiKit.LampWarm, false);
            var tbRt = (RectTransform)tabBg.transform;
            tbRt.anchorMin = new Vector2(0, 1); tbRt.anchorMax = new Vector2(0, 1); tbRt.pivot = new Vector2(0, 0);
            UiKit.Size(tbRt, 210, 32); UiKit.Pos(tbRt, 14, -1);
            var tab = UiKit.Text("Tab", tabBg.transform, "PRINTED ROUTE", 15, TMPro.TextAlignmentOptions.Center, UiKit.Ink, TMPro.FontStyles.Bold);
            UiKit.Stretch((RectTransform)tab.transform); tab.characterSpacing = 4f;

            var content = UiKit.Rect("Content", holder);
            UiKit.Stretch(content, 16f);
            UiKit.HLayout(content.gameObject, 12, new RectOffset(12, 12, 12, 12), TextAnchor.MiddleCenter);

            var ui = holder.gameObject.AddComponent<InstructionStripUI>();
            var tokenPrefab = AssetDatabase.LoadAssetAtPath<TokenView>("Assets/Prefabs/UI/TokenView.prefab");
            W.Wire(ui, "content", content);
            W.Wire(ui, "tokenPrefab", tokenPrefab);
            return (ui, content);
        }

        static (CorrectionTrayUI, System.Collections.Generic.List<Object>, Button) BuildTray(Transform safe)
        {
            var go = UiKit.Rect("CorrectionTray", safe);
            go.anchorMin = new Vector2(0, 0); go.anchorMax = new Vector2(0, 0); go.pivot = new Vector2(0, 0);
            UiKit.Size(go, 900, 320); UiKit.Pos(go, 24, 24);
            var bg = go.gameObject.AddComponent<Image>();
            bg.color = new Color(0.14f, 0.13f, 0.12f, 0.92f);

            var title = UiKit.Text("TrayTitle", go, "CORRECTION TRAY", 20, TextAlignmentOptions.TopLeft, UiKit.LampWarm, FontStyles.Bold);
            Anchor(title, 0.03f, 0.82f, 0.97f, 0.98f);

            var row = UiKit.Rect("Tools", go);
            row.anchorMin = new Vector2(0, 0); row.anchorMax = new Vector2(1, 0.78f);
            row.offsetMin = new Vector2(16, 16); row.offsetMax = new Vector2(-16, -8);
            UiKit.Grid(row.gameObject, new Vector2(168, 120), new Vector2(12, 12), 4);

            var tray = go.gameObject.AddComponent<CorrectionTrayUI>();
            var prefabGo = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/UI/ToolButton.prefab");
            var list = new System.Collections.Generic.List<Object>();
            foreach (CorrectionTool tool in System.Enum.GetValues(typeof(CorrectionTool)))
            {
                var inst = (GameObject)PrefabUtility.InstantiatePrefab(prefabGo, row);
                inst.name = tool + "Button";
                var tb = inst.GetComponent<ToolButton>();
                SetToolField(tb, tool);
                list.Add(tb);
            }

            var undo = UiKit.Button("UndoButton", go, "UNDO", UiKit.CharcoalUp, UiKit.Paper, 20, out _);
            var uRt = (RectTransform)undo.transform;
            uRt.anchorMin = new Vector2(1, 1); uRt.anchorMax = new Vector2(1, 1); uRt.pivot = new Vector2(1, 1);
            UiKit.Size(uRt, 150, 56); UiKit.Pos(uRt, -12, -8);

            return (tray, list, undo);
        }

        static void SetToolField(ToolButton tb, CorrectionTool tool)
        {
            var so = new SerializedObject(tb);
            var p = so.FindProperty("tool");
            if (p != null) { p.enumValueIndex = (int)tool; so.ApplyModifiedPropertiesWithoutUndo(); }
            else Debug.LogWarning("ToolButton 'tool' field not found.");
        }

        static ProofControlsUI BuildLever(Transform safe)
        {
            var go = UiKit.Rect("ProofControls", safe);
            go.anchorMin = new Vector2(1, 0); go.anchorMax = new Vector2(1, 0); go.pivot = new Vector2(1, 0);
            UiKit.Size(go, 420, 300); UiKit.Pos(go, -24, 24);
            var housing = go.gameObject.AddComponent<Image>();
            housing.color = UiKit.Charcoal;

            var lever = UiKit.Button("Lever", go, "", UiKit.CharcoalUp, UiKit.Paper, 1, out _);
            var lRt = (RectTransform)lever.transform;
            UiKit.Stretch(lRt, 18f);

            var handle = UiKit.Image("Handle", lever.transform, UiKit.WarnRed);
            var hRt = (RectTransform)handle.transform;
            hRt.anchorMin = new Vector2(0.5f, 0.1f); hRt.anchorMax = new Vector2(0.5f, 0.1f);
            hRt.pivot = new Vector2(0.5f, 0f);
            UiKit.Size(hRt, 70, 190);
            var knob = UiKit.Image("Knob", handle.transform, UiKit.Metal);
            var kRt = (RectTransform)knob.transform;
            kRt.anchorMin = new Vector2(0.5f, 1); kRt.anchorMax = new Vector2(0.5f, 1); kRt.pivot = new Vector2(0.5f, 1);
            UiKit.Size(kRt, 110, 60); UiKit.Pos(kRt, 0, 20);

            var label = UiKit.Text("Label", go, "RUN PROOF", 28, TextAlignmentOptions.Bottom, UiKit.Paper, FontStyles.Bold);
            Anchor(label, 0.05f, 0.02f, 0.95f, 0.16f);

            var pc = go.gameObject.AddComponent<ProofControlsUI>();
            W.Wire(pc, "leverButton", lever);
            W.Wire(pc, "handle", hRt);
            W.Wire(pc, "label", label);
            return pc;
        }

        static TokenActionPopup BuildPopup(Transform safe)
        {
            var root = UiKit.Rect("TokenActionPopup", safe);
            UiKit.Stretch(root);
            var cg = root.gameObject.AddComponent<CanvasGroup>();
            var popup = root.gameObject.AddComponent<TokenActionPopup>();

            var panel = UiKit.Rect("Panel", root);
            panel.anchorMin = new Vector2(0.5f, 0.5f); panel.anchorMax = new Vector2(0.5f, 0.5f);
            panel.pivot = new Vector2(0.5f, 0.5f); UiKit.Size(panel, 460, 220);
            var pImg = panel.gameObject.AddComponent<Image>();
            pImg.color = UiKit.Charcoal;

            var title = UiKit.Text("Title", panel, "REPLACE WITH", 22, TextAlignmentOptions.Top, UiKit.LampWarm, FontStyles.Bold);
            Anchor(title, 0.05f, 0.72f, 0.95f, 0.96f);
            var chipRow = UiKit.Rect("ChipRow", panel);
            chipRow.anchorMin = new Vector2(0.04f, 0.16f); chipRow.anchorMax = new Vector2(0.96f, 0.68f);
            chipRow.offsetMin = Vector2.zero; chipRow.offsetMax = Vector2.zero;
            var grid = UiKit.Grid(chipRow.gameObject, new Vector2(128, 66), new Vector2(10, 10), 3);

            var close = UiKit.Button("CloseButton", panel, "×", UiKit.WarnRed, UiKit.Paper, 26, out _);
            var cRt = (RectTransform)close.transform;
            cRt.anchorMin = new Vector2(1, 1); cRt.anchorMax = new Vector2(1, 1); cRt.pivot = new Vector2(1, 1);
            UiKit.Size(cRt, 48, 48); UiKit.Pos(cRt, -6, -6);

            var chipPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/UI/Chip.prefab").GetComponent<Button>();

            W.Wire(popup, "panel", panel);
            W.Wire(popup, "canvasGroup", cg);
            W.Wire(popup, "title", title);
            W.Wire(popup, "chipRow", chipRow);
            W.Wire(popup, "chipPrefab", chipPrefab);
            W.Wire(popup, "closeButton", close);
            return popup;
        }

        static Toast BuildToast(Transform safe)
        {
            var go = UiKit.Rect("Toast", safe);
            go.anchorMin = new Vector2(0.5f, 0.34f); go.anchorMax = new Vector2(0.5f, 0.34f);
            go.pivot = new Vector2(0.5f, 0.5f); UiKit.Size(go, 720, 80);
            var cg = go.gameObject.AddComponent<CanvasGroup>();
            cg.alpha = 0f;
            var bg = go.gameObject.AddComponent<Image>();
            bg.color = new Color(0.71f, 0.23f, 0.18f, 0.95f);
            var txt = UiKit.Text("Text", go, "…", 24, TextAlignmentOptions.Center, UiKit.Paper, FontStyles.Bold);
            UiKit.Stretch((RectTransform)txt.transform, 10f);
            var toast = go.gameObject.AddComponent<Toast>();
            W.Wire(toast, "group", cg);
            W.Wire(toast, "text", txt);
            return toast;
        }

        static TutorialLayer BuildTutorial(Transform safe, RectTransform strip, RectTransform tray, RectTransform lever)
        {
            var go = UiKit.Rect("TutorialLayer", safe);
            UiKit.Stretch(go);
            var rootCg = go.gameObject.AddComponent<CanvasGroup>();
            rootCg.blocksRaycasts = false; // the bubble carries its own blocking group
            var tut = go.gameObject.AddComponent<TutorialLayer>();

            var bubble = UiKit.Rect("Bubble", go);
            bubble.anchorMin = new Vector2(0.5f, 0.5f); bubble.anchorMax = new Vector2(0.5f, 0.5f);
            bubble.pivot = new Vector2(0.5f, 0.5f); UiKit.Size(bubble, 660, 190);
            var bubbleCg = bubble.gameObject.AddComponent<CanvasGroup>();
            var bImg = bubble.gameObject.AddComponent<Image>();
            bImg.color = UiKit.Paper;
            UiKit.EdgeFrame("Edge", bubble, UiKit.LampWarm, 6f, 0f);
            var gtab = UiKit.Image("Tab", bubble, UiKit.LampWarm, false);
            var gtRt = (RectTransform)gtab.transform;
            gtRt.anchorMin = new Vector2(0, 1); gtRt.anchorMax = new Vector2(0, 1); gtRt.pivot = new Vector2(0, 0);
            UiKit.Size(gtRt, 168, 36); UiKit.Pos(gtRt, 14, -2);
            var gtabT = UiKit.Text("TabText", gtab.transform, "TRAINING", 16, TextAlignmentOptions.Center, UiKit.Ink, FontStyles.Bold);
            UiKit.Stretch((RectTransform)gtabT.transform); gtabT.characterSpacing = 3f;

            var bText = UiKit.Text("Text", bubble, "…", 23, TextAlignmentOptions.Center, UiKit.Ink, FontStyles.Bold);
            var btRt = (RectTransform)bText.transform;
            btRt.anchorMin = new Vector2(0, 0.28f); btRt.anchorMax = new Vector2(1, 1);
            btRt.offsetMin = new Vector2(24, 0); btRt.offsetMax = new Vector2(-24, -8);

            var cont = UiKit.Button("ContinueButton", bubble, "GOT IT", UiKit.Ink, UiKit.Paper, 18, out var contLabel);
            var cRt = (RectTransform)cont.transform;
            cRt.anchorMin = new Vector2(0.5f, 0f); cRt.anchorMax = new Vector2(0.5f, 0f); cRt.pivot = new Vector2(0.5f, 0f);
            UiKit.Size(cRt, 220, 46); UiKit.Pos(cRt, 0, 14);

            var aStrip  = MakeAnchor("AnchorStrip",  go, new Vector2(0.40f, 0.60f));
            var aTray   = MakeAnchor("AnchorTray",   go, new Vector2(0.31f, 0.42f));
            var aLever  = MakeAnchor("AnchorLever",  go, new Vector2(0.72f, 0.40f));
            var aDefect = MakeAnchor("AnchorDefect", go, new Vector2(0.66f, 0.58f));
            var aBudget = MakeAnchor("AnchorBudget", go, new Vector2(0.60f, 0.82f));

            W.Wire(tut, "group", bubbleCg);
            W.Wire(tut, "bubble", bubble);
            W.Wire(tut, "text", bText);
            W.Wire(tut, "continueButton", cont);
            W.Wire(tut, "continueLabel", contLabel);
            W.Wire(tut, "anchorStrip", aStrip);
            W.Wire(tut, "anchorTray", aTray);
            W.Wire(tut, "anchorLever", aLever);
            W.Wire(tut, "anchorDefect", aDefect);
            W.Wire(tut, "anchorBudget", aBudget);
            return tut;
        }

        static RectTransform MakeAnchor(string name, Transform parent, Vector2 viewport)
        {
            var rt = UiKit.Rect(name, parent);
            rt.anchorMin = viewport; rt.anchorMax = viewport; rt.pivot = new Vector2(0.5f, 0.5f);
            UiKit.Size(rt, 4, 4); UiKit.Pos(rt, 0, 0);
            return rt;
        }

        static PauseScreen BuildPause(Transform safe)
        {
            var go = PanelKit.Plain("PauseLayer", safe, UiKit.PanelBg);
            var pause = go.AddComponent<PauseScreen>();
            PanelKit.Heading("Title", go.transform, "INSPECTION PAUSED",
                new Vector2(0.1f, 0.78f), new Vector2(0.9f, 0.9f), 40, UiKit.Paper);
            var col = UiKit.Rect("Buttons", go.transform);
            col.anchorMin = new Vector2(0.5f, 0.5f); col.anchorMax = new Vector2(0.5f, 0.5f);
            col.pivot = new Vector2(0.5f, 0.5f); UiKit.Size(col, 520, 420);
            UiKit.VLayout(col.gameObject, 18, new RectOffset(0, 0, 0, 0));

            var resume = TallButton(col, "ResumeButton", "RESUME INSPECTION");
            var restart = TallButton(col, "RestartButton", "RESTART JOB");
            var instr = TallButton(col, "InstructionsButton", "INSTRUCTIONS");
            var workshop = TallButton(col, "WorkshopButton", "RETURN TO WORKSHOP");

            W.Wire(pause, "resumeButton", resume);
            W.Wire(pause, "restartButton", restart);
            W.Wire(pause, "instructionsButton", instr);
            W.Wire(pause, "workshopButton", workshop);
            return pause;
        }

        static Button TallButton(RectTransform parent, string name, string label)
        {
            var b = UiKit.Button(name, parent, label, UiKit.CharcoalUp, UiKit.Paper, 24, out _);
            UiKit.Size((RectTransform)b.transform, 520, 84);
            return b;
        }

        static ResultScreen BuildResult(Transform safe)
        {
            var go = PanelKit.Plain("ResultLayer", safe, new Color(0.1f, 0.1f, 0.11f, 0.9f));
            var result = go.AddComponent<ResultScreen>();

            var card = UiKit.Image("Card", go.transform, UiKit.Paper);
            var cardRt = (RectTransform)card.transform;
            cardRt.anchorMin = new Vector2(0.5f, 0.5f); cardRt.anchorMax = new Vector2(0.5f, 0.5f);
            cardRt.pivot = new Vector2(0.5f, 0.5f); UiKit.Size(cardRt, 1000, 640);
            UiKit.EdgeFrame("Edge", card.transform, UiKit.Ink, 4f, 0f);
            UiKit.EdgeFrame("Edge2", card.transform, UiKit.Ink, 2f, 12f);
            UiKit.RegistrationMark(card.transform, new Vector2(0.05f, 0.94f), UiKit.PaperDeep, 22f);
            UiKit.RegistrationMark(card.transform, new Vector2(0.95f, 0.94f), UiKit.PaperDeep, 22f);
            UiKit.RegistrationMark(card.transform, new Vector2(0.05f, 0.06f), UiKit.PaperDeep, 22f);
            UiKit.RegistrationMark(card.transform, new Vector2(0.95f, 0.06f), UiKit.PaperDeep, 22f);

            var head = PanelKit.Heading("Headline", card.transform, "PROOF APPROVED",
                new Vector2(0.06f, 0.80f), new Vector2(0.94f, 0.95f), 46, UiKit.Ink);
            head.characterSpacing = 3f;
            var headRule = UiKit.Image("HeadRule", card.transform, UiKit.Ink, false);
            var hrRt = (RectTransform)headRule.transform;
            hrRt.anchorMin = new Vector2(0.2f, 0.78f); hrRt.anchorMax = new Vector2(0.8f, 0.78f);
            hrRt.sizeDelta = new Vector2(0, 3); hrRt.anchoredPosition = Vector2.zero;
            var reason = UiKit.Text("Reason", card.transform, "…", 24, TextAlignmentOptions.Center, UiKit.InkSoft);
            AnchorChild(reason, card, 0.08f, 0.68f, 0.92f, 0.77f);
            var batchTag = UiKit.Text("BatchTag", card.transform, "BATCH 01 SIGNED OFF", 20,
                TextAlignmentOptions.Center, UiKit.Teal, FontStyles.Bold);
            batchTag.characterSpacing = 3f;
            AnchorChild(batchTag, card, 0.06f, 0.60f, 0.94f, 0.67f);
            batchTag.gameObject.SetActive(false);
            var stats = UiKit.Text("Stats", card.transform, "CORRECTIONS USED  1\nPAR  1", 26, TextAlignmentOptions.Center, UiKit.Ink);
            AnchorChild(stats, card, 0.1f, 0.44f, 0.9f, 0.64f);

            var stamp = UiKit.Rect("RatingStamp", card.transform).gameObject;
            var stRt = (RectTransform)stamp.transform;
            stRt.anchorMin = new Vector2(0.5f, 0.34f); stRt.anchorMax = new Vector2(0.5f, 0.34f);
            stRt.pivot = new Vector2(0.5f, 0.5f); UiKit.Size(stRt, 420, 130);
            stRt.localEulerAngles = new Vector3(0, 0, -6);
            UiKit.EdgeFrame("Ring", stamp.transform, UiKit.WarnRed, 5f, 0f);
            UiKit.EdgeFrame("Ring2", stamp.transform, UiKit.WarnRed, 2f, 10f);
            var rating = UiKit.Text("RatingText", stamp.transform, "MASTER PROOF", 34, TextAlignmentOptions.Center, UiKit.WarnRed, FontStyles.Bold);
            UiKit.Stretch((RectTransform)rating.transform, 14f);
            rating.characterSpacing = 4f;

            var row = UiKit.Rect("Buttons", card.transform);
            row.anchorMin = new Vector2(0.05f, 0.06f); row.anchorMax = new Vector2(0.95f, 0.24f);
            row.offsetMin = Vector2.zero; row.offsetMax = Vector2.zero;
            UiKit.HLayout(row.gameObject, 16, new RectOffset(0, 0, 0, 0), TextAnchor.MiddleCenter);

            var next = WideButton(row, "NextButton", "NEXT JOB", UiKit.Teal);
            var replay = WideButton(row, "ReplayButton", "REPLAY", UiKit.CharcoalUp);
            var jobFiles = WideButton(row, "JobFilesButton", "JOB FILES", UiKit.CharcoalUp);
            var correctAgain = WideButton(row, "CorrectAgainButton", "CORRECT AGAIN", UiKit.Teal);
            var restart = WideButton(row, "RestartButton", "RESTART JOB", UiKit.CharcoalUp);

            W.Wire(result, "headline", head);
            W.Wire(result, "reasonText", reason);
            W.Wire(result, "batchTag", batchTag);
            W.Wire(result, "statsText", stats);
            W.Wire(result, "ratingText", rating);
            W.Wire(result, "ratingStamp", stamp.gameObject);
            W.Wire(result, "nextButton", next);
            W.Wire(result, "replayButton", replay);
            W.Wire(result, "jobFilesButton", jobFiles);
            W.Wire(result, "correctAgainButton", correctAgain);
            W.Wire(result, "restartButton", restart);
            return result;
        }

        static Button WideButton(RectTransform parent, string name, string label, Color bg)
        {
            var b = UiKit.Button(name, parent, label, bg, UiKit.Paper, 22, out _);
            UiKit.Size((RectTransform)b.transform, 260, 84);
            return b;
        }

        static UIPanel BuildInstructions(Transform safe, out Button closeButton)
        {
            var go = PanelKit.Plain("InstructionsPanel", safe, UiKit.Paper);
            var panel = go.AddComponent<UIPanel>();
            UiKit.Keyline(go.transform, UiKit.Ink, 3f, 40f);
            PanelKit.Heading("Title", go.transform, "HOW TO INSPECT",
                new Vector2(0.1f, 0.82f), new Vector2(0.9f, 0.93f), 38, UiKit.Ink);
            var body = UiKit.Text("Body", go.transform,
                "1. Read the printed route strip.\n" +
                "2. Read TODAY'S DEFECT — it tells you exactly what the plate did.\n" +
                "3. Pick a tool, tap the wrong command, choose the fix.\n" +
                "4. Watch your correction budget.\n" +
                "5. Pull RUN PROOF and follow the carriage to the destination.",
                24, TextAlignmentOptions.TopLeft, UiKit.Ink);
            AnchorChild(body, (RectTransform)go.transform, 0.12f, 0.2f, 0.88f, 0.8f);
            var back = UiKit.Button("BackButton", go.transform, "CLOSE", UiKit.CharcoalUp, UiKit.Paper, 22, out _);
            var bRt = (RectTransform)back.transform;
            bRt.anchorMin = new Vector2(0.5f, 0.06f); bRt.anchorMax = new Vector2(0.5f, 0.06f); bRt.pivot = new Vector2(0.5f, 0);
            UiKit.Size(bRt, 280, 64);
            // GameplayFlow wires the click at runtime (a lambda AddListener here
            // is not serialized into the built scene).
            closeButton = back;
            return panel;
        }

        static ConfirmDialog WorkshopDialogClone(Transform safe)
        {
            var go = PanelKit.Plain("DialogLayer", safe, new Color(0, 0, 0, 0.6f));
            var dialog = go.AddComponent<ConfirmDialog>();
            var card = UiKit.Image("Card", go.transform, UiKit.Paper);
            var cardRt = (RectTransform)card.transform;
            cardRt.anchorMin = new Vector2(0.5f, 0.5f); cardRt.anchorMax = new Vector2(0.5f, 0.5f);
            cardRt.pivot = new Vector2(0.5f, 0.5f); UiKit.Size(cardRt, 820, 380);
            UiKit.EdgeFrame("Edge", card.transform, UiKit.Ink, 4f, 0f);
            UiKit.EdgeFrame("Edge2", card.transform, UiKit.Ink, 2f, 12f);
            var title = PanelKit.Heading("Title", card.transform, "RETURN TO WORKSHOP?",
                new Vector2(0.06f, 0.66f), new Vector2(0.94f, 0.9f), 32, UiKit.Ink);
            var body = UiKit.Text("Body", card.transform, "…", 24, TextAlignmentOptions.Center, UiKit.Ink);
            AnchorChild(body, cardRt, 0.08f, 0.32f, 0.92f, 0.64f);
            var confirm = UiKit.Button("ConfirmButton", card.transform, "RETURN", UiKit.WarnRed, UiKit.Paper, 24, out var cl);
            var conRt = (RectTransform)confirm.transform;
            conRt.anchorMin = new Vector2(0.28f, 0.12f); conRt.anchorMax = new Vector2(0.28f, 0.12f); conRt.pivot = new Vector2(0.5f, 0);
            UiKit.Size(conRt, 300, 72);
            var cancel = UiKit.Button("CancelButton", card.transform, "STAY", UiKit.CharcoalUp, UiKit.Paper, 24, out _);
            var canRt = (RectTransform)cancel.transform;
            canRt.anchorMin = new Vector2(0.72f, 0.12f); canRt.anchorMax = new Vector2(0.72f, 0.12f); canRt.pivot = new Vector2(0.5f, 0);
            UiKit.Size(canRt, 300, 72);
            W.Wire(dialog, "titleText", title);
            W.Wire(dialog, "bodyText", body);
            W.Wire(dialog, "confirmLabel", cl);
            W.Wire(dialog, "confirmButton", confirm);
            W.Wire(dialog, "cancelButton", cancel);
            return dialog;
        }

        static void Anchor(Component c, float minX, float minY, float maxX, float maxY)
        {
            var rt = (RectTransform)c.transform;
            rt.anchorMin = new Vector2(minX, minY); rt.anchorMax = new Vector2(maxX, maxY);
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
        }

        static void AnchorChild(Component c, Component parent, float minX, float minY, float maxX, float maxY)
            => Anchor(c, minX, minY, maxX, maxY);

        static void AnchorChild2(Component c, Component parent, float minX, float minY, float maxX, float maxY)
            => Anchor(c, minX, minY, maxX, maxY);
    }
}
