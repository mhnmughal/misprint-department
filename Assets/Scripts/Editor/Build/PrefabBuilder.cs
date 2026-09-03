using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MisprintDepartment.Game.UI;

namespace MisprintDepartment.Editor.Build
{
    /// <summary>Builds the reusable UI + board prefabs used by the scenes.</summary>
    public static class PrefabBuilder
    {
        const string Dir = "Assets/Prefabs";

        [MenuItem("Misprint/Build/3 · Prefabs")]
        public static void BuildAll()
        {
            EnsureDir(Dir);
            EnsureDir(Dir + "/UI");
            EnsureDir(Dir + "/Board");
            BuildTokenView();
            BuildToolButton();
            BuildJobTicket();
            BuildChip();
            BuildCellTile();
            BuildBlockedTile();
            BuildCarriage();
            AssetDatabase.SaveAssets();
            Debug.Log("[Misprint] Prefabs built.");
        }

        static GameObject Badge(Transform parent, string text, Color bg, Color fg)
        {
            var img = UiKit.Image("Badge_" + text, parent, bg, false);
            var rt = (RectTransform)img.transform;
            rt.anchorMin = new Vector2(1, 1); rt.anchorMax = new Vector2(1, 1); rt.pivot = new Vector2(1, 1);
            UiKit.Size(rt, 40, 30); UiKit.Pos(rt, -2, -2);
            var t = UiKit.Text("T", img.transform, text, 18, TextAlignmentOptions.Center, fg, FontStyles.Bold);
            UiKit.Stretch((RectTransform)t.transform);
            return img.gameObject;
        }

        static GameObject Root(string name, float w, float h)
        {
            var go = new GameObject(name, typeof(RectTransform));
            var rt = (RectTransform)go.transform;
            rt.sizeDelta = new Vector2(w, h);
            return go;
        }

        static void Save(GameObject go, string path)
        {
            PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
        }

