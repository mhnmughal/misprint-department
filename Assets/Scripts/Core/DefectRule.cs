using System;
using System.Collections.Generic;
using UnityEngine;

namespace MisprintDepartment.Core
{
    /// <summary>
    /// A printer defect. Deterministically transforms the clean strip the
    /// machine was told to print into the corrupted strip the player sees.
    /// Concrete rules are stored polymorphically via [SerializeReference].
    /// </summary>
    [Serializable]
    public abstract class DefectRule
    {
        /// <summary>Short player-facing description of the fault (the "Defect Rule card").</summary>
        public abstract string Describe();

        /// <summary>Returns a NEW strip; never mutates the input.</summary>
        public abstract CommandStrip Apply(CommandStrip clean);
    }

    public enum SelectionMode { All, EveryNth, Alternating, ExplicitIndices }

    static class DefectSelection
    {
        /// <summary>0-based indices selected from a strip of the given length.</summary>
        public static IEnumerable<int> Select(SelectionMode mode, int length, int n, IList<int> explicitIndices)
        {
            switch (mode)
            {
                case SelectionMode.All:
                    for (int i = 0; i < length; i++) yield return i;
                    break;
                case SelectionMode.EveryNth:
                    if (n < 1) n = 1;
                    for (int i = n - 1; i < length; i += n) yield return i;
                    break;
                case SelectionMode.Alternating:
                    for (int i = 0; i < length; i += 2) yield return i;
                    break;
                case SelectionMode.ExplicitIndices:
                    if (explicitIndices != null)
                        foreach (int i in explicitIndices)
                            if (i >= 0 && i < length) yield return i;
                    break;
            }
        }
    }

    // ---- Chapter 1: Rotate ----
    [Serializable]
    public sealed class RotateDefect : DefectRule
    {
        public SelectionMode selection = SelectionMode.ExplicitIndices;
        public int nth = 3;
        public List<int> indices = new List<int>();
        public int steps = 1; // steps along the command wheel

        public override string Describe() =>
            $"Some commands are rotated {steps} step(s) on the wheel.";

        public override CommandStrip Apply(CommandStrip clean)
        {
            var s = clean.Clone();
            foreach (int i in DefectSelection.Select(selection, s.Count, nth, indices))
                s[i] = s[i].AsCommand(s[i].command.Rotate(steps));
            return s;
        }
    }

    // ---- Chapter 2: Mirror Plate ----
    [Serializable]
    public sealed class MirrorDefect : DefectRule
    {
        public SelectionMode selection = SelectionMode.All;
        public int nth = 2;
        public List<int> indices = new List<int>();
        public bool mirrorMoveReverse = false; // also swap Move<->Reverse

        public override string Describe() =>
            "Turn Left and Turn Right are mirrored" + (mirrorMoveReverse ? "; Move and Reverse are mirrored." : ".");

        public override CommandStrip Apply(CommandStrip clean)
        {
            var s = clean.Clone();
            foreach (int i in DefectSelection.Select(selection, s.Count, nth, indices))
                s[i] = s[i].AsCommand(Mirror(s[i].command));
            return s;
        }

        PrintCommand Mirror(PrintCommand c)
        {
            switch (c)
            {
                case PrintCommand.TurnLeft: return PrintCommand.TurnRight;
                case PrintCommand.TurnRight: return PrintCommand.TurnLeft;
                case PrintCommand.Move: return mirrorMoveReverse ? PrintCommand.Reverse : c;
                case PrintCommand.Reverse: return mirrorMoveReverse ? PrintCommand.Move : c;
                default: return c;
            }
        }
    }

    // ---- Chapter 3: Double Impression ----
    public enum DuplicateTrigger { EveryNth, AfterWait, MarkedInk }

    [Serializable]
    public sealed class DuplicateDefect : DefectRule
    {
        public DuplicateTrigger trigger = DuplicateTrigger.EveryNth;
        public int nth = 3;

        public override string Describe()
        {
            switch (trigger)
            {
                case DuplicateTrigger.AfterWait: return "Every command printed right after a WAIT is duplicated.";
                case DuplicateTrigger.MarkedInk: return "Every command in marked ink is duplicated.";
                default: return $"Every {Ordinal(nth)} command is duplicated.";
            }
        }

