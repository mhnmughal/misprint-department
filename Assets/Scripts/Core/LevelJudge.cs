using System.Collections.Generic;

namespace MisprintDepartment.Core
{
    public enum ProofRating { None = 0, Verified = 1, CleanProof = 2, MasterProof = 3 }

    public sealed class ProofOutcome
    {
        public bool approved;
        public FailReason failReason;
        public int failedAtIndex = -1;
        public int correctionsUsed;
        public ProofRating rating;
        public RouteResult route;
        public string message;

        /// <summary>The strip after corrections (what the proof actually ran). May be null if a correction op was rejected.</summary>
        public CommandStrip correctedStrip;

        public string ReasonHeadline
        {
            get
            {
                if (approved) return "PROOF APPROVED";
                switch (failReason)
                {
                    case FailReason.RouteInvalid: return "ROUTE INVALID";
                    case FailReason.TargetMissed: return "TARGET MISSED";
                    case FailReason.InstructionError: return "INSTRUCTION ERROR";
                    default: return "REPRINT REQUIRED";
                }
            }
        }
    }

    /// <summary>
    /// Runs a player's corrections + proof and judges the job. Deterministic —
    /// same level + same ops always yields the same ProofOutcome.
    /// </summary>
    public static class LevelJudge
    {
        public static ProofOutcome Evaluate(LevelDefinition level, IReadOnlyList<CorrectionOp> ops)
        {
            var outcome = new ProofOutcome { correctionsUsed = ops?.Count ?? 0 };
            var allowed = level.AllowedToolSet();

            if (outcome.correctionsUsed > level.correctionBudget)
            {
                outcome.approved = false;
                outcome.message = "Correction limit exceeded.";
                return outcome;
            }

            var printed = level.BuildPrintedStrip();
            var applied = CorrectionEngine.ApplyAll(printed, ops ?? new List<CorrectionOp>(), allowed);
            if (!applied.ok)
            {
                outcome.approved = false;
                outcome.failReason = FailReason.InstructionError;
                outcome.message = applied.error;
                return outcome;
            }
            outcome.correctedStrip = applied.strip;

            // any faded token left unrestored is an instruction error
            for (int i = 0; i < applied.strip.Count; i++)
            {
                if (applied.strip[i].Has(InkFlags.Faded))
                {
                    outcome.approved = false;
                    outcome.failReason = FailReason.InstructionError;
                    outcome.failedAtIndex = i;
                    outcome.message = "A faded command was never restored.";
                    return outcome;
                }
            }

            var route = RouteSimulator.Run(level.board, applied.strip);
            outcome.route = route;
            outcome.failReason = route.failReason;
            outcome.failedAtIndex = route.failedAtIndex;

            if (!route.success)
            {
                outcome.approved = false;
                outcome.message = route.failReason == FailReason.RouteInvalid
                    ? "Route invalid — the carriage left the board."
                    : "Target missed — the route did not end at the destination.";
                return outcome;
            }

            outcome.approved = true;
            outcome.message = "Proof approved.";
            outcome.rating = RateApproved(level, outcome.correctionsUsed);
            return outcome;
        }

        static ProofRating RateApproved(LevelDefinition level, int used)
        {
            int optimal = level.referenceSolution != null && level.referenceSolution.Count > 0
                ? level.referenceSolution.Count
                : level.par;
            if (used <= optimal) return ProofRating.MasterProof;
            if (used <= level.par) return ProofRating.CleanProof;
            return ProofRating.Verified;
        }
    }
}
