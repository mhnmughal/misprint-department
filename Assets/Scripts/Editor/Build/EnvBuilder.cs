using UnityEditor;
using UnityEngine;

namespace MisprintDepartment.Editor.Build
{
    /// <summary>Shared 2.5D environment pieces for the workshop / gameplay scenes.</summary>
    public static class EnvBuilder
    {
        public static GameObject CameraRig(Vector3 pos, Vector3 euler, float fov)
        {
            var rig = new GameObject("CameraRig");
            var camGo = new GameObject("Main Camera", typeof(Camera));
            camGo.tag = "MainCamera";
            camGo.transform.SetParent(rig.transform, false);
            camGo.transform.localPosition = pos;
            camGo.transform.localEulerAngles = euler;
            var cam = camGo.GetComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.10f, 0.10f, 0.11f);
            cam.fieldOfView = fov;
            cam.nearClipPlane = 0.1f;
            cam.farClipPlane = 60f;
            camGo.AddComponent<AudioListener>();
            return rig;
        }

        public static GameObject Lighting(bool withLamp)
        {
            var root = new GameObject("Lighting");

            // warm interior ambient so paper always reads
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.34f, 0.33f, 0.31f);
            RenderSettings.ambientEquatorColor = new Color(0.26f, 0.25f, 0.24f);
            RenderSettings.ambientGroundColor = new Color(0.12f, 0.11f, 0.10f);
            RenderSettings.fog = false;

            var key = new GameObject("KeyLight", typeof(Light));
            key.transform.SetParent(root.transform, false);
            key.transform.localEulerAngles = new Vector3(52, 28, 0);
            var kl = key.GetComponent<Light>();
            kl.type = LightType.Directional;
            kl.color = new Color(1f, 0.96f, 0.88f);
            kl.intensity = 1.05f;
            kl.shadows = LightShadows.Soft;
            kl.shadowStrength = 0.55f;

            var fill = new GameObject("FillLight", typeof(Light));
            fill.transform.SetParent(root.transform, false);
            fill.transform.localEulerAngles = new Vector3(-20, -140, 0);
            var fl = fill.GetComponent<Light>();
            fl.type = LightType.Directional;
            fl.color = new Color(0.7f, 0.78f, 0.9f);
            fl.intensity = 0.35f;
            fl.shadows = LightShadows.None;

