using MisprintDepartment.Core;

namespace MisprintDepartment.Game.UI
{
    /// <summary>Presentation strings for commands, tools and ratings.</summary>
    public static class CommandVisuals
    {
        public static string Label(PrintCommand c)
        {
            switch (c)
            {
                case PrintCommand.Move: return "MOVE";
                case PrintCommand.TurnLeft: return "TURN LEFT";
                case PrintCommand.TurnRight: return "TURN RIGHT";
                case PrintCommand.Wait: return "WAIT";
                case PrintCommand.Reverse: return "REVERSE";
                case PrintCommand.Jump: return "JUMP";
                default: return c.ToString();
            }
        }

        public static string Short(PrintCommand c)
        {
            switch (c)
            {
                case PrintCommand.Move: return "MOVE";
                case PrintCommand.TurnLeft: return "LEFT";
                case PrintCommand.TurnRight: return "RIGHT";
                case PrintCommand.Wait: return "WAIT";
                case PrintCommand.Reverse: return "REV";
                case PrintCommand.Jump: return "JUMP";
                default: return c.ToString();
            }
        }

        /// <summary>A single glyph that reads at a glance on the strip.
        /// Restricted to characters present in Liberation Sans.</summary>
        public static string Glyph(PrintCommand c)
        {
            switch (c)
            {
                case PrintCommand.Move: return "↑";       // up arrow
                case PrintCommand.TurnLeft: return "←";   // left arrow
                case PrintCommand.TurnRight: return "→";  // right arrow
                case PrintCommand.Wait: return "•";       // bullet
                case PrintCommand.Reverse: return "↓";    // down arrow
                case PrintCommand.Jump: return "↑↑"; // double up
                default: return "?";
            }
        }

        public static string Label(CorrectionTool t)
        {
            switch (t)
            {
                case CorrectionTool.Rotate: return "ROTATE";
                case CorrectionTool.Replace: return "REPLACE";
                case CorrectionTool.Swap: return "SWAP";
                case CorrectionTool.CrossOut: return "CROSS OUT";
                case CorrectionTool.Shift: return "SHIFT";
                case CorrectionTool.Restore: return "RESTORE";
                default: return t.ToString();
            }
        }

        public static string RatingName(ProofRating r)
        {
            switch (r)
            {
                case ProofRating.Verified: return "VERIFIED";
                case ProofRating.CleanProof: return "CLEAN PROOF";
                case ProofRating.MasterProof: return "MASTER PROOF";
                default: return "—";
            }
        }
    }
}
