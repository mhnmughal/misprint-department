using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MisprintDepartment.Core;

namespace MisprintDepartment.Game.UI
{
    /// <summary>One slot on the instruction strip. Prefab authored in the scene.</summary>
    public sealed class TokenView : MonoBehaviour
    {
        [SerializeField] Button button;
        [SerializeField] Image background;
        [SerializeField] TMP_Text glyphText;
        [SerializeField] TMP_Text labelText;
        [SerializeField] GameObject selectionOutline;
        [SerializeField] GameObject fadedOverlay;
        [SerializeField] GameObject proofHighlight;
        [SerializeField] GameObject stripedBadge;   // "//" — machine skips this
        [SerializeField] GameObject markedBadge;    // "x2" — machine runs it twice
        [SerializeField] GameObject stripedFrame;   // distinct border (not colour-only)
        [SerializeField] Color normalColor = new Color(0.96f, 0.93f, 0.86f);
        [SerializeField] Color markedColor = new Color(0.85f, 0.55f, 0.30f);

        public int Index { get; private set; }
        public event Action<int> Clicked;

        void Awake()
        {
            if (button != null) button.onClick.AddListener(() => Clicked?.Invoke(Index));
        }

        public void Bind(int index, CommandToken token)
        {
            Index = index;
            bool faded = token.Has(InkFlags.Faded);
            bool striped = token.Has(InkFlags.Striped);
            bool marked = token.Has(InkFlags.Marked);

            if (glyphText != null) glyphText.text = faded ? "?" : CommandVisuals.Glyph(token.command);
            if (labelText != null) labelText.text = faded ? "FADED" : CommandVisuals.Short(token.command);
            if (fadedOverlay != null) fadedOverlay.SetActive(faded);
            if (stripedBadge != null) stripedBadge.SetActive(striped);
            if (stripedFrame != null) stripedFrame.SetActive(striped);
            if (markedBadge != null) markedBadge.SetActive(marked);
            if (background != null)
                background.color = marked ? markedColor : normalColor;
            SetSelected(false);
            SetProofHighlight(false);
        }

        public void SetSelected(bool on)
        {
            if (selectionOutline != null) selectionOutline.SetActive(on);
        }

        public void SetProofHighlight(bool on)
        {
            if (proofHighlight != null) proofHighlight.SetActive(on);
        }

        public void SetInteractable(bool on)
        {
            if (button != null) button.interactable = on;
        }
    }
}
