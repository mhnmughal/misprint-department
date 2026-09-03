using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using MisprintDepartment.Core;

namespace MisprintDepartment.Editor
{
    public struct LevelIssue
    {
        public LevelDefinition level;
        public string message;
        public bool isError; // false = warning
    }

    /// <summary>
    /// Validates every LevelDefinition asset. Menu: Misprint/Validate All Levels.
    /// Also callable from tests via ValidateAll().
    /// </summary>
    public static class LevelValidator
    {
        [MenuItem("Misprint/Validate All Levels")]
        public static void ValidateAllMenu()
        {
            var issues = ValidateAll(out int count);
            var sb = new StringBuilder();
            sb.AppendLine($"Misprint — validated {count} level(s).");
            if (issues.Count == 0)
            {
                sb.AppendLine("ALL LEVELS PASS.");
                Debug.Log(sb.ToString());
                return;
            }
            foreach (var i in issues)
                sb.AppendLine($"  [{(i.isError ? "ERROR" : "warn")}] L{(i.level ? i.level.levelId.ToString() : "?")}: {i.message}");
            if (issues.Any(x => x.isError)) Debug.LogError(sb.ToString());
            else Debug.LogWarning(sb.ToString());
        }

        public static List<LevelDefinition> LoadAll()
        {
            return AssetDatabase.FindAssets("t:LevelDefinition")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<LevelDefinition>)
                .Where(l => l != null)
                .OrderBy(l => l.levelId)
                .ToList();
        }

        public static List<LevelIssue> ValidateAll(out int count)
        {
            var levels = LoadAll();
            count = levels.Count;
            var issues = new List<LevelIssue>();

            // cross-level: unique ids, chapter coverage
            var byId = new Dictionary<int, LevelDefinition>();
            foreach (var lvl in levels)
            {
                if (byId.ContainsKey(lvl.levelId))
                    issues.Add(Err(lvl, $"duplicate levelId {lvl.levelId} (also {byId[lvl.levelId].name})"));
                else byId[lvl.levelId] = lvl;
            }

            foreach (var lvl in levels)
                issues.AddRange(ValidateOne(lvl));

            return issues;
        }

        public static IEnumerable<LevelIssue> ValidateOne(LevelDefinition lvl)
        {
            var outp = new List<LevelIssue>();

            // levelId 0 / chapter 0 = the Training Job (not one of the 12).
            bool isTraining = lvl.levelId == 0 && lvl.chapter == 0;
            const int Total = 12;   // 6 batches x 2 levels
            const int Batches = 6;

            if (!isTraining)
            {
                if (lvl.levelId < 1 || lvl.levelId > Total) outp.Add(Err(lvl, $"levelId {lvl.levelId} out of 1..{Total}"));
                if (lvl.chapter < 1 || lvl.chapter > Batches) outp.Add(Err(lvl, $"batch {lvl.chapter} out of 1..{Batches}"));
                if (lvl.chapter != (lvl.levelId - 1) / 2 + 1)
                    outp.Add(Warn(lvl, $"levelId {lvl.levelId} does not fall in batch {lvl.chapter}"));
            }
            if (string.IsNullOrWhiteSpace(lvl.displayTitle)) outp.Add(Err(lvl, "empty displayTitle"));

            if (lvl.board == null)
                outp.Add(Err(lvl, "board invalid: null"));
            else if (!lvl.board.IsValid(out string boardErr))
                outp.Add(Err(lvl, "board invalid: " + boardErr));

            if (lvl.cleanStrip == null || lvl.cleanStrip.Count == 0)
            { outp.Add(Err(lvl, "empty clean strip")); return outp; }

            if (lvl.defect == null) outp.Add(Warn(lvl, "no defect rule (clean == printed)"));

            if (lvl.allowedTools == null || lvl.allowedTools.Count == 0)
                outp.Add(Err(lvl, "no allowed correction tools"));
            if (lvl.correctionBudget < 0) outp.Add(Err(lvl, "negative correction budget"));

            // the clean strip alone must solve the board (defect-free sanity)
            var cleanRoute = RouteSimulator.Run(lvl.board, lvl.BuildCleanStrip());
            if (!cleanRoute.success)
                outp.Add(Err(lvl, $"clean strip does not solve the board ({cleanRoute.failReason} @{cleanRoute.failedAtIndex})"));

            // defect must actually change something (except when intentionally none)
            if (lvl.defect != null)
            {
                var printed = lvl.BuildPrintedStrip();
                if (printed.ContentEquals(lvl.BuildCleanStrip()))
                    outp.Add(Warn(lvl, "defect produced no change on this strip"));
            }

            // reference solution must be within budget, use only allowed tools, and pass
            if (lvl.referenceSolution == null || lvl.referenceSolution.Count == 0)
            {
                outp.Add(Err(lvl, "no reference solution"));
            }
            else
            {
                if (lvl.referenceSolution.Count > lvl.correctionBudget)
                    outp.Add(Err(lvl, $"reference solution ({lvl.referenceSolution.Count}) exceeds budget ({lvl.correctionBudget})"));

                var allowed = lvl.AllowedToolSet();
                foreach (var op in lvl.referenceSolution)
                    if (!allowed.Contains(op.tool))
                        outp.Add(Err(lvl, $"reference solution uses disallowed tool {op.tool}"));

                var outcome = LevelJudge.Evaluate(lvl, lvl.referenceSolution);
                if (!outcome.approved)
                    outp.Add(Err(lvl, "reference solution is NOT approved by LevelJudge: " + outcome.message));
                else if (outcome.rating != ProofRating.MasterProof)
                    outp.Add(Warn(lvl, $"reference solution rates {outcome.rating}, expected MasterProof"));
            }

            if (lvl.par < 1) outp.Add(Err(lvl, "par < 1"));
            if (lvl.referenceSolution != null && lvl.referenceSolution.Count > 0 && lvl.par < lvl.referenceSolution.Count)
                outp.Add(Warn(lvl, $"par ({lvl.par}) below reference solution length ({lvl.referenceSolution.Count})"));

            // the uncorrected printed strip should NOT already pass (otherwise there's no puzzle)
            var noFix = LevelJudge.Evaluate(lvl, new List<CorrectionOp>());
            if (noFix.approved)
                outp.Add(Err(lvl, "printed strip already passes with zero corrections — no puzzle"));

            return outp;
        }

        static LevelIssue Err(LevelDefinition l, string m) => new LevelIssue { level = l, message = m, isError = true };
        static LevelIssue Warn(LevelDefinition l, string m) => new LevelIssue { level = l, message = m, isError = false };
    }
}
