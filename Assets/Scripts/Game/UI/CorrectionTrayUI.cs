using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MisprintDepartment.Core;

namespace MisprintDepartment.Game.UI
{
    /// <summary>
    /// The bench correction tray. Tool buttons for THIS level's allowed tools
    /// are authored in the scene (all six exist; unused ones hide). Plus Undo.
    /// </summary>
    public sealed class CorrectionTrayUI : MonoBehaviour
    {
        [SerializeField] List<ToolButton> toolButtons = new List<ToolButton>();
        [SerializeField] Button undoButton;

        public event Action<CorrectionTool> ToolSelected;
        public event Action UndoRequested;

        CorrectionTool? _selected;

        void Awake()
        {
            foreach (var tb in toolButtons)
                tb.Clicked += OnToolClicked;
            if (undoButton != null)
                undoButton.onClick.AddListener(() => UndoRequested?.Invoke());
        }

        public void Bind(LevelController lc)
        {
            var allowed = lc.Level.AllowedToolSet();
            foreach (var tb in toolButtons)
                tb.Bind(tb.Tool, allowed.Contains(tb.Tool));
            _selected = null;
            Refresh(lc);
        }

        public CorrectionTool? Selected => _selected;

        public void ClearSelection()
        {
            _selected = null;
            foreach (var tb in toolButtons) tb.SetSelected(false);
        }

        void OnToolClicked(CorrectionTool tool)
        {
            _selected = _selected == tool ? (CorrectionTool?)null : tool;
            foreach (var tb in toolButtons) tb.SetSelected(tb.Tool == _selected);
            if (_selected.HasValue) ToolSelected?.Invoke(_selected.Value);
        }

        public void Refresh(LevelController lc)
        {
            bool canCorrect = !lc.InputLocked && lc.BudgetRemaining > 0;
            foreach (var tb in toolButtons) tb.SetInteractable(canCorrect);
            if (undoButton != null) undoButton.interactable = !lc.InputLocked && lc.Applied.Count > 0;
        }

        public void SetInteractable(bool on)
        {
            foreach (var tb in toolButtons) tb.SetInteractable(on);
            if (undoButton != null) undoButton.interactable = on;
        }
    }
}
