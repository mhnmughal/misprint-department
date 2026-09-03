using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MisprintDepartment.Core;

namespace MisprintDepartment.Game.UI
{
    /// <summary>One correction-tool button in the tray. Prefab authored in scene;
    /// the scene builder sets <see cref="tool"/> per instance.</summary>
    public sealed class ToolButton : MonoBehaviour
    {
        [SerializeField] CorrectionTool tool;
        [SerializeField] Button button;
        [SerializeField] TMP_Text label;
        [SerializeField] GameObject selectedState;
        [SerializeField] CanvasGroup canvasGroup;

        public CorrectionTool Tool => tool;
        public event Action<CorrectionTool> Clicked;

        void Awake()
        {
            if (button != null) button.onClick.AddListener(() => Clicked?.Invoke(tool));
        }

        public void Bind(CorrectionTool assignedTool, bool available)
        {
            tool = assignedTool;
            if (label != null) label.text = CommandVisuals.Label(tool);
            gameObject.SetActive(available);
            SetSelected(false);
        }

        public void SetSelected(bool on)
        {
            if (selectedState != null) selectedState.SetActive(on);
        }

        public void SetInteractable(bool on)
        {
            if (button != null) button.interactable = on;
            if (canvasGroup != null) canvasGroup.alpha = on ? 1f : 0.45f;
        }
    }
}
