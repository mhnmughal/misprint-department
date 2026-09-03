using System;
using UnityEngine;
using UnityEngine.UI;

namespace MisprintDepartment.Game.UI
{
    /// <summary>Inspection-folder pause sheet. Authored in the scene.</summary>
    public sealed class PauseScreen : UIPanel
    {
        [SerializeField] Button resumeButton;
        [SerializeField] Button restartButton;
        [SerializeField] Button instructionsButton;
        [SerializeField] Button workshopButton;

        public event Action Resume, Restart, Instructions, Workshop;

        protected override void Awake()
        {
            base.Awake();
            if (resumeButton != null) resumeButton.onClick.AddListener(() => Resume?.Invoke());
            if (restartButton != null) restartButton.onClick.AddListener(() => Restart?.Invoke());
            if (instructionsButton != null) instructionsButton.onClick.AddListener(() => Instructions?.Invoke());
            if (workshopButton != null) workshopButton.onClick.AddListener(() => Workshop?.Invoke());
        }
    }
}