        static void Wire(Component c, string field, Object value)
        {
            var so = new SerializedObject(c);
            var p = so.FindProperty(field);
            if (p == null) { Debug.LogWarning($"{c.GetType().Name}.{field} not found"); return; }
            p.objectReferenceValue = value;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        static void WireColor(Component c, string field, Color value)
        {
            var so = new SerializedObject(c);
            var p = so.FindProperty(field);
            if (p != null) { p.colorValue = value; so.ApplyModifiedPropertiesWithoutUndo(); }
        }

        // ---------------- TokenView ----------------
        static void BuildTokenView()
        {
            var go = Root("TokenView", 108, 138);
            var bg = go.AddComponent<Image>();
            bg.color = UiKit.Paper;
            var btn = go.AddComponent<Button>();
            btn.targetGraphic = bg;

            // subtle ink keyline — reads as a printed cell
            UiKit.EdgeFrame("CellEdge", go.transform, UiKit.PaperDeep, 3f, 0f);

            var glyph = UiKit.Text("Glyph", go.transform, "↑", 54, TextAlignmentOptions.Center, UiKit.Ink, FontStyles.Bold);
            var grt = (RectTransform)glyph.transform;
            grt.anchorMin = new Vector2(0, 0.30f); grt.anchorMax = new Vector2(1, 1); grt.offsetMin = Vector2.zero; grt.offsetMax = Vector2.zero;

            var label = UiKit.Text("Label", go.transform, "MOVE", 17, TextAlignmentOptions.Center, UiKit.InkSoft, FontStyles.Bold);
            var lrt = (RectTransform)label.transform;
            lrt.anchorMin = new Vector2(0, 0.02f); lrt.anchorMax = new Vector2(1, 0.28f); lrt.offsetMin = Vector2.zero; lrt.offsetMax = Vector2.zero;
            label.characterSpacing = 2f;

            var faded = UiKit.Image("FadedOverlay", go.transform, new Color(0.60f, 0.57f, 0.50f, 0.80f));
            UiKit.Stretch((RectTransform)faded.transform, 3f);
            faded.gameObject.SetActive(false);

            // striped (Color Pass) — a distinct metal border + a "//" corner badge (not colour-only)
            var stripedFrame = UiKit.EdgeFrame("StripedFrame", go.transform, UiKit.Metal, 6f, -2f);
            stripedFrame.SetActive(false);
            var stripedBadge = Badge(go.transform, "//", UiKit.Metal, UiKit.Paper);
            stripedBadge.SetActive(false);

            // marked (Color Pass) — "x2" corner badge
            var markedBadge = Badge(go.transform, "x2", new Color(0.85f, 0.55f, 0.30f), UiKit.Ink);
            markedBadge.SetActive(false);

            // proof highlight = warm keyline frame, does not tint the glyph
            var hi = UiKit.EdgeFrame("ProofHighlight", go.transform, UiKit.LampWarm, 6f, -4f);
            hi.SetActive(false);

            // selection = teal keyline frame (label/glyph stay readable)
            var sel = UiKit.EdgeFrame("SelectionOutline", go.transform, UiKit.Teal, 5f, -2f);
            sel.SetActive(false);

            var tv = go.AddComponent<TokenView>();
            Wire(tv, "button", btn);
            Wire(tv, "background", bg);
            Wire(tv, "glyphText", glyph);
            Wire(tv, "labelText", label);
            Wire(tv, "selectionOutline", sel);
            Wire(tv, "fadedOverlay", faded.gameObject);
            Wire(tv, "proofHighlight", hi);
            Wire(tv, "stripedBadge", stripedBadge);
            Wire(tv, "markedBadge", markedBadge);
            Wire(tv, "stripedFrame", stripedFrame);
            WireColor(tv, "normalColor", UiKit.Paper);
            WireColor(tv, "markedColor", new Color(0.85f, 0.55f, 0.30f));

            Save(go, $"{Dir}/UI/TokenView.prefab");
        }

        // ---------------- ToolButton ----------------
        static void BuildToolButton()
        {
            var go = Root("ToolButton", 172, 128);
            var bg = go.AddComponent<Image>();
            bg.color = UiKit.CharcoalUp;
            var btn = go.AddComponent<Button>();
            btn.targetGraphic = bg;
            var cg = go.AddComponent<CanvasGroup>();

            // bottom lip for tactility
            var lip = UiKit.Image("Lip", go.transform, new Color(0, 0, 0, 0.3f), false);
            var lrt = (RectTransform)lip.transform;
            lrt.anchorMin = new Vector2(0, 0); lrt.anchorMax = new Vector2(1, 0);
            lrt.sizeDelta = new Vector2(0, 5); lrt.anchoredPosition = Vector2.zero;

            // selected = teal keyline frame (never covers the label)
            var sel = UiKit.EdgeFrame("SelectedState", go.transform, UiKit.Teal, 5f, 0f);
            var accent = UiKit.Image("Accent", sel.transform, UiKit.Teal, false);
            var art = (RectTransform)accent.transform;
            art.anchorMin = new Vector2(0, 1); art.anchorMax = new Vector2(1, 1);
            art.sizeDelta = new Vector2(0, 10); art.anchoredPosition = new Vector2(0, 0);
            sel.SetActive(false);

            // label always on top
            var label = UiKit.Text("Label", go.transform, "ROTATE", 22, TextAlignmentOptions.Center, UiKit.Paper, FontStyles.Bold);
            UiKit.Stretch((RectTransform)label.transform, 10f);
            label.characterSpacing = 2f;

            var tb = go.AddComponent<ToolButton>();
            Wire(tb, "button", btn);
            Wire(tb, "label", label);
            Wire(tb, "selectedState", sel);
            Wire(tb, "canvasGroup", cg);
            Save(go, $"{Dir}/UI/ToolButton.prefab");
        }

        // ---------------- JobTicket ----------------
        static void BuildJobTicket()
        {
            var go = Root("JobTicket", 280, 200);
            var bg = go.AddComponent<Image>();
            bg.color = UiKit.Paper;
            var btn = go.AddComponent<Button>();
            btn.targetGraphic = bg;

            UiKit.EdgeFrame("Edge", go.transform, UiKit.Ink, 3f, 0f);
            // job-ticket header band
            var band = UiKit.Image("HeaderBand", go.transform, UiKit.Ink, false);
            var bdr = (RectTransform)band.transform;
            bdr.anchorMin = new Vector2(0, 1); bdr.anchorMax = new Vector2(1, 1); bdr.pivot = new Vector2(0.5f, 1);
            UiKit.Size(bdr, 0, 8); UiKit.Pos(bdr, 0, -6);
            UiKit.RegistrationMark(go.transform, new Vector2(0.92f, 0.12f), UiKit.PaperDeep, 18f);

            var punch = UiKit.Image("Punch", go.transform, UiKit.PaperDeep);
            var prt = (RectTransform)punch.transform;
            prt.anchorMin = new Vector2(0, 1); prt.anchorMax = new Vector2(0, 1); prt.pivot = new Vector2(0, 1);
            UiKit.Size(prt, 34, 34); UiKit.Pos(prt, 18, -22);

            var num = UiKit.Text("JobNumber", go.transform, "001", 44, TextAlignmentOptions.TopLeft, UiKit.Ink, FontStyles.Bold);
            var nrt = (RectTransform)num.transform;
            nrt.anchorMin = new Vector2(0, 1); nrt.anchorMax = new Vector2(1, 1); nrt.pivot = new Vector2(0.5f, 1);
            UiKit.Size(nrt, 0, 60); UiKit.Pos(nrt, 0, -14); nrt.offsetMin = new Vector2(70, nrt.offsetMin.y); nrt.offsetMax = new Vector2(-16, nrt.offsetMax.y);

            var title = UiKit.Text("JobTitle", go.transform, "FIRST PROOF", 20, TextAlignmentOptions.Center, UiKit.Ink, FontStyles.Bold);
            var trt = (RectTransform)title.transform;
            trt.anchorMin = new Vector2(0, 0.36f); trt.anchorMax = new Vector2(1, 0.58f); trt.offsetMin = new Vector2(12, 0); trt.offsetMax = new Vector2(-12, 0);

            var batch = UiKit.Text("BatchLabel", go.transform, "BATCH 1 · REGISTRATION ERROR", 13, TextAlignmentOptions.Center, UiKit.InkSoft, FontStyles.Bold);
            var brt = (RectTransform)batch.transform;
            brt.anchorMin = new Vector2(0, 0.60f); brt.anchorMax = new Vector2(1, 0.72f); brt.offsetMin = new Vector2(10, 0); brt.offsetMax = new Vector2(-10, 0);
            batch.characterSpacing = 1f;

            var rating = UiKit.Text("RatingText", go.transform, "MASTER PROOF", 18, TextAlignmentOptions.Center, UiKit.WarnRed, FontStyles.Bold);
            var rrt = (RectTransform)rating.transform;
            rrt.anchorMin = new Vector2(0, 0.05f); rrt.anchorMax = new Vector2(1, 0.25f); rrt.offsetMin = new Vector2(12, 0); rrt.offsetMax = new Vector2(-12, 0);

            var stamp = UiKit.Rect("CompleteStamp", go.transform).gameObject;
            var srt = (RectTransform)stamp.transform;
            srt.anchorMin = new Vector2(1, 1); srt.anchorMax = new Vector2(1, 1); srt.pivot = new Vector2(1, 1);
            UiKit.Size(srt, 108, 76); UiKit.Pos(srt, -8, -12);
            srt.localEulerAngles = new Vector3(0, 0, -9);
            UiKit.EdgeFrame("Ring", stamp.transform, UiKit.WarnRed, 4f, 0f);
            UiKit.EdgeFrame("Ring2", stamp.transform, UiKit.WarnRed, 2f, 8f);
            var stampTxt = UiKit.Text("Mark", stamp.transform, "PASS", 26, TextAlignmentOptions.Center, UiKit.WarnRed, FontStyles.Bold);
            UiKit.Stretch((RectTransform)stampTxt.transform, 6f);
            stampTxt.characterSpacing = 4f;
            stamp.SetActive(false);

            var locked = UiKit.Image("LockedOverlay", go.transform, new Color(0.17f, 0.17f, 0.18f, 0.82f));
            UiKit.Stretch((RectTransform)locked.transform);
            var lockTxt = UiKit.Text("LockLabel", locked.transform, "LOCKED", 26, TextAlignmentOptions.Center, UiKit.Metal, FontStyles.Bold);
            UiKit.Stretch((RectTransform)lockTxt.transform);
            locked.gameObject.SetActive(false);

            var jt = go.AddComponent<JobTicket>();
            Wire(jt, "button", btn);
            Wire(jt, "jobNumber", num);
            Wire(jt, "jobTitle", title);
            Wire(jt, "batchLabel", batch);
            Wire(jt, "ratingText", rating);
            Wire(jt, "lockedOverlay", locked.gameObject);
            Wire(jt, "completeStamp", stamp.gameObject);
            Save(go, $"{Dir}/UI/JobTicket.prefab");
        }

        // ---------------- Chip ----------------
        static void BuildChip()
        {
            var go = Root("Chip", 132, 74);
            var bg = go.AddComponent<Image>();
            bg.color = UiKit.Paper;
            var btn = go.AddComponent<Button>();
            btn.targetGraphic = bg;
            UiKit.EdgeFrame("Edge", go.transform, UiKit.Ink, 2f, 0f);
            var lip = UiKit.Image("Lip", go.transform, new Color(0,0,0,0.22f), false);
            var lrt=(RectTransform)lip.transform; lrt.anchorMin=new Vector2(0,0); lrt.anchorMax=new Vector2(1,0);
            lrt.sizeDelta=new Vector2(0,4); lrt.anchoredPosition=Vector2.zero;
            var label = UiKit.Text("Label", go.transform, "MOVE", 21, TextAlignmentOptions.Center, UiKit.Ink, FontStyles.Bold);
            UiKit.Stretch((RectTransform)label.transform, 6f);
            label.characterSpacing = 2f;
            Save(go, $"{Dir}/UI/Chip.prefab");
        }

        // ---------------- Board tiles ----------------
        static void BuildCellTile()
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Quad);
            go.name = "CellTile";
            go.transform.localEulerAngles = new Vector3(90, 0, 0); // face up (+Y)
            go.transform.localScale = new Vector3(0.92f, 0.92f, 1f);
            var mat = ArtBuilder.Load(ArtBuilder.M.TileLight);
            if (mat != null) go.GetComponent<Renderer>().sharedMaterial = mat;
            Object.DestroyImmediate(go.GetComponent<Collider>());
            Save(go, $"{Dir}/Board/CellTile.prefab");
        }

