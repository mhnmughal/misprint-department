using System.IO;
using UnityEditor;
using UnityEngine;

namespace MisprintDepartment.Editor.Build
{
    /// <summary>Creates the shared URP material palette and simple environment primitives.</summary>
    public static class ArtBuilder
    {
        const string MatDir = "Assets/Art/Materials";

        public static class M
        {
            public const string Paper = "M_Paper";
            public const string PaperEdge = "M_PaperEdge";
            public const string Charcoal = "M_Charcoal";
            public const string CharcoalLight = "M_CharcoalLight";
            public const string Metal = "M_Metal";
            public const string Ink = "M_Ink";
            public const string WarnRed = "M_WarnRed";
            public const string Teal = "M_Teal";
            public const string LampGlow = "M_LampGlow";
            public const string Wood = "M_Wood";
            public const string TileLight = "M_TileLight";
            public const string TileDark = "M_TileDark";
            public const string Carriage = "M_Carriage";
        }

        [MenuItem("Misprint/Build/1 · Material Palette")]
        public static void BuildPalette()
        {
            EnsureDir(MatDir);
            Mat(M.Paper, new Color(0.95f, 0.91f, 0.84f), 0f, 0.12f);
            Mat(M.PaperEdge, new Color(0.89f, 0.84f, 0.74f), 0f, 0.1f);
            Mat(M.Charcoal, new Color(0.17f, 0.17f, 0.18f), 0.15f, 0.25f);
            Mat(M.CharcoalLight, new Color(0.24f, 0.24f, 0.26f), 0.15f, 0.3f);
            Mat(M.Metal, new Color(0.55f, 0.57f, 0.58f), 0.85f, 0.55f);
            Mat(M.Ink, new Color(0.12f, 0.11f, 0.10f), 0.1f, 0.2f);
            Mat(M.WarnRed, new Color(0.71f, 0.23f, 0.18f), 0.1f, 0.35f);
            Mat(M.Teal, new Color(0.25f, 0.56f, 0.57f), 0.2f, 0.45f);
            Mat(M.LampGlow, new Color(0.95f, 0.79f, 0.48f), 0f, 0.9f, emission: new Color(0.95f, 0.79f, 0.48f) * 2.2f);
            Mat(M.Wood, new Color(0.42f, 0.31f, 0.22f), 0f, 0.2f);
            Mat(M.TileLight, new Color(0.90f, 0.86f, 0.78f), 0f, 0.15f);
            Mat(M.TileDark, new Color(0.30f, 0.29f, 0.27f), 0.1f, 0.2f);
            Mat(M.Carriage, new Color(0.71f, 0.23f, 0.18f), 0.3f, 0.4f);
            AssetDatabase.SaveAssets();
            Debug.Log("[Misprint] Material palette built.");
        }

        public static Material Load(string name) =>
            AssetDatabase.LoadAssetAtPath<Material>($"{MatDir}/{name}.mat");

        static Material Mat(string name, Color baseColor, float metallic, float smoothness, Color? emission = null)
        {
            var path = $"{MatDir}/{name}.mat";
            var shader = Shader.Find("Universal Render Pipeline/Lit");
            var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null)
            {
                mat = new Material(shader);
                AssetDatabase.CreateAsset(mat, path);
            }
            mat.shader = shader;
            mat.SetColor("_BaseColor", baseColor);
            mat.SetFloat("_Metallic", metallic);
            mat.SetFloat("_Smoothness", smoothness);
            if (emission.HasValue)
            {
                mat.EnableKeyword("_EMISSION");
                mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
                mat.SetColor("_EmissionColor", emission.Value);
            }
            EditorUtility.SetDirty(mat);
            return mat;
        }

        public static GameObject Prim(PrimitiveType type, string name, Transform parent,
            Vector3 pos, Vector3 scale, Vector3 euler, string material)
        {
            var go = GameObject.CreatePrimitive(type);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = pos;
            go.transform.localScale = scale;
            go.transform.localEulerAngles = euler;
            var mat = Load(material);
            if (mat != null) go.GetComponent<Renderer>().sharedMaterial = mat;
            var col = go.GetComponent<Collider>();
            if (col != null) Object.DestroyImmediate(col);
            return go;
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
