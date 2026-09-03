using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MisprintDepartment.Game.UI
{
    /// <summary>Top HUD: job id, correction budget, pause button. Authored in scene.</summary>
    public sealed class GameplayHUD : MonoBehaviour
    {
        [SerializeField] TMP_Text jobIdText;
        [SerializeField] TMP_Text jobTitleText;
        [SerializeField] TMP_Text budgetText;
        [SerializeField] Button pauseButton;

        public void WirePause(System.Action onPause)
        {
            if (pauseButton != null) pauseButton.onClick.AddListener(() => onPause?.Invoke());
        }

        public void BindLevel(LevelController lc)
        {
            if (jobIdText != null) jobIdText.text = $"JOB {lc.Level.levelId:000}";
            if (jobTitleText != null) jobTitleText.text = lc.Level.displayTitle.ToUpperInvariant();
            RefreshBudget(lc);
        }

        public void RefreshBudget(LevelController lc)
        {
            if (budgetText != null)
                budgetText.text = $"CORRECTIONS  {lc.BudgetRemaining}/{lc.BudgetTotal}";
        }

        public void SetInteractable(bool on)
        {
            if (pauseButton != null) pauseButton.interactable = on;
        }
    }
}
