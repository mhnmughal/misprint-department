using System.Collections;
using UnityEngine;

namespace MisprintDepartment.Game.UI
{
    /// <summary>
    /// A scene-authored panel that can be shown/hidden with a short fade. The
    /// GameObject and all its children are authored in the scene; this only
    /// toggles visibility and interactivity — it never creates UI.
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class UIPanel : MonoBehaviour
    {
        [SerializeField] float fadeSeconds = 0.15f;
        [SerializeField] bool visibleOnStart;

        CanvasGroup _cg;
        Coroutine _anim;

        public bool IsVisible { get; private set; }

        protected virtual void Awake()
        {
            _cg = GetComponent<CanvasGroup>();
            SetInstant(visibleOnStart);
        }

        public void Show()
        {
            if (IsVisible) return;
            IsVisible = true;
            gameObject.SetActive(true);
            OnShow();
            Animate(1f);
        }

        public void Hide()
        {
            if (!IsVisible) { SetInstant(false); return; }
            IsVisible = false;
            OnHide();
            Animate(0f);
        }

        public void SetInstant(bool visible)
        {
            IsVisible = visible;
            if (_cg == null) _cg = GetComponent<CanvasGroup>();
            _cg.alpha = visible ? 1f : 0f;
            _cg.interactable = visible;
            _cg.blocksRaycasts = visible;
            gameObject.SetActive(visible);
            if (visible) OnShow();
        }

        protected virtual void OnShow() { }
        protected virtual void OnHide() { }

        void Animate(float target)
        {
            if (_anim != null) StopCoroutine(_anim);
            _anim = StartCoroutine(FadeTo(target));
        }

        IEnumerator FadeTo(float target)
        {
            _cg.interactable = target > 0.5f;
            _cg.blocksRaycasts = target > 0.5f;
            float from = _cg.alpha;
            float t = 0f;
            while (t < 1f)
            {
                t += Time.unscaledDeltaTime / Mathf.Max(0.01f, fadeSeconds);
                _cg.alpha = Mathf.Lerp(from, target, t);
                yield return null;
            }
            _cg.alpha = target;
            if (target <= 0f) gameObject.SetActive(false);
        }
    }
}
