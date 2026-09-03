using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MisprintDepartment.Editor.Build
{
    /// <summary>Shared helpers for authoring full-screen panels + common bits.</summary>
    public static class PanelKit
    {
        public static (GameObject go, T comp) Panel<T>(string name, Transform parent, Color bg) where T : Component
        {
            var rt = UiKit.Rect(name, parent);
            UiKit.Stretch(rt);
            var img = rt.gameObject.AddComponent<Image>();
            img.color = bg;
            img.raycastTarget = bg.a > 0.01f;
            rt.gameObject.AddComponent<CanvasGroup>();
            var comp = rt.gameObject.AddComponent<T>();
            return (rt.gameObject, comp);
        }

        public static GameObject Plain(string name, Transform parent, Color bg)
        {
            var rt = UiKit.Rect(name, parent);
            UiKit.Stretch(rt);
            var img = rt.gameObject.AddComponent<Image>();
            img.color = bg;
            img.raycastTarget = bg.a > 0.01f;
            rt.gameObject.AddComponent<CanvasGroup>();
            return rt.gameObject;
        }

        public static Button TextButton(string name, Transform parent, string label,
            Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 pos, Vector2 size,
            Color bg, Color text, float fontSize, out TMP_Text labelText)
        {
            var btn = UiKit.Button(name, parent, label, bg, text, fontSize, out labelText);
            var rt = (RectTransform)btn.transform;
            rt.anchorMin = anchorMin; rt.anchorMax = anchorMax; rt.pivot = pivot;
            rt.anchoredPosition = pos; rt.sizeDelta = size;
            return btn;
        }

        public static TMP_Text Heading(string name, Transform parent, string content,
            Vector2 anchorMin, Vector2 anchorMax, float size, Color color, TextAlignmentOptions align = TextAlignmentOptions.Center)
        {
            var t = UiKit.Text(name, parent, content, size, align, color, FontStyles.Bold);
            var rt = (RectTransform)t.transform;
            rt.anchorMin = anchorMin; rt.anchorMax = anchorMax;
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
            return t;
        }
    }
}
