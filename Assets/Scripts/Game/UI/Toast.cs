using System.Collections;
using UnityEngine;
using TMPro;

namespace MisprintDepartment.Game.UI
{
    /// <summary>Brief non-blocking message (e.g. "No corrections left"). Authored in scene.</summary>
    public sealed class Toast : MonoBehaviour
    {
        [SerializeField] CanvasGroup group;
        [SerializeField] TMP_Text text;
        [SerializeField] float holdSeconds = 1.4f;

        Coroutine _running;
        float _hold;

        void Awake() { if (group != null) group.alpha = 0f; }

        public void Show(string message) => Show(message, holdSeconds);

        public void Show(string message, float hold)
        {
            if (text != null) text.text = message;
            _hold = hold;
            if (_running != null) StopCoroutine(_running);
            _running = StartCoroutine(Run());
        }

        IEnumerator Run()
        {
            yield return Fade(1f, 0.12f);
            yield return new WaitForSeconds(_hold);
            yield return Fade(0f, 0.3f);
        }

        IEnumerator Fade(float target, float secs)
        {
            if (group == null) yield break;
            float from = group.alpha, t = 0f;
            while (t < 1f) { t += Time.unscaledDeltaTime / secs; group.alpha = Mathf.Lerp(from, target, t); yield return null; }
            group.alpha = target;
        }
    }
}
