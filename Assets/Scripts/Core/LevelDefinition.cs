using System;
using System.Collections.Generic;
using UnityEngine;

namespace MisprintDepartment.Core
{
    [Serializable]
    public struct SerializableToken
    {
        public PrintCommand command;
        public InkFlags flags;
        public CommandToken ToToken() => new CommandToken(command, flags);
        public static SerializableToken From(CommandToken t) =>
            new SerializableToken { command = t.command, flags = t.flags };
    }

    /// <summary>
    /// Data-driven definition of one puzzle job. One asset per level under
    /// Assets/Content/Levels/ChapterNN/. No hard-coded level logic anywhere.
    /// </summary>
    [CreateAssetMenu(menuName = "Misprint/Level Definition", fileName = "L00_Level")]
    public sealed class LevelDefinition : ScriptableObject
    {
        [Header("Identity")]
        public int levelId = 1;            // 1..42, unique
        public int chapter = 1;            // 1..7
        public string displayTitle = "Untitled Job";

        [Header("Board")]
        public BoardConfig board = new BoardConfig();

        [Header("Command strip (clean — what the machine was told to print)")]
        public List<SerializableToken> cleanStrip = new List<SerializableToken>();

        [Header("Printer defect for this job")]
        [SerializeReference] public DefectRule defect;

        [Header("Corrections")]
        public List<CorrectionTool> allowedTools = new List<CorrectionTool>();
        public int correctionBudget = 3;

        [Tooltip("Designer reference solution — validated by the level validator and tests.")]
        public List<CorrectionOp> referenceSolution = new List<CorrectionOp>();

        [Tooltip("Par correction count for CLEAN PROOF. Optimal <= par earns MASTER PROOF.")]
        public int par = 2;

        [Header("Tutorial / presentation")]
        public List<string> tutorialKeys = new List<string>();

        // ---- helpers ----

        public CommandStrip BuildCleanStrip()
        {
            var s = new CommandStrip();
            foreach (var t in cleanStrip) s.tokens.Add(t.ToToken());
            return s;
        }

        /// <summary>The corrupted strip the player is shown (clean strip run through the defect).</summary>
        public CommandStrip BuildPrintedStrip()
        {
            var clean = BuildCleanStrip();
            return defect != null ? defect.Apply(clean) : clean;
        }

        public HashSet<CorrectionTool> AllowedToolSet() => new HashSet<CorrectionTool>(allowedTools);
    }
}