        public override CommandStrip Apply(CommandStrip clean)
        {
            var outp = new CommandStrip();
            for (int i = 0; i < clean.Count; i++)
            {
                var t = clean[i];
                outp.tokens.Add(t);
                bool dup;
                switch (trigger)
                {
                    case DuplicateTrigger.AfterWait:
                        dup = i > 0 && clean[i - 1].command == PrintCommand.Wait;
                        break;
                    case DuplicateTrigger.MarkedInk:
                        dup = t.Has(InkFlags.Marked);
                        break;
                    default:
                        dup = nth >= 1 && ((i + 1) % nth == 0);
                        break;
                }
                if (dup) outp.tokens.Add(t);
            }
            return outp;
        }

        static string Ordinal(int n) => n == 2 ? "2nd" : n == 3 ? "3rd" : n + "th";
    }

    // ---- Chapter 4: Registration Shift ----
    [Serializable]
    public sealed class ShiftDefect : DefectRule
    {
        public SelectionMode selection = SelectionMode.EveryNth;
        public int nth = 4;
        public List<int> indices = new List<int>();
        public int direction = 1; // +1 = shifts one slot right (swap with next)

        public override string Describe() =>
            $"Every {nth}th command drifts one slot {(direction >= 0 ? "right" : "left")}.";

        public override CommandStrip Apply(CommandStrip clean)
        {
            var s = clean.Clone();
            var sel = new List<int>(DefectSelection.Select(selection, s.Count, nth, indices));
            // apply left-to-right for right drift, right-to-left for left drift
            if (direction >= 0) sel.Sort();
            else sel.Sort((a, b) => b.CompareTo(a));
            foreach (int i in sel)
            {
                int j = i + (direction >= 0 ? 1 : -1);
                if (j < 0 || j >= s.Count) continue;
                (s[i], s[j]) = (s[j], s[i]);
            }
            return s;
        }
    }

    // ---- Chapter 5: Faded Ink ----
    [Serializable]
    public sealed class FadeDefect : DefectRule
    {
        public List<int> indices = new List<int>();

        public override string Describe() => "Some commands are faded and unreadable.";

        public override CommandStrip Apply(CommandStrip clean)
        {
            var s = clean.Clone();
            foreach (int i in indices)
                if (i >= 0 && i < s.Count) s[i] = s[i].With(InkFlags.Faded);
            return s;
        }
    }

    // ---- Chapter 6: Color Pass ----
    public enum InkPass { Striped, Marked }

    [Serializable]
    public sealed class ColorPassDefect : DefectRule
    {
        public InkPass pass = InkPass.Striped;
        public List<int> indices = new List<int>();

        public override string Describe() =>
            pass == InkPass.Striped
                ? "Striped commands are skipped by the machine — clear the stripe to run them."
                : "Marked commands are run twice by the machine.";

        public override CommandStrip Apply(CommandStrip clean)
        {
            var flag = pass == InkPass.Striped ? InkFlags.Striped : InkFlags.Marked;
            var s = clean.Clone();
            foreach (int i in indices)
                if (i >= 0 && i < s.Count) s[i] = s[i].With(flag);
            return s;
        }
    }

    // ---- swap two explicit slots (used across chapters) ----
    [Serializable]
    public sealed class SwapDefect : DefectRule
    {
        public int a = 0;
        public int b = 1;

        public override string Describe() => $"Slots {a + 1} and {b + 1} are swapped.";

        public override CommandStrip Apply(CommandStrip clean)
        {
            var s = clean.Clone();
            if (a >= 0 && b >= 0 && a < s.Count && b < s.Count)
                (s[a], s[b]) = (s[b], s[a]);
            return s;
        }
    }

    // ---- Chapter 7: Final Proof ----
    [Serializable]
    public sealed class CompositeDefect : DefectRule
    {
        [SerializeReference]
        public List<DefectRule> rules = new List<DefectRule>();

        public override string Describe()
        {
            var parts = new List<string>();
            foreach (var r in rules) if (r != null) parts.Add(r.Describe());
            return string.Join(" ", parts);
        }

        public override CommandStrip Apply(CommandStrip clean)
        {
            var s = clean;
            foreach (var r in rules)
                if (r != null) s = r.Apply(s);
            return s;
        }
    }
}
