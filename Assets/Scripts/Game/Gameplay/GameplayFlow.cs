using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MisprintDepartment.Core;
using MisprintDepartment.Game.UI;

namespace MisprintDepartment.Game
{
    /// <summary>
    /// Wires the authored Gameplay UI to the LevelController and owns the
    /// job-to-job flow (next / replay / restart / return / save). Creates no UI.
    /// </summary>
    public sealed class GameplayFlow : MonoBehaviour
    {
        [SerializeField] LevelController level;
        [SerializeField] GameplayHUD hud;
        [SerializeField] DefectCardUI defectCard;
        [SerializeField] InstructionStripUI strip;
        [SerializeField] CorrectionTrayUI tray;
        [SerializeField] TokenActionPopup popup;
        [SerializeField] ProofControlsUI proofControls;
        [SerializeField] ResultScreen result;
        [SerializeField] PauseScreen pause;
        [SerializeField] TutorialLayer tutorial;
        [SerializeField] ConfirmDialog dialog;
        [SerializeField] Toast toast;
        [SerializeField] UIPanel instructionsPanel;

        static readonly PrintCommand[] ReplaceOptions =
        {
            PrintCommand.Move, PrintCommand.TurnLeft, PrintCommand.TurnRight,
            PrintCommand.Reverse, PrintCommand.Jump, PrintCommand.Wait,
        };

        void Awake()
        {
            Services.EnsureForEditor();

            level.OnLevelLoaded += OnLevelLoaded;
            level.OnStripChanged += OnStripChanged;
            level.OnProofStarted += OnProofStarted;
            level.OnStepHighlight += i => strip.SetProofHighlight(i);
            level.OnProofComplete += OnProofComplete;

            tray.ToolSelected += OnToolSelected;
            tray.UndoRequested += () => level.UndoLast();
            strip.TokenClicked += OnTokenClicked;
            proofControls.Fired += OnProofFired;

            hud.WirePause(OpenPause);

            result.Next += GoNext;
            result.Replay += Replay;
            result.JobFiles += ReturnToWorkshop;
            result.CorrectAgain += CorrectAgain;
            result.Restart += Replay;

            pause.Resume += ClosePause;
            pause.Restart += () => { ClosePause(); Replay(); };
            pause.Instructions += () => instructionsPanel?.Show();
            pause.Workshop += () => dialog.Ask("Return to Workshop?",
                "This job is not finished. Progress on it will be lost.",
                "Return", ReturnToWorkshop);
        }

        void OnLevelLoaded()
        {
            hud.BindLevel(level);
            defectCard.Bind(level.Level);
            tray.Bind(level);
            strip.Render(level.Working);
            popup.Close();
            result.SetInstant(false);
            pause.SetInstant(false);

            bool isTraining = level.Level.levelId == 0;
            if (isTraining && tutorial != null) tutorial.Begin();
            else if (tutorial != null) tutorial.EndImmediately();

            if (!isTraining) ShowFirstUseHint();
        }

        static readonly Dictionary<string, string> HintText = new Dictionary<string, string>
        {
            { "tool.rotate",     "NEW TOOL — ROTATE steps a command around the wheel." },
            { "tool.swap",       "NEW TOOL — SWAP exchanges two commands. Tap one, then the other." },
            { "tool.crossout",   "NEW TOOL — CROSS OUT deletes a stray command." },
            { "tool.shift",      "NEW TOOL — SHIFT nudges a command one slot left or right." },
            { "tool.restore",    "NEW TOOL — RESTORE lifts faded or striped ink." },
            { "defect.duplicate","NEW DEFECT — the plate is printing some commands twice." },
            { "defect.shift",    "NEW DEFECT — the plate is drifting commands between slots." },
            { "defect.fade",     "NEW DEFECT — some commands came out faded. Deduce them." },
            { "defect.colorpass","NEW DEFECT — striped commands are skipped by the ink pass." },
        };

        void ShowFirstUseHint()
        {
            var save = Services.Save?.Data;
            if (save == null || level.Level.tutorialKeys == null) return;
            foreach (var key in level.Level.tutorialKeys)
            {
                if (save.tutorialsSeen.Contains(key)) continue;
                save.tutorialsSeen.Add(key);
                if (HintText.TryGetValue(key, out var msg))
                {
                    toast.Show(msg, 3.2f);
                    Services.Save.Save();
                    return; // one hint per job
                }
                Services.Save.Save();
            }
        }

        void OnStripChanged()
        {
            strip.Render(level.Working);
            hud.RefreshBudget(level);
            tray.Refresh(level);
        }

        int _swapFirst = -1; // Swap tool: index of the first tapped token

        void OnToolSelected(CorrectionTool tool)
        {
            popup.Close();
            strip.ClearSelection();
            _swapFirst = -1;
            if (tool == CorrectionTool.Replace) tutorial?.CompleteStep(2);
            Services.Audio?.PlayUi(Sfx.UiSelect);
        }

