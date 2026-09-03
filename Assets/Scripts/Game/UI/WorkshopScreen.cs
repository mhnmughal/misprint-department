using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MisprintDepartment.Game.UI
{
    /// <summary>
    /// Main interaction layer of the workshop desk. Authored in the scene: a
    /// primary proof-sheet control plus rack / manual / maintenance controls.
    /// </summary>
    public sealed class WorkshopScreen : UIPanel
    {
        [SerializeField] Button startButton;
        [SerializeField] TMP_Text startLabel;
        [SerializeField] TMP_Text currentJobLabel;
        [SerializeField] Button jobFilesButton;
        [SerializeField] Button manualButton;
        [SerializeField] Button settingsButton;
        [SerializeField] Button creditsButton;

        public event Action StartInspection, JobFiles, Manual, Settings, Credits;

        protected override void Awake()
        {
            base.Awake();
            if (startButton != null) startButton.onClick.AddListener(() => StartInspection?.Invoke());
            if (jobFilesButton != null) jobFilesButton.onClick.AddListener(() => JobFiles?.Invoke());
            if (manualButton != null) manualButton.onClick.AddListener(() => Manual?.Invoke());
            if (settingsButton != null) settingsButton.onClick.AddListener(() => Settings?.Invoke());
            if (creditsButton != null) creditsButton.onClick.AddListener(() => Credits?.Invoke());
        }

        protected override void OnShow() => RefreshStatus();

        public void RefreshStatus()
        {
            var save = Services.Save?.Data;
            var db = Services.Levels;
            int currentId = CurrentJobId;
            bool trainingDone = save == null || save.tutorialsSeen.Contains("training");
            bool anyRealProgress = save != null && save.levels.Exists(r => r.levelId >= 1 && r.completed);

            if (startLabel != null)
                startLabel.text = !trainingDone ? "START TRAINING"
                    : anyRealProgress ? "CONTINUE INSPECTION" : "START INSPECTION";

            if (currentJobLabel != null)
            {
                if (!trainingDone)
                {
                    currentJobLabel.text = "NEXT: TRAINING JOB — LEARN THE BENCH";
                }
                else
                {
                    var lvl = db != null ? db.ById(currentId) : null;
                    currentJobLabel.text = lvl != null
                        ? $"NEXT: JOB {lvl.levelId:000} — {lvl.displayTitle.ToUpperInvariant()}"
                        : $"NEXT: JOB {currentId:000}";
                }
            }
        }

        /// <summary>0 = the Training Job (until it's been done), otherwise the highest unlocked job.</summary>
        public int CurrentJobId
        {
            get
            {
                var save = Services.Save?.Data;
                if (save == null) return 0;
                return save.tutorialsSeen.Contains("training") ? save.highestUnlockedLevel : 0;
            }
        }
    }
}
