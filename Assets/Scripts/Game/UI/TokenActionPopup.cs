using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MisprintDepartment.Core;

namespace MisprintDepartment.Game.UI
{
    /// <summary>
    /// Small contextual popup shown next to a tapped token: rotate direction for
    /// the Rotate tool, or a replacement command for the Replace tool. The panel
    /// and one chip prefab are authored in the scene.
    /// </summary>
    public sealed class TokenActionPopup : MonoBehaviour
    {
        [SerializeField] RectTransform panel;
        [SerializeField] CanvasGroup canvasGroup;
        [SerializeField] TMP_Text title;
        [SerializeField] RectTransform chipRow;
        [SerializeField] Button chipPrefab;
        [SerializeField] Button closeButton;

        readonly List<Button> _chips = new List<Button>();
        Action<CorrectionOp> _onChoose;
        int _tokenIndex;

        void Awake()
        {
            if (closeButton != null) closeButton.onClick.AddListener(Close);
            SetVisible(false);
        }

        public bool IsOpen => canvasGroup != null && canvasGroup.blocksRaycasts;

        public void OpenRotate(int tokenIndex, Vector3 worldAnchor, Action<CorrectionOp> onChoose)
        {
            _tokenIndex = tokenIndex;
            _onChoose = onChoose;
            if (title != null) title.text = "ROTATE";
            BuildChips(new (string, CorrectionOp)[]
            {
                ("↶  back", CorrectionOp.DoRotate(tokenIndex, -1)),
                ("fwd  ↷", CorrectionOp.DoRotate(tokenIndex, +1)),
            });
            Place(worldAnchor);
            SetVisible(true);
        }

        public void OpenReplace(int tokenIndex, IEnumerable<PrintCommand> options, Vector3 worldAnchor, Action<CorrectionOp> onChoose)
        {
            _tokenIndex = tokenIndex;
            _onChoose = onChoose;
            if (title != null) title.text = "REPLACE WITH";
            var list = new List<(string, CorrectionOp)>();
            foreach (var c in options)
                list.Add((CommandVisuals.Short(c), CorrectionOp.DoReplace(tokenIndex, c)));
            BuildChips(list.ToArray());
            Place(worldAnchor);
            SetVisible(true);
        }

        public void OpenShift(int tokenIndex, Vector3 worldAnchor, Action<CorrectionOp> onChoose)
        {
            _tokenIndex = tokenIndex;
            _onChoose = onChoose;
            if (title != null) title.text = "SHIFT";
            BuildChips(new (string, CorrectionOp)[]
            {
                ("<  left", CorrectionOp.DoShift(tokenIndex, -1)),
                ("right  >", CorrectionOp.DoShift(tokenIndex, +1)),
            });
            Place(worldAnchor);
            SetVisible(true);
        }

        public void OpenRestore(int tokenIndex, IEnumerable<PrintCommand> options, Vector3 worldAnchor, Action<CorrectionOp> onChoose)
        {
            _tokenIndex = tokenIndex;
            _onChoose = onChoose;
            if (title != null) title.text = "RESTORE TO";
            var list = new List<(string, CorrectionOp)>();
            foreach (var c in options)
                list.Add((CommandVisuals.Short(c), CorrectionOp.DoRestore(tokenIndex, c)));
            BuildChips(list.ToArray());
            Place(worldAnchor);
            SetVisible(true);
        }

        void BuildChips((string label, CorrectionOp op)[] items)
        {
            foreach (var c in _chips) Destroy(c.gameObject);
            _chips.Clear();
            foreach (var item in items)
            {
                var chip = Instantiate(chipPrefab, chipRow);
                var txt = chip.GetComponentInChildren<TMP_Text>();
                if (txt != null) txt.text = item.label;
                var op = item.op;
                chip.onClick.AddListener(() =>
                {
                    _onChoose?.Invoke(op);
                    Close();
                });
                chip.gameObject.SetActive(true);
                _chips.Add(chip);
            }
        }

        void Place(Vector3 screenAnchor)
        {
            if (panel == null) return;
            // Overlay canvas: RectTransform.position is screen pixels. Nudge the
            // popup above the token and clamp inside the screen.
            float w = panel.rect.width, h = panel.rect.height;
            float x = Mathf.Clamp(screenAnchor.x, w * 0.5f + 8f, Screen.width - w * 0.5f - 8f);
            float y = Mathf.Clamp(screenAnchor.y + h * 0.5f + 40f, h * 0.5f + 8f, Screen.height - h * 0.5f - 8f);
            panel.position = new Vector3(x, y, 0f);
        }

        public void Close() => SetVisible(false);

        void SetVisible(bool on)
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = on ? 1f : 0f;
                canvasGroup.interactable = on;
                canvasGroup.blocksRaycasts = on;
            }
            gameObject.SetActive(true); // keep component alive; visibility via canvasGroup
            if (panel != null) panel.gameObject.SetActive(on);
        }
    }
}