        void OnTokenClicked(int index)
        {
            if (level.InputLocked) return;
            var selected = tray.Selected;
            if (selected == null)
            {
                toast.Show("Pick a correction tool first.");
                return;
            }
            if (level.BudgetRemaining <= 0)
            {
                toast.Show("No corrections left — undo one first.");
                return;
            }

            var pos = strip.TokenScreenPosition(index);
            var token = index >= 0 && index < level.Working.Count ? level.Working[index] : default;

            switch (selected.Value)
            {
                case CorrectionTool.Rotate:
                    strip.SetSelected(index);
                    popup.OpenRotate(index, pos, ApplyOp);
                    break;

                case CorrectionTool.Replace:
                    strip.SetSelected(index);
                    popup.OpenReplace(index, ReplaceOptions, pos, ApplyOp);
                    break;

                case CorrectionTool.Shift:
                    strip.SetSelected(index);
                    popup.OpenShift(index, pos, ApplyOp);
                    break;

                case CorrectionTool.CrossOut:
                    ApplyOp(CorrectionOp.DoCrossOut(index));
                    break;

                case CorrectionTool.Restore:
                    if (token.Has(InkFlags.Faded))
                    {
                        strip.SetSelected(index);
                        popup.OpenRestore(index, ReplaceOptions, pos, ApplyOp);
                    }
                    else if (token.Has(InkFlags.Striped))
                    {
                        ApplyOp(CorrectionOp.DoRestore(index, token.command));
                    }
                    else
                    {
                        toast.Show("Nothing to restore on that command.");
                    }
                    break;

                case CorrectionTool.Swap:
                    if (_swapFirst < 0)
                    {
                        _swapFirst = index;
                        strip.SetSelected(index);
                        toast.Show("Now tap the command to swap it with.");
                    }
                    else if (_swapFirst == index)
                    {
                        _swapFirst = -1;
                        strip.ClearSelection();
                    }
                    else
                    {
                        int a = _swapFirst;
                        _swapFirst = -1;
                        ApplyOp(CorrectionOp.DoSwap(a, index));
                    }
                    break;
            }
        }

        void ApplyOp(CorrectionOp op)
        {
            if (!level.TryApply(op, out string error))
            {
                toast.Show(error);
                return;
            }
            Services.Audio?.PlaySfx(op.tool == CorrectionTool.Rotate ? Sfx.CorrectionRotate : Sfx.CorrectionReplace);
            Services.Audio?.Vibrate();
            _swapFirst = -1;
            strip.ClearSelection();
            // keep the tool selected so multiple corrections chain smoothly;
            // it clears out when budget runs dry or the player switches tools.
            if (level.BudgetRemaining <= 0) tray.ClearSelection();
            tray.Refresh(level);
            tutorial?.CompleteStep(3);
        }

        void OnProofFired()
        {
            if (level.InputLocked) return;
            popup.Close();
            tray.ClearSelection();
            strip.ClearSelection();
            tutorial?.CompleteStep(5);
            level.RunProof();
        }

        void OnProofStarted()
        {
            SetInputLocked(true);
            Services.Audio?.PlaySfx(Sfx.RollerFeed);
        }

        void OnProofComplete(ProofOutcome outcome)
        {
            SetInputLocked(false);

            if (Services.Save != null)
            {
                var save = Services.Save.Data;
                bool changed = save.RecordResult(
                    level.Level.levelId, outcome.approved, outcome.rating,
                    outcome.correctionsUsed, Services.Levels != null ? Services.Levels.TotalLevels : 12);

                if (outcome.approved && level.Level.levelId == 0 && !save.tutorialsSeen.Contains("training"))
                {
                    save.tutorialsSeen.Add("training");
                    tutorial?.EndImmediately();
                    changed = true;
                }
                if (changed) Services.Save.Save();
            }

            bool hasNext = Services.Levels != null && Services.Levels.Next(level.Level.levelId) != null;

            string batchSignOff = null;
            int id = level.Level.levelId;
            if (outcome.approved && id >= 1 && id % UI.BatchInfo.LevelsPerBatch == 0)
            {
                int batch = level.Level.chapter;
                batchSignOff = batch >= UI.BatchInfo.Batches
                    ? "FINAL PROOF SIGNED OFF — ALL BATCHES CLEARED"
                    : $"BATCH {batch:00} SIGNED OFF — {UI.BatchInfo.Name(batch).ToUpperInvariant()}";
            }

            result.ShowOutcome(outcome, level.Level.par, hasNext, batchSignOff);
        }

        void SetInputLocked(bool locked)
        {
            hud.SetInteractable(!locked);
            tray.SetInteractable(!locked);
            strip.SetInteractable(!locked);
            proofControls.SetInteractable(!locked);
            if (locked) popup.Close();
        }

        // ---- flow ----

        void GoNext()
        {
            var next = Services.Levels?.Next(level.Level.levelId);
            if (next == null) { ReturnToWorkshop(); return; }
            GameSession.PendingLevel = next;
            SceneManager.LoadScene(SceneNames.Gameplay);
        }

        void Replay()
        {
            result.Hide();
            level.RestartJob();
            SetInputLocked(false);
        }

        void CorrectAgain()
        {
            result.Hide();
            SetInputLocked(false);
        }

        void ReturnToWorkshop()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneNames.MainWorkshop);
        }

        void OpenPause()
        {
            if (level.InputLocked) return;
            Time.timeScale = 0f;
            pause.Show();
        }

        void ClosePause()
        {
            Time.timeScale = 1f;
            pause.Hide();
        }
    }
}
