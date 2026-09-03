using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MisprintDepartment.Core;

namespace MisprintDepartment.Game.UI
{
    /// <summary>PROOF APPROVED / REPRINT REQUIRED panel. Authored in the scene.</summary>
    public sealed class ResultScreen : UIPanel
    {
        [SerializeField] TMP_Text headline;
        [SerializeField] TMP_Text reasonText;
        [SerializeField] TMP_Text statsText;
        [SerializeField] TMP_Text ratingText;
        [SerializeField] GameObject ratingStamp;
        [SerializeField] TMP_Text batchTag;

        [Header("Approved buttons")]
        [SerializeField] Button nextButton;
        [SerializeField] Button replayButton;
        [SerializeField] Button jobFilesButton;

        [Header("Failed buttons")]
        [SerializeField] Button correctAgainButton;
        [SerializeField] Button restartButton;

        public event Action Next, Replay, JobFiles, CorrectAgain, Restart;

        protected override void Awake()
        {
            base.Awake();
            Hook(nextButton, () => Next?.Invoke());
            Hook(replayButton, () => Replay?.Invoke());
            Hook(jobFilesButton, () => JobFiles?.Invoke());
            Hook(correctAgainButton, () => CorrectAgain?.Invoke());
            Hook(restartButton, () => Restart?.Invoke());
        }

        static void Hook(Button b, Action a) { if (b != null) b.onClick.AddListener(() => a()); }

        public void ShowOutcome(ProofOutcome o, int par, bool hasNext, string batchSignOff = null)
        {
            if (headline != null) headline.text = o.approved ? "PROOF APPROVED" : "REPRINT REQUIRED";
            if (reasonText != null) reasonText.text = o.message;
            if (batchTag != null)
            {
                bool show = o.approved && !string.IsNullOrEmpty(batchSignOff);
                batchTag.gameObject.SetActive(show);
                if (show) batchTag.text = batchSignOff;
            }
            if (statsText != null)
                statsText.text = $"CORRECTIONS USED  {o.correctionsUsed}\nPAR  {par}";

            bool showRating = o.approved;
            if (ratingText != null)
            {
                ratingText.gameObject.SetActive(showRating);
                ratingText.text = CommandVisuals.RatingName(o.rating);
            }
            if (ratingStamp != null) ratingStamp.SetActive(showRating);

            SetActive(nextButton, o.approved && hasNext);
            SetActive(replayButton, o.approved);
            SetActive(jobFilesButton, o.approved);
            SetActive(correctAgainButton, !o.approved);
            SetActive(restartButton, !o.approved);

            Show();
        }

        static void SetActive(Button b, bool on) { if (b != null) b.gameObject.SetActive(on); }
    }
}
