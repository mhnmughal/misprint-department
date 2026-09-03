using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MisprintDepartment.Game.UI
{
    /// <summary>Reusable yes/no dialog. Authored once per scene in the DialogLayer.</summary>
    public sealed class ConfirmDialog : UIPanel
    {
        [SerializeField] TMP_Text titleText;
        [SerializeField] TMP_Text bodyText;
        [SerializeField] TMP_Text confirmLabel;
        [SerializeField] Button confirmButton;
        [SerializeField] Button cancelButton;

        Action _onConfirm;

        protected override void Awake()
        {
            base.Awake();
            if (confirmButton != null) confirmButton.onClick.AddListener(() => { var a = _onConfirm; _onConfirm = null; Hide(); a?.Invoke(); });
            if (cancelButton != null) cancelButton.onClick.AddListener(() => { _onConfirm = null; Hide(); });
        }

        public void Ask(string title, string body, string confirmText, Action onConfirm)
        {
            if (titleText != null) titleText.text = title;
            if (bodyText != null) bodyText.text = body;
            if (confirmLabel != null) confirmLabel.text = confirmText;
            _onConfirm = onConfirm;
            Show();
        }
    }
}
