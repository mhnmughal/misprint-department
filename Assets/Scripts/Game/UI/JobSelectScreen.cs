using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MisprintDepartment.Core;

namespace MisprintDepartment.Game.UI
{
    /// <summary>
    /// Batch 01 job rack. The panel, header and rack container are authored; job
    /// tickets are pooled from one authored prefab (data-driven content).
    /// </summary>
    public sealed class JobSelectScreen : UIPanel
    {
        [SerializeField] TMP_Text batchHeader;
        [SerializeField] RectTransform rack;
        [SerializeField] JobTicket ticketPrefab;
        [SerializeField] Button backButton;

        readonly List<JobTicket> _tickets = new List<JobTicket>();

        public event Action<int> JobChosen;
        public event Action Back;

        protected override void Awake()
        {
            base.Awake();
            if (backButton != null) backButton.onClick.AddListener(() => Back?.Invoke());
        }

        protected override void OnShow() => Rebuild();

        void Rebuild()
        {
            var db = Services.Levels;
            if (db == null) return;
            if (batchHeader != null) batchHeader.text = "JOB FILES — 6 BATCHES";

            var save = Services.Save?.Data;

            // Training Job first (always unlocked, replayable), then all 12 jobs.
            var jobs = new List<LevelDefinition>();
            if (db.trainingLevel != null) jobs.Add(db.trainingLevel);
            for (int i = 0; i < db.Count; i++) jobs.Add(db.ByIndex(i));
            EnsureCount(jobs.Count);

            for (int i = 0; i < _tickets.Count; i++)
            {
                bool used = i < jobs.Count;
                _tickets[i].gameObject.SetActive(used);
                if (!used) continue;

                var lvl = jobs[i];
                bool isTraining = lvl.levelId == 0;
                bool unlocked = isTraining || save == null || save.IsUnlocked(lvl.levelId);
                var rec = save?.Find(lvl.levelId);
                bool completed = rec != null && rec.completed;
                var rating = (ProofRating)(rec?.rating ?? 0);
                _tickets[i].Bind(lvl, unlocked, completed, rating);
            }
        }

        void EnsureCount(int n)
        {
            while (_tickets.Count < n)
            {
                var t = Instantiate(ticketPrefab, rack);
                t.Clicked += id => JobChosen?.Invoke(id);
                _tickets.Add(t);
            }
        }
    }
}