            if (withLamp)
            {
                var lamp = new GameObject("InspectionLampLight", typeof(Light));
                lamp.transform.SetParent(root.transform, false);
                lamp.transform.localPosition = new Vector3(-1.6f, 2.6f, -1.2f);
                var ll = lamp.GetComponent<Light>();
                ll.type = LightType.Point;
                ll.color = new Color(1f, 0.85f, 0.6f);
                ll.intensity = 2.4f;
                ll.range = 7f;
                ll.shadows = LightShadows.Soft;
            }
            return root;
        }

        /// <summary>The shared bench + machine massing. Returns the Environment root.</summary>
        public static GameObject Workstation(string variant)
        {
            var env = new GameObject("Environment");

            var bench = new GameObject("Workbench");
            bench.transform.SetParent(env.transform, false);
            ArtBuilder.Prim(PrimitiveType.Cube, "BenchTop", bench.transform,
                new Vector3(0, -0.15f, 0), new Vector3(9f, 0.3f, 6.5f), Vector3.zero, ArtBuilder.M.Wood);
            ArtBuilder.Prim(PrimitiveType.Cube, "BenchApron", bench.transform,
                new Vector3(0, -0.65f, 2.9f), new Vector3(9f, 0.9f, 0.3f), Vector3.zero, ArtBuilder.M.Wood);

            var printer = new GameObject("PrinterBody");
            printer.transform.SetParent(env.transform, false);
            printer.transform.localPosition = new Vector3(0, 0, -1.6f);
            ArtBuilder.Prim(PrimitiveType.Cube, "Chassis", printer.transform,
                new Vector3(0, 0.55f, 0), new Vector3(5.4f, 1.1f, 2.2f), Vector3.zero, ArtBuilder.M.Charcoal);
            ArtBuilder.Prim(PrimitiveType.Cube, "Hood", printer.transform,
                new Vector3(0, 1.15f, -0.2f), new Vector3(5.0f, 0.5f, 1.7f), new Vector3(-12, 0, 0), ArtBuilder.M.CharcoalLight);
            ArtBuilder.Prim(PrimitiveType.Cube, "FeedSlot", printer.transform,
                new Vector3(0, 0.62f, 1.15f), new Vector3(3.6f, 0.16f, 0.1f), Vector3.zero, ArtBuilder.M.Ink);

            var rollers = new GameObject("RollerAssembly");
            rollers.transform.SetParent(printer.transform, false);
            rollers.transform.localPosition = new Vector3(0, 0.62f, 0.9f);
            for (int i = -1; i <= 1; i++)
            {
                ArtBuilder.Prim(PrimitiveType.Cylinder, $"Roller{i + 2}", rollers.transform,
                    new Vector3(i * 1.4f, 0, 0), new Vector3(0.28f, 1.6f, 0.28f), new Vector3(0, 0, 90),
                    ArtBuilder.M.Metal);
                ArtBuilder.Prim(PrimitiveType.Cylinder, $"RollerRubber{i + 2}", rollers.transform,
                    new Vector3(i * 1.4f, 0, 0), new Vector3(0.34f, 0.9f, 0.34f), new Vector3(0, 0, 90),
                    ArtBuilder.M.Ink);
            }
            // drive gears on the left side of the machine
            var gears = new GameObject("DriveGears");
            gears.transform.SetParent(printer.transform, false);
            gears.transform.localPosition = new Vector3(-2.55f, 0.55f, 0f);
            for (int g = 0; g < 2; g++)
            {
                var gear = ArtBuilder.Prim(PrimitiveType.Cylinder, $"Gear{g}", gears.transform,
                    new Vector3(0, g * 0.55f, 0.55f + g * 0.1f), new Vector3(0.62f - g * 0.18f, 0.08f, 0.62f - g * 0.18f),
                    new Vector3(90, 0, 0), ArtBuilder.M.Metal);
                for (int t = 0; t < 8; t++)
                    ArtBuilder.Prim(PrimitiveType.Cube, $"Tooth{g}_{t}", gear.transform,
                        Quaternion.Euler(0, 0, t * 45f) * new Vector3(0, 0.52f, 0),
                        new Vector3(0.14f, 0.14f, 0.5f), new Vector3(0, 0, t * 45f), ArtBuilder.M.Metal);
            }

            var ink = new GameObject("InkAssembly");
            ink.transform.SetParent(printer.transform, false);
            ink.transform.localPosition = new Vector3(1.9f, 1.0f, 0.1f);
            ArtBuilder.Prim(PrimitiveType.Cube, "Cartridge", ink.transform,
                Vector3.zero, new Vector3(0.7f, 0.5f, 0.9f), Vector3.zero, ArtBuilder.M.WarnRed);
            ArtBuilder.Prim(PrimitiveType.Cube, "CartridgeCap", ink.transform,
                new Vector3(0, 0.32f, 0), new Vector3(0.5f, 0.16f, 0.7f), Vector3.zero, ArtBuilder.M.Ink);
            ArtBuilder.Prim(PrimitiveType.Cube, "CartridgeTeal", ink.transform,
                new Vector3(0, 0f, 0.5f), new Vector3(0.5f, 0.42f, 0.2f), Vector3.zero, ArtBuilder.M.Teal);

            // registration bar + proof tray on the operator side
            ArtBuilder.Prim(PrimitiveType.Cube, "RegistrationBar", printer.transform,
                new Vector3(0, 0.7f, 1.35f), new Vector3(4.4f, 0.05f, 0.05f), Vector3.zero, ArtBuilder.M.WarnRed);
            var tray = ArtBuilder.Prim(PrimitiveType.Cube, "ProofTray", printer.transform,
                new Vector3(0, 0.28f, 1.9f), new Vector3(4.0f, 0.06f, 1.4f), new Vector3(-16, 0, 0), ArtBuilder.M.Metal);
            ArtBuilder.Prim(PrimitiveType.Cube, "TrayLip", tray.transform,
                new Vector3(0, 0.4f, 0.5f), new Vector3(1.02f, 1.4f, 0.06f), Vector3.zero, ArtBuilder.M.CharcoalLight);

            // paper guides feeding the strip
            for (int s = -1; s <= 1; s += 2)
                ArtBuilder.Prim(PrimitiveType.Cube, $"PaperGuide{s}", printer.transform,
                    new Vector3(s * 1.7f, 0.66f, 1.1f), new Vector3(0.08f, 0.22f, 0.5f), Vector3.zero, ArtBuilder.M.Metal);

            var lampArm = new GameObject("InspectionLamp");
            lampArm.transform.SetParent(env.transform, false);
            lampArm.transform.localPosition = new Vector3(-3.4f, 0, -0.4f);
            ArtBuilder.Prim(PrimitiveType.Cylinder, "Post", lampArm.transform,
                new Vector3(0, 1.3f, 0), new Vector3(0.09f, 1.3f, 0.09f), Vector3.zero, ArtBuilder.M.Metal);
            ArtBuilder.Prim(PrimitiveType.Cylinder, "Arm", lampArm.transform,
                new Vector3(0.7f, 2.5f, 0), new Vector3(0.06f, 0.8f, 0.06f), new Vector3(0, 0, 62), ArtBuilder.M.Metal);
            ArtBuilder.Prim(PrimitiveType.Cylinder, "Shade", lampArm.transform,
                new Vector3(1.5f, 2.4f, 0), new Vector3(0.5f, 0.25f, 0.5f), new Vector3(180, 0, 20), ArtBuilder.M.Charcoal);
            ArtBuilder.Prim(PrimitiveType.Sphere, "Bulb", lampArm.transform,
                new Vector3(1.5f, 2.2f, 0), new Vector3(0.22f, 0.22f, 0.22f), Vector3.zero, ArtBuilder.M.LampGlow);

            var props = new GameObject("DecorativeProps");
            props.transform.SetParent(env.transform, false);
            ArtBuilder.Prim(PrimitiveType.Cube, "PaperStack", props.transform,
                new Vector3(3.0f, 0.12f, 1.9f), new Vector3(1.2f, 0.24f, 1.5f), new Vector3(0, 8, 0), ArtBuilder.M.Paper);
            ArtBuilder.Prim(PrimitiveType.Cube, "ProofSheetLoose", props.transform,
                new Vector3(3.15f, 0.26f, 1.75f), new Vector3(1.0f, 0.02f, 1.3f), new Vector3(0, -6, 0), ArtBuilder.M.Paper);
            ArtBuilder.Prim(PrimitiveType.Cylinder, "CorrectionTapeRoll", props.transform,
                new Vector3(-2.4f, 0.12f, 1.7f), new Vector3(0.44f, 0.12f, 0.44f), new Vector3(90, 0, 0), ArtBuilder.M.Teal);
            ArtBuilder.Prim(PrimitiveType.Cylinder, "Counter", props.transform,
                new Vector3(2.4f, 0.15f, -0.6f), new Vector3(0.4f, 0.15f, 0.4f), Vector3.zero, ArtBuilder.M.Metal);
            ArtBuilder.Prim(PrimitiveType.Cube, "PartsBin", props.transform,
                new Vector3(-3.5f, 0.15f, 1.9f), new Vector3(0.9f, 0.3f, 0.7f), new Vector3(0, 12, 0), ArtBuilder.M.CharcoalLight);
            ArtBuilder.Prim(PrimitiveType.Cube, "StampBlock", props.transform,
                new Vector3(-1.9f, 0.16f, -0.5f), new Vector3(0.4f, 0.32f, 0.4f), new Vector3(0, 20, 0), ArtBuilder.M.WarnRed);
            ArtBuilder.Prim(PrimitiveType.Cylinder, "StampHandle", props.transform,
                new Vector3(-1.9f, 0.44f, -0.5f), new Vector3(0.1f, 0.16f, 0.1f), Vector3.zero, ArtBuilder.M.Wood);
            // bench registration marks (thin ink crosses)
            foreach (var p in new[] { new Vector3(3.6f, 0.01f, -2.4f), new Vector3(-3.6f, 0.01f, -2.4f), new Vector3(3.6f, 0.01f, 2.6f) })
            {
                ArtBuilder.Prim(PrimitiveType.Cube, "RegMarkH", props.transform, p, new Vector3(0.4f, 0.01f, 0.05f), Vector3.zero, ArtBuilder.M.Ink);
                ArtBuilder.Prim(PrimitiveType.Cube, "RegMarkV", props.transform, p, new Vector3(0.05f, 0.01f, 0.4f), Vector3.zero, ArtBuilder.M.Ink);
            }

            return env;
        }
    }
}
