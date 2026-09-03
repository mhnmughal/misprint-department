using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MisprintDepartment.Game.UI
{
    /// <summary>
    /// Scripted contextual walkthrough for the Training Job. One authored callout
    /// bubble re-points and re-labels as the player progresses. Info steps advance
    /// on a "GOT IT" tap; action steps advance when the player does the thing.
    /// Disappears for good once the sequence completes (persisted by the flow).
    /// </summary>
    public sealed class TutorialLayer : MonoBehaviour
    {
        [Tooltip("CanvasGroup ON THE BUBBLE — blocks raycasts only on info steps so the game stays usable on action steps.")]
        [SerializeField] CanvasGroup group;
        [SerializeField] RectTransform bubble;
        [SerializeField] TMP_Text text;
        [SerializeField] Button continueButton;
        [SerializeField] TMP_Text continueLabel;

        [Header("Anchors (authored in scene)")]
        [SerializeField] RectTransform anchorStrip;
        [SerializeField] RectTransform anchorTray;
        [SerializeField] RectTransform anchorLever;
        [SerializeField] RectTransform anchorDefect;
        [SerializeField] RectTransform anchorBudget;

        // step index -> line, anchor, and whether it advances on a tap (info) or an action
        static readonly (string line, int anchor, bool info)[] Script =
        {
            ("Welcome to the proof bench. The command printer just ran this route strip.", 0, true),
            ("The plate has one fault today. Read it on the DEFECT card — here it swaps Left and Right turns.", 3, true),
            ("So one turn came out wrong. Take the REPLACE tool from the tray.", 1, false),
            ("Tap the wrong command on the strip, then choose the turn it should be.", 0, false),
            ("Every job gives you a limited correction budget — keep an eye on it up here.", 4, true),
            ("Corrected. Now pull the RUN PROOF lever and follow the carriage to the target.", 2, false),
        };

        int _step = -1;
        public bool Running => _step >= 0 && _step < Script.Length;
        public int CurrentStep => _step;

        void Awake()
        {
            if (continueButton != null) continueButton.onClick.AddListener(TryTapAdvance);
            if (group != null) { group.alpha = 0f; group.blocksRaycasts = false; }
        }

        public void Begin()
        {
            _step = 0;
            Render();
        }

        /// <summary>Advance if we're waiting on the given action step.</summary>
        public void CompleteStep(int step)
        {
            if (_step == step && !Script[step].info) { _step++; Render(); }
        }

        public void TryTapAdvance()
        {
            if (Running && Script[_step].info) { _step++; Render(); }
        }

        public void EndImmediately()
        {
            _step = Script.Length;
            if (group != null) { group.alpha = 0f; group.blocksRaycasts = false; }
        }

        void Render()
        {
            if (!Running) { EndImmediately(); return; }
            var s = Script[_step];
            if (text != null) text.text = s.line;
            if (continueLabel != null) continueLabel.text = s.info ? "GOT IT" : "DO THIS";
            if (continueButton != null) continueButton.interactable = s.info;

            var anchor = s.anchor switch
            {
                1 => anchorTray,
                2 => anchorLever,
                3 => anchorDefect,
                4 => anchorBudget,
                _ => anchorStrip,
            };
            if (bubble != null && anchor != null) bubble.position = anchor.position;
            if (group != null)
            {
                group.alpha = 1f;
                group.interactable = true;
                group.blocksRaycasts = s.info; // action steps: let taps reach the game
            }
        }
    }
}
