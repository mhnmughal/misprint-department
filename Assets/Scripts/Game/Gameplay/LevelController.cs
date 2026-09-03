using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MisprintDepartment.Core;

namespace MisprintDepartment.Game
{
    /// <summary>
    /// The gameplay brain. Owns the working strip and correction history for the
    /// active job, runs the proof, and reports the outcome. UI binds to its
    /// events; it never builds UI.
    /// </summary>
    public sealed class LevelController : MonoBehaviour
    {
        [Tooltip("Used when the Gameplay scene is opened directly (no job selected).")]
        [SerializeField] LevelDefinition fallbackLevel;
        [SerializeField] RouteView routeView;
        [SerializeField] float stepSeconds = 0.30f;
        [SerializeField] float startupSeconds = 0.6f;

        public LevelDefinition Level { get; private set; }
        public CommandStrip Working { get; private set; }
        public IReadOnlyList<CorrectionOp> Applied => _applied;
        public int BudgetTotal => Level != null ? Level.correctionBudget : 0;
        public int BudgetRemaining => Level != null ? Level.correctionBudget - _applied.Count : 0;
        public bool ProofRunning { get; private set; }
        public bool InputLocked => ProofRunning;
        public ProofOutcome LastOutcome { get; private set; }

        public event Action OnLevelLoaded;
        public event Action OnStripChanged;
        public event Action OnProofStarted;
        public event Action<int> OnStepHighlight;   // token index, -1 clears
        public event Action<ProofOutcome> OnProofComplete;

        readonly List<CorrectionOp> _applied = new List<CorrectionOp>();
        CommandStrip _printed;
        HashSet<CorrectionTool> _allowed;

        void Start()
        {
            var target = GameSession.PendingLevel != null ? GameSession.PendingLevel : fallbackLevel;
            if (target == null)
            {
                Debug.LogError("[LevelController] No level to load.");
                return;
            }
            Load(target);
        }

        public void Load(LevelDefinition level)
        {
            Level = level;
            _printed = level.BuildPrintedStrip();
            _allowed = level.AllowedToolSet();
            _applied.Clear();
            Working = _printed.Clone();
            ProofRunning = false;
            LastOutcome = null;

            if (routeView != null) routeView.Build(level.board);
            OnLevelLoaded?.Invoke();
            OnStripChanged?.Invoke();
        }

        public bool CanUseTool(CorrectionTool tool) => _allowed != null && _allowed.Contains(tool);

        /// <summary>Try to apply one correction. Returns false + reason on rejection.</summary>
        public bool TryApply(CorrectionOp op, out string error)
        {
            error = null;
            if (InputLocked) { error = "The proof is running."; return false; }
            if (_applied.Count >= Level.correctionBudget) { error = "No corrections left."; return false; }

            var result = CorrectionEngine.Apply(Working, op, _allowed);
            if (!result.ok) { error = result.error; return false; }

            Working = result.strip;
            _applied.Add(op);
            OnStripChanged?.Invoke();
            return true;
        }

        public void UndoLast()
        {
            if (InputLocked || _applied.Count == 0) return;
            _applied.RemoveAt(_applied.Count - 1);
            Rebuild();
        }

        public void RestartJob()
        {
            if (ProofRunning) StopAllCoroutines();
            _applied.Clear();
            Rebuild();
            ProofRunning = false;
            OnStepHighlight?.Invoke(-1);
        }

        void Rebuild()
        {
            var s = _printed.Clone();
            var r = CorrectionEngine.ApplyAll(s, _applied, _allowed);
            Working = r.ok ? r.strip : _printed.Clone();
            OnStripChanged?.Invoke();
        }

        public void RunProof()
        {
            if (ProofRunning || Level == null) return;
            StartCoroutine(ProofRoutine());
        }

        IEnumerator ProofRoutine()
        {
            ProofRunning = true;
            OnProofStarted?.Invoke();
            Services.Audio?.PlaySfx(Sfx.ProofLever);

            var outcome = LevelJudge.Evaluate(Level, _applied);
            LastOutcome = outcome;

            yield return new WaitForSeconds(startupSeconds);
            Services.Audio?.PlaySfx(Sfx.PrinterStart);

            var route = outcome.route;
            if (route != null && route.trace.Count > 0)
            {
                if (routeView != null) routeView.ResetCarriage(Level.board);
                foreach (var step in route.trace)
                {
                    OnStepHighlight?.Invoke(step.tokenIndex);
                    if (routeView != null)
                        yield return routeView.PlayStep(step, stepSeconds);
                    else
                        yield return new WaitForSeconds(stepSeconds);

                    if (step.illegal) Services.Audio?.PlaySfx(Sfx.ProofRejected);
                    else if (step.moved) Services.Audio?.PlaySfx(Sfx.RouteStep);
                    else Services.Audio?.PlaySfx(Sfx.RouteTurn);
                }
            }

            OnStepHighlight?.Invoke(-1);
            yield return new WaitForSeconds(0.25f);

            Services.Audio?.PlaySfx(outcome.approved ? Sfx.ProofApproved : Sfx.ProofRejected);
            Services.Audio?.Vibrate();
            ProofRunning = false;
            OnProofComplete?.Invoke(outcome);
        }
    }
}
