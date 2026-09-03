using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using MisprintDepartment.Core;
using C = MisprintDepartment.Core.PrintCommand;
using T = MisprintDepartment.Core.CorrectionTool;
using SelectionMode = MisprintDepartment.Core.SelectionMode;

namespace MisprintDepartment.Editor
{
    /// <summary>
    /// Authors every LevelDefinition asset in code so they are reproducible and
    /// reviewable. Menu: Misprint/Build Levels/Build All. Each level is validated
    /// by LevelValidator afterwards — check the Console.
    ///
    /// Campaign: Training Job + 12 levels = 6 batches x 2. Each batch introduces
    /// one printer-defect family and one correction tool; batch 6 combines them.
    /// </summary>
    public static class ChapterBuilders
    {
        const string Root = "Assets/Content/Levels";

        [MenuItem("Misprint/Build Levels/Build All (Training + 12)")]
        public static void BuildAllLevels()
        {
            // clean out any pre-restructure folders
            foreach (var stale in new[] { "Chapter01", "Chapter02", "Chapter03", "Chapter04", "Chapter05", "Chapter06", "Chapter07" })
                if (AssetDatabase.IsValidFolder($"{Root}/{stale}"))
                    AssetDatabase.DeleteAsset($"{Root}/{stale}");

            EnsureFolder($"{Root}/Training");
            Save(Training(), $"{Root}/Training/L00_Training_Job.asset");

            var levels = new List<LevelDefinition>
            {
                L01(), L02(),   // Batch 1 — Registration Error   (Replace, Rotate)
                L03(), L04(),   // Batch 2 — Mirror Plate         (Swap)
                L05(), L06(),   // Batch 3 — Double Impression    (Cross Out)
                L07(), L08(),   // Batch 4 — Registration Shift   (Shift)
                L09(), L10(),   // Batch 5 — Faded Ink            (Restore)
                L11(), L12(),   // Batch 6 — Color Pass / Final   (all tools)
            };
            foreach (var lvl in levels)
            {
                EnsureFolder($"{Root}/Batch{lvl.chapter:00}");
                Save(lvl, $"{Root}/Batch{lvl.chapter:00}/L{lvl.levelId:00}_{Slug(lvl.displayTitle)}.asset");
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            LevelValidator.ValidateAllMenu();
        }

        // Kept for the master-build pipeline / old callers.
        public static void BuildChapter1() => BuildAllLevels();

        // ---------------- Training Job (not one of the 12) ----------------

        static LevelDefinition Training()
        {
            var l = New(0, 0, "Training Job");
            l.board = Board(3, 3, new Cell(0, 0), Direction.North, new Cell(2, 2));
            l.cleanStrip = Strip(C.Move, C.Move, C.TurnRight, C.Move, C.Move);
            l.defect = new MirrorDefect { selection = SelectionMode.ExplicitIndices, indices = { 2 } };
            l.allowedTools = new List<T> { T.Replace };
            l.correctionBudget = 2;
            l.par = 1;
            l.referenceSolution = new List<CorrectionOp> { CorrectionOp.DoReplace(2, C.TurnRight) };
            l.tutorialKeys = new List<string> { "training" };
            return l;
        }

        // ================= Batch 1 — Registration Error =================

        static LevelDefinition L01()
        {
            var l = New(1, 1, "First Proof");
            l.board = Board(4, 4, new Cell(0, 0), Direction.North, new Cell(2, 3));
            l.cleanStrip = Strip(C.Move, C.Move, C.Move, C.TurnRight, C.Move, C.Move);
            l.defect = new MirrorDefect { selection = SelectionMode.ExplicitIndices, indices = { 3 } };
            l.allowedTools = new List<T> { T.Replace };
            l.correctionBudget = 2;
            l.par = 1;
            l.referenceSolution = new List<CorrectionOp> { CorrectionOp.DoReplace(3, C.TurnRight) };
            l.tutorialKeys = new List<string> { "tool.replace" };
            return l;
        }

        static LevelDefinition L02()
        {
            var l = New(2, 1, "Rotated Impression");
            l.board = Board(5, 5, new Cell(0, 0), Direction.North, new Cell(3, 3));
            l.cleanStrip = Strip(C.Move, C.Move, C.Move, C.TurnRight, C.Move, C.Move, C.Move);
            l.defect = new RotateDefect { indices = { 3 }, steps = 1 };
            l.allowedTools = new List<T> { T.Rotate, T.Replace };
            l.correctionBudget = 2;
            l.par = 1;
            l.referenceSolution = new List<CorrectionOp> { CorrectionOp.DoRotate(3, -1) };
            l.tutorialKeys = new List<string> { "tool.rotate", "defect.rotate" };
            return l;
        }

        // ================= Batch 2 — Mirror Plate (Swap) =================

        static LevelDefinition L03()
        {
            var l = New(3, 2, "Mirror Plate");
            l.board = Board(5, 5, new Cell(0, 0), Direction.East, new Cell(2, 2));
            l.cleanStrip = Strip(C.Move, C.Move, C.TurnLeft, C.Move, C.Move);
            l.defect = new SwapDefect { a = 2, b = 3 };
            l.allowedTools = new List<T> { T.Swap };
            l.correctionBudget = 2;
            l.par = 1;
            l.referenceSolution = new List<CorrectionOp> { CorrectionOp.DoSwap(2, 3) };
            l.tutorialKeys = new List<string> { "tool.swap", "defect.swap" };
            return l;
        }

        static LevelDefinition L04()
        {
            var l = New(4, 2, "Alternating Plate");
            l.board = Board(6, 5, new Cell(0, 0), Direction.East, new Cell(4, 2));
            l.cleanStrip = Strip(C.Move, C.Move, C.TurnLeft, C.Move, C.Move, C.TurnRight, C.Move, C.Move);
            l.defect = new SwapDefect { a = 2, b = 5 };
            l.allowedTools = new List<T> { T.Swap, T.Replace };
            l.correctionBudget = 2;
            l.par = 1;
            l.referenceSolution = new List<CorrectionOp> { CorrectionOp.DoSwap(2, 5) };
            return l;
        }

        // ================= Batch 3 — Double Impression (Cross Out) =================

        static LevelDefinition L05()
        {
            var l = New(5, 3, "Double Impression");
            l.board = Board(5, 5, new Cell(0, 0), Direction.North, new Cell(2, 4));
            l.cleanStrip = Strip(C.Move, C.Move, C.Move, C.Move, C.TurnRight, C.Move, C.Move);
            l.defect = new DuplicateDefect { trigger = DuplicateTrigger.EveryNth, nth = 3 };
            l.allowedTools = new List<T> { T.CrossOut };
            l.correctionBudget = 3;
            l.par = 2;
            l.referenceSolution = new List<CorrectionOp>
            {
                CorrectionOp.DoCrossOut(4),
                CorrectionOp.DoCrossOut(6),
            };
            l.tutorialKeys = new List<string> { "tool.crossout", "defect.duplicate" };
            return l;
        }

        static LevelDefinition L06()
        {
            var l = New(6, 3, "Ink Marker");
            l.board = Board(5, 5, new Cell(0, 0), Direction.East, new Cell(3, 1));
            l.cleanStrip = Strip(C.Move, C.Wait, C.Move, C.Move, C.TurnLeft, C.Move);
            l.defect = new DuplicateDefect { trigger = DuplicateTrigger.AfterWait };
            l.allowedTools = new List<T> { T.CrossOut };
            l.correctionBudget = 2;
            l.par = 1;
            l.referenceSolution = new List<CorrectionOp> { CorrectionOp.DoCrossOut(3) };
            return l;
        }

        // ================= Batch 4 — Registration Shift (Shift) =================

        static LevelDefinition L07()
        {
            var l = New(7, 4, "Registration Shift");
            l.board = Board(6, 5, new Cell(0, 0), Direction.East, new Cell(4, 2));
            l.cleanStrip = Strip(C.Move, C.Move, C.Move, C.Move, C.TurnLeft, C.Move, C.Move);
            l.defect = new ShiftDefect { selection = SelectionMode.EveryNth, nth = 5, direction = 1 };
            l.allowedTools = new List<T> { T.Shift };
            l.correctionBudget = 2;
            l.par = 1;
            l.referenceSolution = new List<CorrectionOp> { CorrectionOp.DoShift(5, -1) };
            l.tutorialKeys = new List<string> { "tool.shift", "defect.shift" };
            return l;
        }

        static LevelDefinition L08()
        {
            var l = New(8, 4, "Registration Drift");
            l.board = Board(6, 6, new Cell(0, 0), Direction.East, new Cell(4, 3));
            l.cleanStrip = Strip(C.Move, C.Move, C.TurnLeft, C.Move, C.Move, C.Move,
                                 C.TurnRight, C.Move, C.Move);
            l.defect = new CompositeDefect
            {
                rules = new List<DefectRule>
                {
                    new ShiftDefect { selection = SelectionMode.ExplicitIndices, indices = { 2 }, direction = 1 },
                    new SwapDefect { a = 6, b = 8 },
                }
            };
            l.allowedTools = new List<T> { T.Shift, T.Swap };
            l.correctionBudget = 3;
            l.par = 2;
            l.referenceSolution = new List<CorrectionOp>
            {
                CorrectionOp.DoShift(3, -1),
                CorrectionOp.DoSwap(6, 8),
            };
            return l;
        }

        // ================= Batch 5 — Faded Ink (Restore) =================

        static LevelDefinition L09()
        {
            var l = New(9, 5, "Faded Ink");
            l.board = Board(5, 5, new Cell(0, 0), Direction.North, new Cell(2, 2));
            l.cleanStrip = Strip(C.Move, C.Move, C.TurnRight, C.Move, C.Move);
            l.defect = new FadeDefect { indices = { 2 } };
            l.allowedTools = new List<T> { T.Restore };
            l.correctionBudget = 2;
            l.par = 1;
            l.referenceSolution = new List<CorrectionOp> { CorrectionOp.DoRestore(2, C.TurnRight) };
            l.tutorialKeys = new List<string> { "tool.restore", "defect.fade" };
            return l;
        }

        static LevelDefinition L10()
        {
            var l = New(10, 5, "Faded Registration");
            l.board = Board(6, 5, new Cell(0, 0), Direction.East, new Cell(4, 2));
            l.cleanStrip = Strip(C.Move, C.Move, C.TurnLeft, C.Move, C.Move, C.TurnRight, C.Move, C.Move);
            l.defect = new FadeDefect { indices = { 2, 5 } };
            l.allowedTools = new List<T> { T.Restore };
            l.correctionBudget = 3;
            l.par = 2;
            l.referenceSolution = new List<CorrectionOp>
            {
                CorrectionOp.DoRestore(2, C.TurnLeft),
                CorrectionOp.DoRestore(5, C.TurnRight),
            };
            return l;
        }

        // ================= Batch 6 — Color Pass / Final Proof =================

        static LevelDefinition L11()
        {
            var l = New(11, 6, "Color Pass");
            l.board = Board(5, 5, new Cell(0, 0), Direction.North, new Cell(2, 3));
            l.cleanStrip = Strip(C.Move, C.Move, C.Move, C.TurnRight, C.Move, C.Move);
            l.defect = new CompositeDefect
            {
                rules = new List<DefectRule>
                {
                    new ColorPassDefect { pass = InkPass.Striped, indices = { 1 } },
                    new MirrorDefect { selection = SelectionMode.ExplicitIndices, indices = { 3 } },
                }
            };
            l.allowedTools = new List<T> { T.Restore, T.Replace };
            l.correctionBudget = 3;
            l.par = 2;
            l.referenceSolution = new List<CorrectionOp>
            {
                CorrectionOp.DoRestore(1, C.Move),
                CorrectionOp.DoReplace(3, C.TurnRight),
            };
            l.tutorialKeys = new List<string> { "defect.colorpass" };
            return l;
        }

        static LevelDefinition L12()
        {
            var l = New(12, 6, "Final Proof");
            l.board = Board(5, 4, new Cell(0, 0), Direction.East, new Cell(4, 3));
            l.cleanStrip = Strip(C.Move, C.Move, C.TurnLeft, C.Move, C.Move, C.Move,
                                 C.TurnRight, C.Move, C.Move);
            l.defect = new CompositeDefect
            {
                rules = new List<DefectRule>
                {
                    new FadeDefect { indices = { 6 } },
                    new MirrorDefect { selection = SelectionMode.ExplicitIndices, indices = { 2 } },
                    new DuplicateDefect { trigger = DuplicateTrigger.EveryNth, nth = 4 },
                }
            };
            l.allowedTools = new List<T> { T.Replace, T.Rotate, T.Swap, T.CrossOut, T.Shift, T.Restore };
            l.correctionBudget = 5;
            l.par = 4;
            l.referenceSolution = new List<CorrectionOp>
            {
                CorrectionOp.DoReplace(2, C.TurnLeft),
                CorrectionOp.DoCrossOut(3),
                CorrectionOp.DoCrossOut(7),
                CorrectionOp.DoRestore(6, C.TurnRight),
            };
            return l;
        }

        // ---------------- helpers ----------------

        static LevelDefinition New(int id, int chapter, string title)
        {
            var l = ScriptableObject.CreateInstance<LevelDefinition>();
            l.name = $"L{id:00}_{Slug(title)}";
            l.levelId = id;
            l.chapter = chapter;
            l.displayTitle = title;
            return l;
        }

        static BoardConfig Board(int w, int h, Cell start, Direction facing, Cell dest, params Cell[] blocked)
        {
            return new BoardConfig
            {
                width = w, height = h,
                start = start, startFacing = facing, destination = dest,
                blocked = new List<Cell>(blocked),
            };
        }

        static List<SerializableToken> Strip(params C[] commands)
        {
            var list = new List<SerializableToken>();
            foreach (var c in commands)
                list.Add(new SerializableToken { command = c, flags = InkFlags.None });
            return list;
        }

        static void Save(LevelDefinition lvl, string path)
        {
            var existing = AssetDatabase.LoadAssetAtPath<LevelDefinition>(path);
            if (existing != null)
            {
                EditorUtility.CopySerialized(lvl, existing);
                EditorUtility.SetDirty(existing);
                Object.DestroyImmediate(lvl);
            }
            else
            {
                AssetDatabase.CreateAsset(lvl, path);
            }
        }

        static void EnsureFolder(string path)
        {
            var parts = path.Split('/');
            var cur = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                var next = cur + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next))
                    AssetDatabase.CreateFolder(cur, parts[i]);
                cur = next;
            }
        }

        static string Slug(string s)
        {
            var sb = new System.Text.StringBuilder();
            foreach (var ch in s)
                if (char.IsLetterOrDigit(ch)) sb.Append(ch);
                else if (ch == ' ') sb.Append('_');
            return sb.ToString();
        }
    }
}
