using System;
using System.Collections.Generic;

namespace MisprintDepartment.Core
{
    public enum FailReason
    {
        None = 0,
        RouteInvalid,        // ran off-board or into a blocked cell
        TargetMissed,        // finished the strip not on the destination
        InstructionError,    // an illegal / faded token remained after corrections
    }

    /// <summary>One executed token, for animated playback and QA readouts.</summary>
    [Serializable]
    public struct StepTrace
    {
        public int tokenIndex;
        public PrintCommand command;
        public Cell cell;       // carriage cell AFTER this token
        public Direction facing; // facing AFTER this token
        public bool moved;      // did the carriage change cell
        public bool illegal;    // this token caused the failure
        public bool skipped;    // striped — the machine ignored it
    }

    /// <summary>Deterministic result of running a command strip on a board.</summary>
    [Serializable]
    public sealed class RouteResult
    {
        public bool success;
        public FailReason failReason;
        public int failedAtIndex = -1;
        public Cell finalCell;
        public Direction finalFacing;

        /// <summary>Carriage cell after each executed move, starting with the start cell.</summary>
        public List<Cell> path = new List<Cell>();

        /// <summary>One entry per executed token (includes the failing token).</summary>
        public List<StepTrace> trace = new List<StepTrace>();

        public override string ToString() =>
            success
                ? $"PASS end={finalCell} facing={finalFacing} steps={path.Count - 1}"
                : $"FAIL {failReason} @{failedAtIndex} end={finalCell}";
    }

    /// <summary>
    /// Pure, deterministic route runner. No UnityEngine, no randomness, integer math.
    /// The same board + strip always yields an identical RouteResult.
    /// </summary>
    public static class RouteSimulator
    {
        public static RouteResult Run(BoardConfig board, CommandStrip strip)
        {
            if (board == null) throw new ArgumentNullException(nameof(board));
            if (strip == null) throw new ArgumentNullException(nameof(strip));

            var result = new RouteResult();
            Cell pos = board.start;
            Direction facing = board.startFacing;
            result.path.Add(pos);

            for (int i = 0; i < strip.Count; i++)
            {
                CommandToken token = strip[i];

                if (token.Has(InkFlags.Faded))
                {
                    result.trace.Add(Trace(i, token.command, pos, facing, false, true));
                    result.success = false;
                    result.failReason = FailReason.InstructionError;
                    result.failedAtIndex = i;
                    result.finalCell = pos;
                    result.finalFacing = facing;
                    return result;
                }

                // Color Pass: the machine's ink-pass skips striped commands entirely.
                if (token.Has(InkFlags.Striped))
                {
                    result.trace.Add(new StepTrace
                    {
                        tokenIndex = i, command = token.command, cell = pos, facing = facing,
                        moved = false, illegal = false, skipped = true,
                    });
                    continue;
                }

                // Color Pass: marked commands are run by the machine twice.
                int reps = token.Has(InkFlags.Marked) ? 2 : 1;
                for (int rep = 0; rep < reps; rep++)
                {
                Cell before = pos;
                switch (token.command)
                {
                    case PrintCommand.TurnLeft: facing = facing.TurnLeft(); break;
                    case PrintCommand.TurnRight: facing = facing.TurnRight(); break;
                    case PrintCommand.Reverse: facing = facing.Reverse(); break;
                    case PrintCommand.Wait: break;

                    case PrintCommand.Move:
                        pos += facing.Step();
                        if (!board.IsWalkable(pos))
                            return Invalid(result, i, token.command, pos, facing);
                        result.path.Add(pos);
                        break;

                    case PrintCommand.Jump:
                        // advance 2 cells; the middle cell's block state is ignored,
                        // the landing cell must be walkable.
                        pos += facing.Step() * 2;
                        if (!board.IsWalkable(pos))
                            return Invalid(result, i, token.command, pos, facing);
                        result.path.Add(pos);
                        break;
                }
                result.trace.Add(Trace(i, token.command, pos, facing, pos != before, false));
                } // rep
            }

            result.finalCell = pos;
            result.finalFacing = facing;
            if (pos == board.destination)
            {
                result.success = true;
            }
            else
            {
                result.success = false;
                result.failReason = FailReason.TargetMissed;
                result.failedAtIndex = strip.Count - 1;
            }
            return result;
        }

        static StepTrace Trace(int i, PrintCommand c, Cell cell, Direction facing, bool moved, bool illegal) =>
            new StepTrace { tokenIndex = i, command = c, cell = cell, facing = facing, moved = moved, illegal = illegal };

        static RouteResult Invalid(RouteResult r, int i, PrintCommand c, Cell pos, Direction facing)
        {
            r.success = false;
            r.failReason = FailReason.RouteInvalid;
            r.failedAtIndex = i;
            r.finalCell = pos;
            r.finalFacing = facing;
            r.path.Add(pos);
            r.trace.Add(Trace(i, c, pos, facing, true, true));
            return r;
        }
    }
}
