using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace MisprintDepartment.Editor.Build
{
    /// <summary>
    /// Helpers for authoring real UI hierarchy from an editor script. Everything
    /// produced here is a serialized scene/prefab object visible in Edit Mode —
    /// no runtime construction.
    /// </summary>
    public static class UiKit
    {
        // --- palette (retro industrial print QC) ---
        public static readonly Color Paper       = new Color32(0xF2, 0xEA, 0xD8, 0xFF);
        public static readonly Color PaperDeep   = new Color32(0xE4, 0xD8, 0xBE, 0xFF);
        public static readonly Color Ink         = new Color32(0x23, 0x20, 0x1C, 0xFF);
        public static readonly Color Charcoal    = new Color32(0x2B, 0x2B, 0x2E, 0xFF);
        public static readonly Color CharcoalUp  = new Color32(0x3A, 0x3A, 0x3E, 0xFF);
        public static readonly Color Metal       = new Color32(0x8A, 0x8F, 0x93, 0xFF);
        public static readonly Color WarnRed     = new Color32(0xB5, 0x3A, 0x2E, 0xFF);
        public static readonly Color Teal        = new Color32(0x3F, 0x8E, 0x92, 0xFF);
        public static readonly Color LampWarm    = new Color32(0xF3, 0xC9, 0x7A, 0xFF);
        public static readonly Color PanelBg     = new Color32(0x2A, 0x26, 0x21, 0xFF);

        public static readonly Color InkSoft     = new Color32(0x5A, 0x53, 0x49, 0xFF);
        public static readonly Color Shadow      = new Color32(0x1A, 0x18, 0x16, 0x80);

        public static TMP_FontAsset Font =>
            TMP_Settings.defaultFontAsset;

        /// <summary>Four stretched edge images forming a keyline frame. Returns the container.</summary>
        public static GameObject EdgeFrame(string name, Transform parent, Color color, float thickness, float inset = 0f)
        {
            var root = Rect(name, parent);
            Stretch(root, inset);
            void Edge(string n, Vector2 aMin, Vector2 aMax, Vector2 size)
            {
                var e = Image(n, root, color, false);
                var rt = (RectTransform)e.transform;
                rt.anchorMin = aMin; rt.anchorMax = aMax;
                rt.sizeDelta = size; rt.anchoredPosition = Vector2.zero;
            }
            Edge("Top", new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, thickness));
            Edge("Bottom", new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, thickness));
            Edge("Left", new Vector2(0, 0), new Vector2(0, 1), new Vector2(thickness, 0));
            Edge("Right", new Vector2(1, 0), new Vector2(1, 1), new Vector2(thickness, 0));
            return root.gameObject;
        }

        /// <summary>Thin keyline just inside a panel's edge — printed-form look.</summary>
        public static void Keyline(Transform panel, Color color, float thickness = 2f, float inset = 10f)
        {
            EdgeFrame("Keyline", panel, color, thickness, inset);
        }

        /// <summary>Small registration cross-hair mark — repeated on cards for identity.</summary>
        public static void RegistrationMark(Transform parent, Vector2 anchor, Color color, float s = 22f)
        {
            var root = Rect("RegMark", parent);
            root.anchorMin = anchor; root.anchorMax = anchor; root.pivot = new Vector2(0.5f, 0.5f);
            root.sizeDelta = new Vector2(s, s);
            var h = Image("H", root, color, false); ((RectTransform)h.transform).sizeDelta = new Vector2(s, 2);
            var v = Image("V", root, color, false); ((RectTransform)v.transform).sizeDelta = new Vector2(2, s);
        }

        public static RectTransform Rect(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.layer = LayerMask.NameToLayer("UI");
            var rt = (RectTransform)go.transform;
            rt.SetParent(parent, false);
            rt.localScale = Vector3.one;
            rt.anchoredPosition = Vector2.zero;
            rt.sizeDelta = new Vector2(100, 100);
            return rt;
        }

        public static void Stretch(RectTransform rt, float pad = 0f)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = new Vector2(pad, pad);
            rt.offsetMax = new Vector2(-pad, -pad);
        }

        public static void Anchor(RectTransform rt, Vector2 min, Vector2 max, Vector2 pivot)
        {
            rt.anchorMin = min; rt.anchorMax = max; rt.pivot = pivot;
        }

        public static RectTransform Size(RectTransform rt, float w, float h)
        {
            rt.sizeDelta = new Vector2(w, h);
            return rt;
        }

        public static RectTransform Pos(RectTransform rt, float x, float y)
        {
            rt.anchoredPosition = new Vector2(x, y);
            return rt;
        }

        public static Image Image(string name, Transform parent, Color color, bool raycast = true)
        {
            var rt = Rect(name, parent);
            var img = rt.gameObject.AddComponent<UnityEngine.UI.Image>();
            img.color = color;
            img.raycastTarget = raycast;
            return img;
        }

        public static TMP_Text Text(string name, Transform parent, string content, float size,
            TextAlignmentOptions align = TextAlignmentOptions.Center, Color? color = null, FontStyles style = FontStyles.Normal)
        {
            var rt = Rect(name, parent);
            var t = rt.gameObject.AddComponent<TextMeshProUGUI>();
            t.text = content;
            t.fontSize = size;
            t.alignment = align;
            t.color = color ?? Ink;
            t.fontStyle = style;
            t.enableWordWrapping = true;
            t.raycastTarget = false;
            t.overflowMode = TextOverflowModes.Overflow;
            t.lineSpacing = 6f;
            t.paragraphSpacing = 8f;
            if (Font != null) t.font = Font;
            return t;
        }

        public static Button Button(string name, Transform parent, string label, Color bg, Color textColor,
            float fontSize, out TMP_Text labelText)
        {
            var rt = Rect(name, parent);
            var img = rt.gameObject.AddComponent<UnityEngine.UI.Image>();
            img.color = bg;
            var btn = rt.gameObject.AddComponent<UnityEngine.UI.Button>();
            btn.targetGraphic = img;
            var colors = btn.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1.08f, 1.08f, 1.08f, 1f);
            colors.pressedColor = new Color(0.82f, 0.82f, 0.82f, 1f);
            colors.selectedColor = Color.white;
            colors.fadeDuration = 0.08f;
            btn.colors = colors;

            // tactile bottom lip
            var lip = Image(name + "Lip", rt, new Color(0f, 0f, 0f, 0.28f), false);
            var lrt = (RectTransform)lip.transform;
            lrt.anchorMin = new Vector2(0, 0); lrt.anchorMax = new Vector2(1, 0);
            lrt.sizeDelta = new Vector2(0, 5); lrt.anchoredPosition = Vector2.zero;

            labelText = Text(name + "Label", rt, label, fontSize, TextAlignmentOptions.Midline, textColor, FontStyles.Bold);
            Stretch((RectTransform)labelText.transform, 8f);
            labelText.characterSpacing = 3f;
            return btn;
        }

        public static Button Button(string name, Transform parent, string label, Color bg, Color textColor, float fontSize)
            => Button(name, parent, label, bg, textColor, fontSize, out _);

        public static CanvasGroup CanvasGroup(GameObject go)
        {
            var cg = go.GetComponent<CanvasGroup>();
            if (cg == null) cg = go.AddComponent<CanvasGroup>();
            return cg;
        }

        public static Slider Slider(string name, Transform parent, Color track, Color fill, Color handle)
        {
            var rt = Rect(name, parent);
            Size(rt, 420, 40);
            var slider = rt.gameObject.AddComponent<UnityEngine.UI.Slider>();

            var bg = Image("Background", rt, track);
            Stretch((RectTransform)bg.transform);

            var fillArea = Rect("Fill Area", rt);
            Stretch(fillArea); fillArea.offsetMin = new Vector2(8, 0); fillArea.offsetMax = new Vector2(-8, 0);
            var fillImg = Image("Fill", fillArea, fill);
            var fillRt = (RectTransform)fillImg.transform;
            fillRt.anchorMin = new Vector2(0, 0); fillRt.anchorMax = new Vector2(1, 1);
            fillRt.sizeDelta = Vector2.zero;

            var handleArea = Rect("Handle Slide Area", rt);
            Stretch(handleArea); handleArea.offsetMin = new Vector2(10, 0); handleArea.offsetMax = new Vector2(-10, 0);
            var handleImg = Image("Handle", handleArea, handle);
            Size((RectTransform)handleImg.transform, 22, 40);

            slider.fillRect = fillRt;
            slider.handleRect = (RectTransform)handleImg.transform;
            slider.targetGraphic = handleImg;
            slider.direction = UnityEngine.UI.Slider.Direction.LeftToRight;
            slider.minValue = 0f; slider.maxValue = 1f; slider.value = 0.8f;
            return slider;
        }

        public static Toggle Toggle(string name, Transform parent, string label, Color box, Color check, Color textColor)
        {
            var rt = Rect(name, parent);
            Size(rt, 420, 44);
            var toggle = rt.gameObject.AddComponent<UnityEngine.UI.Toggle>();

            var boxImg = Image("Box", rt, box);
            var boxRt = (RectTransform)boxImg.transform;
            boxRt.anchorMin = new Vector2(0, 0.5f); boxRt.anchorMax = new Vector2(0, 0.5f);
            boxRt.pivot = new Vector2(0, 0.5f);
            Size(boxRt, 36, 36); Pos(boxRt, 0, 0);

            var checkImg = Image("Check", boxImg.transform, check);
            Stretch((RectTransform)checkImg.transform, 6f);

            var lab = Text("Label", rt, label, 26, TextAlignmentOptions.MidlineLeft, textColor);
            var labRt = (RectTransform)lab.transform;
            labRt.anchorMin = new Vector2(0, 0); labRt.anchorMax = new Vector2(1, 1);
            labRt.offsetMin = new Vector2(48, 0); labRt.offsetMax = Vector2.zero;

            toggle.targetGraphic = boxImg;
            toggle.graphic = checkImg;
            toggle.isOn = true;
            return toggle;
        }

        public static HorizontalLayoutGroup HLayout(GameObject go, float spacing, RectOffset padding, TextAnchor align = TextAnchor.MiddleCenter)
        {
            var h = go.AddComponent<HorizontalLayoutGroup>();
            h.spacing = spacing;
            h.padding = padding;
            h.childAlignment = align;
            h.childControlWidth = false; h.childControlHeight = false;
            h.childForceExpandWidth = false; h.childForceExpandHeight = false;
            return h;
        }

        public static VerticalLayoutGroup VLayout(GameObject go, float spacing, RectOffset padding, TextAnchor align = TextAnchor.UpperCenter)
        {
            var v = go.AddComponent<VerticalLayoutGroup>();
            v.spacing = spacing;
            v.padding = padding;
            v.childAlignment = align;
            v.childControlWidth = false; v.childControlHeight = false;
            v.childForceExpandWidth = false; v.childForceExpandHeight = false;
            return v;
        }

        public static GridLayoutGroup Grid(GameObject go, Vector2 cell, Vector2 spacing, int cols)
        {
            var g = go.AddComponent<GridLayoutGroup>();
            g.cellSize = cell; g.spacing = spacing;
            g.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            g.constraintCount = cols;
            g.childAlignment = TextAnchor.MiddleCenter;
            return g;
        }

        /// <summary>Create (or return) the scene Canvas + SafeArea + EventSystem.</summary>
        public static (Canvas canvas, RectTransform safeArea) BuildCanvas(string canvasName)
        {
            var canvasGo = new GameObject(canvasName, typeof(RectTransform));
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 0;
            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
            canvasGo.AddComponent<GraphicRaycaster>();

            var safe = Rect("SafeArea", canvasGo.transform);
            Stretch(safe);
            safe.gameObject.AddComponent<MisprintDepartment.Game.UI.SafeAreaFitter>();

            EnsureEventSystem();
            return (canvas, safe);
        }

        public static void EnsureEventSystem()
        {
            if (Object.FindFirstObjectByType<EventSystem>() != null) return;
            var es = new GameObject("EventSystem", typeof(EventSystem));
            es.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
        }
    }
}
