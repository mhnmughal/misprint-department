using System;
using System.Collections.Generic;

namespace MisprintDepartment.Core
{
    public enum CorrectionTool
    {
        Rotate = 0,   // step a token along the command wheel
        Replace = 1,  // set a token to a chosen command
        Swap = 2,     // exchange two slots
        CrossOut = 3, // delete a token
        Shift = 4,    // move a token one slot left/right
        Restore = 5,  // reveal a faded token to a chosen command
    }

    /// <summary>One correction the player applies to the working strip.</summary>
    [Serializable]
    public struct CorrectionOp
    {
        public CorrectionTool tool;
        public int indexA;
        public int indexB;             // Swap
        public PrintCommand payload;   // Replace / Restore target command
        public int amount;             // Rotate steps (signed) / Shift direction (+1/-1)

        public static CorrectionOp DoRotate(int index, int steps) =>
            new CorrectionOp { tool = CorrectionTool.Rotate, indexA = index, amount = steps };
        public static CorrectionOp DoReplace(int index, PrintCommand to) =>
            new CorrectionOp { tool = CorrectionTool.Replace, indexA = index, payload = to };
        public static CorrectionOp DoSwap(int a, int b) =>
            new CorrectionOp { tool = CorrectionTool.Swap, indexA = a, indexB = b };
        public static CorrectionOp DoCrossOut(int index) =>
            new CorrectionOp { tool = CorrectionTool.CrossOut, indexA = index };
        public static CorrectionOp DoShift(int index, int direction) =>
            new CorrectionOp { tool = CorrectionTool.Shift, indexA = index, amount = direction >= 0 ? 1 : -1 };
        public static CorrectionOp DoRestore(int index, PrintCommand to) =>
            new CorrectionOp { tool = CorrectionTool.Restore, indexA = index, payload = to };

        public override string ToString()
        {
            switch (tool)
            {
                case CorrectionTool.Rotate: return $"Rotate #{indexA} by {amount}";
                case CorrectionTool.Replace: return $"Replace #{indexA} -> {payload}";
                case CorrectionTool.Swap: return $"Swap #{indexA} <-> #{indexB}";
                case CorrectionTool.CrossOut: return $"CrossOut #{indexA}";
                case CorrectionTool.Shift: return $"Shift #{indexA} {(amount >= 0 ? "right" : "left")}";
                case CorrectionTool.Restore: return $"Restore #{indexA} -> {payload}";
                default: return "?";
            }
        }
    }

    public sealed class CorrectionResult
    {
        public bool ok;
        public string error;
        public CommandStrip strip;

        public static CorrectionResult Fail(string e) => new CorrectionResult { ok = false, error = e };
        public static CorrectionResult Pass(CommandStrip s) => new CorrectionResult { ok = true, strip = s };
    }

    /// <summary>
    /// Applies correction ops to a working strip. Pure and deterministic.
    /// Validates the tool is allowed and indices are in range; the per-level
    /// budget is enforced by the caller counting ops.
    /// </summary>
    public static class CorrectionEngine
    {
        public static CorrectionResult Apply(CommandStrip working, CorrectionOp op, ICollection<CorrectionTool> allowedTools)
        {
            if (working == null) return CorrectionResult.Fail("No strip.");
            if (allowedTools != null && !allowedTools.Contains(op.tool))
                return CorrectionResult.Fail($"{op.tool} is not available on this job.");

            var s = working.Clone();
            int n = s.Count;

            switch (op.tool)
            {
                case CorrectionTool.Rotate:
                {
                    if (!InRange(op.indexA, n)) return OutOfRange();
                    var t = s[op.indexA];
                    if (t.Has(InkFlags.Faded)) return CorrectionResult.Fail("Cannot rotate a faded command.");
                    if (!t.command.OnWheel()) return CorrectionResult.Fail($"{t.command} is not on the command wheel.");
                    s[op.indexA] = t.AsCommand(t.command.Rotate(op.amount == 0 ? 1 : op.amount));
                    break;
                }
                case CorrectionTool.Replace:
                {
                    if (!InRange(op.indexA, n)) return OutOfRange();
                    var t = s[op.indexA];
                    if (t.Has(InkFlags.Faded)) return CorrectionResult.Fail("Use Restore on a faded command.");
                    s[op.indexA] = t.AsCommand(op.payload);
                    break;
                }
                case CorrectionTool.Swap:
                {
                    if (!InRange(op.indexA, n) || !InRange(op.indexB, n)) return OutOfRange();
                    if (op.indexA == op.indexB) return CorrectionResult.Fail("Swap needs two different slots.");
                    (s[op.indexA], s[op.indexB]) = (s[op.indexB], s[op.indexA]);
                    break;
                }
                case CorrectionTool.CrossOut:
                {
                    if (!InRange(op.indexA, n)) return OutOfRange();
                    s.tokens.RemoveAt(op.indexA);
                    break;
                }
                case CorrectionTool.Shift:
                {
                    if (!InRange(op.indexA, n)) return OutOfRange();
                    int j = op.indexA + (op.amount >= 0 ? 1 : -1);
                    if (!InRange(j, n)) return CorrectionResult.Fail("Cannot shift past the end of the strip.");
                    (s[op.indexA], s[j]) = (s[j], s[op.indexA]);
                    break;
                }
                case CorrectionTool.Restore:
                {
                    if (!InRange(op.indexA, n)) return OutOfRange();
                    var t = s[op.indexA];
                    bool faded = t.Has(InkFlags.Faded);
                    bool striped = t.Has(InkFlags.Striped);
                    if (!faded && !striped)
                        return CorrectionResult.Fail("Nothing to restore — that command is clear.");
                    // Restore lifts obscuring ink (faded / striped). For a faded token the
                    // player also names the command; a striped token keeps its command.
                    var command = faded ? op.payload : t.command;
                    s[op.indexA] = new CommandToken(command, t.flags & ~(InkFlags.Faded | InkFlags.Striped));
                    break;
                }
                default:
                    return CorrectionResult.Fail("Unknown tool.");
            }

            return CorrectionResult.Pass(s);
        }

        /// <summary>Apply a sequence, stopping at the first failure.</summary>
        public static CorrectionResult ApplyAll(CommandStrip working, IEnumerable<CorrectionOp> ops, ICollection<CorrectionTool> allowedTools)
        {
            var s = working.Clone();
            foreach (var op in ops)
            {
                var r = Apply(s, op, allowedTools);
                if (!r.ok) return r;
                s = r.strip;
            }
            return CorrectionResult.Pass(s);
        }

        static bool InRange(int i, int n) => i >= 0 && i < n;
        static CorrectionResult OutOfRange() => CorrectionResult.Fail("Correction target is out of range.");
    }
}
