using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace MisprintDepartment.Editor.Build
{
    /// <summary>
    /// Draws the original 1024x1024 app icon: a misprinted command strip feeding
    /// through a printer roller with one instruction being corrected. Pure pixel
    /// drawing — no font, no external art. Menu: Misprint/Build/6 · App Icon.
    /// </summary>
    public sealed class IconBuilder
    {
        IconBuilder() { }

        const int S = 1024;
        const string Dir = "Assets/Art/Icon";
        const string PngPath = Dir + "/AppIcon.png";

        static readonly Color Charcoal = new Color32(0x24, 0x24, 0x27, 0xFF);
        static readonly Color CharcoalHi = new Color32(0x33, 0x33, 0x38, 0xFF);
        static readonly Color Paper = new Color32(0xF2, 0xEA, 0xD8, 0xFF);
        static readonly Color PaperShade = new Color32(0xDD, 0xD1, 0xB6, 0xFF);
        static readonly Color Ink = new Color32(0x23, 0x20, 0x1C, 0xFF);
        static readonly Color Red = new Color32(0xC0, 0x3A, 0x2C, 0xFF);
        static readonly Color Teal = new Color32(0x3F, 0x8E, 0x92, 0xFF);
        static readonly Color Metal = new Color32(0x6E, 0x72, 0x76, 0xFF);

        Color[] _buf;

        [MenuItem("Misprint/Build/6 · App Icon")]
        public static void BuildMenu()
        {
            var tex = Render();
            EnsureDir(Dir);
            File.WriteAllBytes(PngPath, tex.EncodeToPNG());
            Object.DestroyImmediate(tex);
            AssetDatabase.ImportAsset(PngPath, ImportAssetOptions.ForceUpdate);

            var importer = (TextureImporter)AssetImporter.GetAtPath(PngPath);
            importer.textureType = TextureImporterType.Default;
            importer.sRGBTexture = true;
            importer.alphaIsTransparency = false;
            importer.mipmapEnabled = false;
            importer.npotScale = TextureImporterNPOTScale.None;
            importer.maxTextureSize = 1024;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();

            var sprite = AssetDatabase.LoadAssetAtPath<Texture2D>(PngPath);
            ApplyToPlayerSettings(sprite);
            Debug.Log("[Misprint] App icon built and assigned: " + PngPath);
        }

        public static void ApplyToPlayerSettings(Texture2D icon)
        {
            if (icon == null) return;
            var tex = new[] { icon };

            // Default / unknown platform
            PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Unknown, tex);

            SetPlatformIcons(NamedBuildTarget.iOS, icon);
            SetPlatformIcons(NamedBuildTarget.Android, icon);
        }

        static void SetPlatformIcons(NamedBuildTarget target, Texture2D icon)
        {
            var kinds = PlayerSettings.GetSupportedIconKinds(target);
            foreach (var kind in kinds)
            {
                var slots = PlayerSettings.GetPlatformIcons(target, kind);
                for (int i = 0; i < slots.Length; i++)
                    slots[i].SetTexture(icon, 0);
                PlayerSettings.SetPlatformIcons(target, kind, slots);
            }
        }

        // ---------------- drawing ----------------

        public static Texture2D Render()
        {
            var d = new IconBuilder { _buf = new Color[S * S] };
            d.Fill(Charcoal);
            d.Vignette();

            // strip band through the middle
            int bandH = 540;
            int bandY = (S - bandH) / 2;
            int bandX = 40, bandW = S - 80;
            d.RoundRect(bandX, bandY, bandW, bandH, 30, Paper);
            d.RoundRectOutline(bandX, bandY, bandW, bandH, 30, 7, PaperShade);

            // feed perforations along top & bottom of the strip
            for (int x = bandX + 70; x < bandX + bandW - 60; x += 92)
            {
                d.Disc(x, bandY + 42, 11, Charcoal);
                d.Disc(x, bandY + bandH - 42, 11, Charcoal);
            }

            // three command cells
            int cw = 258, ch = 300;
            int cy = S / 2 - ch / 2;
            int gap = (bandW - 120 - 3 * cw) / 2;
            int x0 = bandX + 60;
            int[] cx = { x0, x0 + cw + gap, x0 + 2 * (cw + gap) };

            d.RoundRect(cx[0], cy, cw, ch, 20, PaperShade);
            d.UpArrow(cx[0] + cw / 2, cy + ch / 2, 116, Ink);

            d.RoundRect(cx[2], cy, cw, ch, 20, PaperShade);
            d.UpArrow(cx[2] + cw / 2, cy + ch / 2, 116, Ink);

            // middle cell = the misprint being corrected
            d.RoundRect(cx[1], cy, cw, ch, 20, Red);
            d.TurnArrow(cx[1] + cw / 2, cy + ch / 2, 104, new Color(0.95f, 0.86f, 0.80f, 0.30f));
            // correction stroke — a bold cream swipe of correction tape, kept inside the cell
            d.ThickLine(cx[1] + 34, cy + 40, cx[1] + cw - 34, cy + ch - 40, 40, Paper);
            // the corrected instruction stamped on top
            d.UpArrow(cx[1] + cw / 2, cy + ch / 2, 112, Ink);
            // teal inspection tick, top-right of the cell
            d.Check(cx[1] + cw - 34, cy + ch - 34, 52, Teal);

            // printer rollers across the strip (top + bottom), on top of everything
            d.Roller(bandY - 20);
            d.Roller(bandY + bandH - 90);

            var tex = new Texture2D(S, S, TextureFormat.RGBA32, false, false);
            tex.SetPixels(d._buf);
            tex.Apply();
            return tex;
        }

        void Roller(int y)
        {
            RoundRect(24, y, S - 48, 90, 40, CharcoalHi);
            RoundRectOutline(24, y, S - 48, 90, 40, 4, new Color32(0x1A, 0x1A, 0x1D, 0xFF));
            // metal highlight
            RoundRect(60, y + 20, S - 120, 12, 6, Metal);
            Disc(70, y + 45, 26, Metal);
            Disc(S - 70, y + 45, 26, Metal);
        }

        // ---- primitives ----

        void Fill(Color c) { for (int i = 0; i < _buf.Length; i++) _buf[i] = c; }

        void Vignette()
        {
            var cx = S / 2f; var cy = S / 2f; var max = Mathf.Sqrt(cx * cx + cy * cy);
            for (int y = 0; y < S; y++)
            for (int x = 0; x < S; x++)
            {
                float dd = Mathf.Sqrt((x - cx) * (x - cx) + (y - cy) * (y - cy)) / max;
                float k = Mathf.Lerp(0.06f, -0.10f, dd);
                var c = _buf[y * S + x];
                _buf[y * S + x] = new Color(
                    Mathf.Clamp01(c.r + k), Mathf.Clamp01(c.g + k), Mathf.Clamp01(c.b + k), 1f);
            }
        }

        void Px(int x, int y, Color c)
        {
            if (x < 0 || y < 0 || x >= S || y >= S) return;
            var d = _buf[y * S + x];
            _buf[y * S + x] = c.a >= 1f ? c : Color.Lerp(d, c, c.a);
        }

        void RectFill(int x0, int y0, int w, int h, Color c)
        {
            for (int y = y0; y < y0 + h; y++)
            for (int x = x0; x < x0 + w; x++)
                Px(x, y, c);
        }

        void RoundRect(int x0, int y0, int w, int h, int r, Color c)
        {
            for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
            {
                int dx = Mathf.Min(x, w - 1 - x);
                int dy = Mathf.Min(y, h - 1 - y);
                if (dx < r && dy < r)
                {
                    int ex = r - dx, ey = r - dy;
                    if (ex * ex + ey * ey > r * r) continue;
                }
                Px(x0 + x, y0 + y, c);
            }
        }

        void RoundRectOutline(int x0, int y0, int w, int h, int r, int t, Color c)
        {
            for (int i = 0; i < t; i++)
            {
                // draw progressively smaller rounded rects as a ring
                for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    bool edge = x == i || y == i || x == w - 1 - i || y == h - 1 - i;
                    if (!edge) continue;
                    int dx = Mathf.Min(x, w - 1 - x), dy = Mathf.Min(y, h - 1 - y);
                    if (dx < r && dy < r)
                    {
                        int ex = r - dx, ey = r - dy;
                        if (ex * ex + ey * ey > r * r) continue;
                    }
                    Px(x0 + x, y0 + y, c);
                }
            }
        }

        void Disc(int cx, int cy, int rad, Color c)
        {
            for (int y = -rad; y <= rad; y++)
            for (int x = -rad; x <= rad; x++)
                if (x * x + y * y <= rad * rad) Px(cx + x, cy + y, c);
        }

        void ThickLine(int x0, int y0, int x1, int y1, int thick, Color c)
        {
            int steps = Mathf.Max(Mathf.Abs(x1 - x0), Mathf.Abs(y1 - y0));
            for (int i = 0; i <= steps; i++)
            {
                float t = i / (float)steps;
                int x = Mathf.RoundToInt(Mathf.Lerp(x0, x1, t));
                int y = Mathf.RoundToInt(Mathf.Lerp(y0, y1, t));
                Disc(x, y, thick / 2, c);
            }
        }

        void UpArrow(int cx, int cy, int size, Color c)
        {
            int half = size / 2;
            // shaft (texture space: +y is up)
            RectFill(cx - size / 7, cy - half, 2 * (size / 7), size - half / 2, c);
            // head: apex at top (cy + half), widening downward
            for (int y = 0; y <= half; y++)
            {
                int wy = Mathf.RoundToInt(half * (y / (float)half)); // 0 at apex .. half at base
                RectFill(cx - wy, cy + half - y, wy * 2, 1, c);
            }
        }

        void TurnArrow(int cx, int cy, int size, Color c)
        {
            int half = size / 2;
            RectFill(cx - half, cy - size / 8, size, size / 4, c);            // horizontal shaft
            RectFill(cx - half, cy - half, size / 4, half, c);                // vertical elbow
            for (int x = 0; x <= half; x++)                                   // right-pointing head
            {
                int wy = Mathf.RoundToInt(half * (1f - x / (float)half));
                RectFill(cx + half - x, cy - wy, 1, wy * 2, c);
            }
        }

        void Check(int cx, int cy, int size, Color c)
        {
            ThickLine(cx - size / 2, cy, cx - size / 6, cy - size / 2, 14, c);
            ThickLine(cx - size / 6, cy - size / 2, cx + size / 2, cy + size / 2, 14, c);
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