        static void BuildBlockedTile()
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = "BlockedTile";
            go.transform.localScale = new Vector3(0.9f, 0.35f, 0.9f);
            go.transform.localPosition = new Vector3(0, 0.17f, 0);
            var mat = ArtBuilder.Load(ArtBuilder.M.TileDark);
            if (mat != null) go.GetComponent<Renderer>().sharedMaterial = mat;
            Object.DestroyImmediate(go.GetComponent<Collider>());
            Save(go, $"{Dir}/Board/BlockedTile.prefab");
        }

        static void BuildCarriage()
        {
            var go = new GameObject("Carriage");
            var body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            body.name = "Body";
            body.transform.SetParent(go.transform, false);
            body.transform.localScale = new Vector3(0.5f, 0.22f, 0.5f);
            body.transform.localPosition = new Vector3(0, 0.11f, 0);
            Object.DestroyImmediate(body.GetComponent<Collider>());
            var bmat = ArtBuilder.Load(ArtBuilder.M.Carriage);
            if (bmat != null) body.GetComponent<Renderer>().sharedMaterial = bmat;

            var nose = GameObject.CreatePrimitive(PrimitiveType.Cube);
            nose.name = "Nose";
            nose.transform.SetParent(go.transform, false);
            nose.transform.localScale = new Vector3(0.16f, 0.16f, 0.28f);
            nose.transform.localPosition = new Vector3(0, 0.14f, 0.28f); // +Z = North (facing)
            Object.DestroyImmediate(nose.GetComponent<Collider>());
            var nmat = ArtBuilder.Load(ArtBuilder.M.Metal);
            if (nmat != null) nose.GetComponent<Renderer>().sharedMaterial = nmat;

            Save(go, $"{Dir}/Board/Carriage.prefab");
        }

        static void EnsureDir(string dir)
        {
            if (AssetDatabase.IsValidFolder(dir)) return;
            var parts = dir.Split('/');
            var cur = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                var next = cur + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next)) AssetDatabase.CreateFolder(cur, parts[i]);
                cur = next;
            }
        }
    }
}
