using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MisprintDepartment.Core;

namespace MisprintDepartment.Game.UI
{
    /// <summary>One industrial job ticket in the Job Files rack. Prefab authored in scene.</summary>
    public sealed class JobTicket : MonoBehaviour
    {
        [SerializeField] Button button;
        [SerializeField] TMP_Text jobNumber;
        [SerializeField] TMP_Text jobTitle;
        [SerializeField] TMP_Text batchLabel;
        [SerializeField] TMP_Text ratingText;
        [SerializeField] GameObject lockedOverlay;
        [SerializeField] GameObject completeStamp;

        public int LevelId { get; private set; }
        public event Action<int> Clicked;

        void Awake()
        {
            if (button != null) button.onClick.AddListener(() => Clicked?.Invoke(LevelId));
        }

        public void Bind(LevelDefinition level, bool unlocked, bool completed, ProofRating rating)
        {
            LevelId = level.levelId;
            if (jobNumber != null) jobNumber.text = level.levelId == 0 ? "TR" : $"{level.levelId:000}";
            if (jobTitle != null) jobTitle.text = unlocked ? level.displayTitle.ToUpperInvariant() : "LOCKED";
            if (batchLabel != null)
                batchLabel.text = level.levelId == 0 ? "TRAINING" : $"BATCH {level.chapter} · {BatchInfo.Name(level.chapter).ToUpperInvariant()}";
            if (ratingText != null)
            {
                ratingText.gameObject.SetActive(completed);
                ratingText.text = CommandVisuals.RatingName(rating);
            }
            if (lockedOverlay != null) lockedOverlay.SetActive(!unlocked);
            if (completeStamp != null) completeStamp.SetActive(completed);
            if (button != null) button.interactable = unlocked;
        }
    }
}
