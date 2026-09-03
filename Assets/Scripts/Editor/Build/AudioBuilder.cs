using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using MisprintDepartment.Game;

namespace MisprintDepartment.Editor.Build
{
    /// <summary>
    /// Generates 100% original procedural SFX (16-bit PCM WAV), builds the
    /// AudioMixer with Master/Music/SFX/UI groups and exposed volume params,
    /// and populates the SoundBank. Nothing is downloaded.
    /// </summary>
    public static class AudioBuilder
    {
        const int SR = 44100;
        const string SfxDir = "Assets/Audio/SFX";
        const string MixerPath = "Assets/Audio/MisprintMixer.mixer";
        const string BankPath = "Assets/Audio/SoundBank.asset";

        [MenuItem("Misprint/Build/2 · Audio (mixer + generated SFX + bank)")]
        public static void BuildAll()
        {
            EnsureDir(SfxDir);
            GenerateClips();
            AssetDatabase.Refresh();
            var mixer = BuildMixer();
            BuildBank(mixer);
            AssetDatabase.SaveAssets();
            Debug.Log("[Misprint] Audio built: mixer + SFX + SoundBank.");
        }

        // ---------------- synthesis ----------------

        static void GenerateClips()
        {
            Save("ui_select", UiSelect());
            Save("mech_button", MechButton());
            Save("correction_rotate", Rotate());
            Save("correction_replace", Replace());
            Save("printer_start", PrinterStart());
            Save("roller_feed", RollerFeed());
            Save("proof_lever", ProofLever());
            Save("route_step", RouteStep());
            Save("route_turn", RouteTurn());
            Save("proof_approved", Approved());
            Save("proof_rejected", Rejected());
            Save("ticket_move", TicketMove());
            Save("stamp_mark", Stamp());
            Save("ambience_bench", Ambience());
        }

        // A small soft "tock" — clean and quiet, for menu taps.
        static float[] UiSelect()
        {
            var body = Partials(0.10f, 520f, new[] { (1f, 1f), (2.02f, 0.28f) }, 0.09f);
            var tick = Env(LowPass(Noise(0.012f, 771), 4200f), 0.0004f, 0.012f, 0.22f);
            var o = Mix(body, tick);
            return Normalize(Adsr(o, 0.002f, 0.10f), 0.5f);
        }

        // Firm mechanical button — a low click with a woody thump.
        static float[] MechButton()
        {
            var body = Partials(0.09f, 165f, new[] { (1f, 1f), (2.7f, 0.35f), (5.1f, 0.12f) }, 0.06f);
            var click = Env(LowPass(Noise(0.02f, 12345), 3000f), 0.0003f, 0.02f, 0.4f);
            return Normalize(Adsr(Mix(body, click), 0.001f, 0.09f), 0.62f);
        }

        // Rotate ratchet — four soft detented ticks, gently rising.
        static float[] Rotate()
        {
            var outp = new float[(int)(SR * 0.24f)];
            for (int k = 0; k < 4; k++)
            {
                var tone = Partials(0.028f, 280f + k * 70f, new[] { (1f, 1f), (3f, 0.25f) }, 0.02f);
                var grit = Env(LowPass(Noise(0.028f, (ulong)(900 + k)), 3500f), 0.0004f, 0.02f, 0.3f);
                var tick = Adsr(Mix(tone, grit), 0.0006f, 0.026f);
                Overlay(outp, tick, (int)(SR * (0.015f + k * 0.052f)));
            }
            return Normalize(outp, 0.6f);
        }

        // Replace — a rubber-stamp press: soft ink squelch + low thud.
        static float[] Replace()
        {
            var thud = Partials(0.13f, 128f, new[] { (1f, 1f), (2.3f, 0.3f) }, 0.11f);
            var squelch = Env(LowPass(Noise(0.10f, 4242), 1600f), 0.004f, 0.09f, 0.22f);
            return Normalize(Adsr(Mix(thud, squelch), 0.001f, 0.13f), 0.66f);
        }

        // Printer motor spin-up — smooth rising hum, no aliasing.
        static float[] PrinterStart()
        {
            int n = (int)(SR * 0.75f);
            var outp = new float[n];
            double ph1 = 0, ph2 = 0;
            for (int i = 0; i < n; i++)
            {
                float t = i / (float)SR;
                float f = Mathf.Lerp(42f, 116f, Ease(Mathf.Clamp01(t / 0.62f)));
                ph1 += 2 * Mathf.PI * f / SR;
                ph2 += 2 * Mathf.PI * (f * 2f) / SR;
                outp[i] = (float)(System.Math.Sin(ph1) * 0.5 + System.Math.Sin(ph2) * 0.16);
            }
            var bed = Env(LowPass(Noise(0.75f, 77), 900f), 0.06f, 0.35f, 0.10f);
            var o = LowPass(Mix(outp, bed), 2400f);
            return Normalize(Adsr(o, 0.05f, 0.2f), 0.55f);
        }

        // Roller / paper feed — soft filtered rush with faint periodic ticks.
        static float[] RollerFeed()
        {
            int n = (int)(SR * 0.5f);
            var outp = new float[n];
            var rng = new Rng(9001);
            float lp = 0f;
            for (int i = 0; i < n; i++)
            {
                float w = (float)(rng.NextDouble() * 2 - 1);
                lp += (w - lp) * 0.05f;
                float tick = (i % 1700 < 22) ? 0.22f * (float)(rng.NextDouble()) : 0f;
                outp[i] = lp * 0.2f + tick;
            }
            return Normalize(Adsr(LowPass(outp, 2200f), 0.03f, 0.14f), 0.5f);
        }

        // Run Proof lever — a chunky two-stage throw: pull, then a heavy latch.
        static float[] ProofLever()
        {
            var pull = Env(LowPass(Noise(0.11f, 555), 2400f), 0.003f, 0.10f, 0.24f);
            var clunk = Partials(0.16f, 88f, new[] { (1f, 1f), (2.4f, 0.4f), (4.6f, 0.12f) }, 0.13f);
            var outp = Mix(pull, clunk);
            var latch = Adsr(Mix(Partials(0.05f, 240f, new[] { (1f, 1f), (2.1f, 0.3f) }, 0.03f),
                                 Env(LowPass(Noise(0.04f, 32), 3000f), 0.0005f, 0.03f, 0.35f)), 0.0006f, 0.045f);
            Overlay(outp, latch, (int)(SR * 0.11f));
            return Normalize(Adsr(outp, 0.002f, 0.2f), 0.82f);
        }

        // Carriage step — a soft low tap.
        static float[] RouteStep() =>
            Normalize(Adsr(Partials(0.05f, 210f, new[] { (1f, 1f), (2.6f, 0.2f) }, 0.035f), 0.0008f, 0.05f), 0.4f);

        // Carriage turn — a short upward pip.
        static float[] RouteTurn()
        {
            int n = (int)(SR * 0.08f);
            var outp = new float[n];
            double ph = 0;
            for (int i = 0; i < n; i++)
            {
                float t = i / (float)n;
                float f = Mathf.Lerp(280f, 460f, Ease(t));
                ph += 2 * Mathf.PI * f / SR;
                outp[i] = (float)System.Math.Sin(ph) * 0.3f;
            }
            return Normalize(Adsr(outp, 0.001f, 0.06f), 0.4f);
        }

        // Proof approved — a warm QC-desk bell: struck partials + soft sub, gentle tail.
        static float[] Approved()
        {
            var bell = Partials(0.9f, 392f, new[]
            {
                (1f, 1f), (2.0f, 0.5f), (2.99f, 0.28f), (4.2f, 0.12f), (5.4f, 0.06f),
            }, 0.55f);
            var sub = Partials(0.7f, 196f, new[] { (1f, 1f) }, 0.5f);
            var strike = Env(LowPass(Noise(0.02f, 12), 2600f), 0.0004f, 0.018f, 0.28f);
            var o = Mix(Mix(bell, Scale(sub, 0.5f)), strike);
            return Normalize(Adsr(o, 0.002f, 0.85f), 0.8f);
        }

        // Reprint required — a soft low buzzer (triangle, not square) + dull thud.
        static float[] Rejected()
        {
            int n = (int)(SR * 0.34f);
            var buzz = new float[n];
            double ph = 0;
            for (int i = 0; i < n; i++)
            {
                ph += 2 * Mathf.PI * 96f / SR;
                float tri = (float)(System.Math.Asin(System.Math.Sin(ph)) * (2.0 / Mathf.PI)); // triangle
                buzz[i] = tri * 0.22f;
            }
            var thud = Partials(0.22f, 66f, new[] { (1f, 1f), (2.2f, 0.35f) }, 0.19f);
            var o = LowPass(Mix(buzz, thud), 1400f);
            return Normalize(Adsr(o, 0.006f, 0.16f), 0.7f);
        }

        static float[] TicketMove() =>
            Normalize(Adsr(LowPass(Noise(0.15f, 606), 2600f), 0.012f, 0.13f), 0.34f);

        // QC stamp — sharp ink impact.
        static float[] Stamp()
        {
            var hit = Partials(0.13f, 104f, new[] { (1f, 1f), (2.5f, 0.4f), (4.8f, 0.15f) }, 0.10f);
            var snap = Env(LowPass(Noise(0.04f, 88), 4000f), 0.0004f, 0.03f, 0.4f);
            return Normalize(Adsr(Mix(hit, snap), 0.0004f, 0.12f), 0.78f);
        }

        // 6 s loopable bench ambience — quiet hum + slow air.
        static float[] Ambience()
        {
            int n = SR * 6;
            var outp = new float[n];
            var rng = new Rng(2024);
            float lp = 0f;
            double p1 = 0, p2 = 0;
            for (int i = 0; i < n; i++)
            {
                p1 += 2 * Mathf.PI * 50f / SR;
                p2 += 2 * Mathf.PI * 100f / SR;
                float hum = (float)(System.Math.Sin(p1) * 0.05 + System.Math.Sin(p2) * 0.014);
                float w = (float)(rng.NextDouble() * 2 - 1);
                lp += (w - lp) * 0.012f;
                outp[i] = hum + lp * 0.022f;
            }
            int f = SR / 2;
            for (int i = 0; i < f; i++) { float g = i / (float)f; outp[i] *= g; outp[n - 1 - i] *= g; }
            return Normalize(outp, 0.26f);
        }

        // ---------------- dsp helpers ----------------

        struct Rng
        {
            ulong _s;
            public Rng(ulong seed) { _s = seed == 0 ? 1 : seed; }
            public double NextDouble()
            {
                _s ^= _s << 13; _s ^= _s >> 7; _s ^= _s << 17;
                return (_s % 1000000) / 1000000.0;
            }
        }

        static float[] Noise(float seconds, ulong seed)
        {
            int n = (int)(SR * seconds);
            var s = new float[n];
            var rng = new Rng(seed);
            for (int i = 0; i < n; i++) s[i] = (float)(rng.NextDouble() * 2 - 1);
            return s;
        }

        static float[] Env(float[] s, float attack, float release, float peak)
        {
            int a = Mathf.Max(1, (int)(SR * attack));
            int r = Mathf.Max(1, (int)(SR * release));
            int n = s.Length;
            var o = new float[n];
            for (int i = 0; i < n; i++)
            {
                float g = 1f;
                if (i < a) g = i / (float)a;
                else if (i > n - r) g = Mathf.Max(0f, (n - i) / (float)r);
                o[i] = Mathf.Clamp(s[i] * g * peak, -1f, 1f);
            }
            return o;
        }

        static float[] Mix(float[] a, float[] b)
        {
            int n = Mathf.Max(a.Length, b.Length);
            var o = new float[n];
            for (int i = 0; i < n; i++)
            {
                float v = 0f;
                if (i < a.Length) v += a[i];
                if (i < b.Length) v += b[i];
                o[i] = v; // headroom kept; Normalize() tames the final peak
            }
            return o;
        }

        static void Overlay(float[] target, float[] add, int offset)
        {
            for (int i = 0; i < add.Length && offset + i < target.Length; i++)
                target[offset + i] += add[i];
        }

        static float[] Scale(float[] s, float k)
        {
            var o = new float[s.Length];
            for (int i = 0; i < s.Length; i++) o[i] = s[i] * k;
            return o;
        }

        static float Ease(float t) => t * t * (3f - 2f * t);

        /// <summary>Struck-partial voice: each (ratio, amp) is a sine that decays over the clip.</summary>
        static float[] Partials(float seconds, float baseFreq, (float ratio, float amp)[] parts, float decayTime)
        {
            int n = (int)(SR * seconds);
            var o = new float[n];
            foreach (var (ratio, amp) in parts)
            {
                double ph = 0;
                double inc = 2 * Mathf.PI * (baseFreq * ratio) / SR;
                for (int i = 0; i < n; i++)
                {
                    ph += inc;
                    float dk = Mathf.Exp(-(i / (float)SR) / Mathf.Max(0.01f, decayTime));
                    o[i] += (float)System.Math.Sin(ph) * amp * dk;
                }
            }
            return o;
        }

        /// <summary>Smooth attack + exponential release.</summary>
        static float[] Adsr(float[] s, float attack, float release)
        {
            int n = s.Length;
            int a = Mathf.Max(1, (int)(SR * attack));
            int r = Mathf.Max(1, (int)(SR * release));
            var o = new float[n];
            for (int i = 0; i < n; i++)
            {
                float g = 1f;
                if (i < a) g = Ease(i / (float)a);
                int fromEnd = n - i;
                if (fromEnd < r) g *= Mathf.Exp(-3f * (1f - fromEnd / (float)r));
                o[i] = s[i] * g;
            }
            return o;
        }

        /// <summary>One-pole low-pass.</summary>
        static float[] LowPass(float[] s, float cutoffHz)
        {
            float dt = 1f / SR;
            float rc = 1f / (2 * Mathf.PI * cutoffHz);
            float alpha = dt / (rc + dt);
            var o = new float[s.Length];
            float y = 0f;
            for (int i = 0; i < s.Length; i++) { y += alpha * (s[i] - y); o[i] = y; }
            return o;
        }

        /// <summary>Scale so the largest absolute sample equals target (also removes DC).</summary>
        static float[] Normalize(float[] s, float target)
        {
            double sum = 0;
            for (int i = 0; i < s.Length; i++) sum += s[i];
            float dc = (float)(sum / Mathf.Max(1, s.Length));
            float peak = 1e-6f;
            for (int i = 0; i < s.Length; i++) peak = Mathf.Max(peak, Mathf.Abs(s[i] - dc));
            float k = target / peak;
            var o = new float[s.Length];
            for (int i = 0; i < s.Length; i++) o[i] = Mathf.Clamp((s[i] - dc) * k, -1f, 1f);
            return o;
        }

        static void Save(string name, float[] samples)
        {
            var path = $"{SfxDir}/{name}.wav";
            File.WriteAllBytes(path, EncodeWav(samples, SR));
        }

        static byte[] EncodeWav(float[] samples, int sampleRate)
        {
            using var ms = new MemoryStream();
            using var w = new BinaryWriter(ms);
            int dataBytes = samples.Length * 2;
            w.Write(new[] { 'R', 'I', 'F', 'F' });
            w.Write(36 + dataBytes);
            w.Write(new[] { 'W', 'A', 'V', 'E' });
            w.Write(new[] { 'f', 'm', 't', ' ' });
            w.Write(16);
            w.Write((short)1);            // PCM
            w.Write((short)1);            // mono
            w.Write(sampleRate);
            w.Write(sampleRate * 2);      // byte rate
            w.Write((short)2);            // block align
            w.Write((short)16);           // bits
            w.Write(new[] { 'd', 'a', 't', 'a' });
            w.Write(dataBytes);
            foreach (var f in samples)
                w.Write((short)(Mathf.Clamp(f, -1f, 1f) * 32760));
            w.Flush();
            return ms.ToArray();
        }

        // ---------------- mixer (reflection into UnityEditor.Audio) ----------------

        static AudioMixer BuildMixer()
        {
            var existing = AssetDatabase.LoadAssetAtPath<AudioMixer>(MixerPath);
            if (existing != null) return existing;

            var ctrlType = Type.GetType("UnityEditor.Audio.AudioMixerController, UnityEditor");
            var groupType = Type.GetType("UnityEditor.Audio.AudioMixerGroupController, UnityEditor");
            var pathType = Type.GetType("UnityEditor.Audio.AudioGroupParameterPath, UnityEditor");

            var create = ctrlType.GetMethod("CreateMixerControllerAtPath", BindingFlags.Public | BindingFlags.Static);
            var ctrl = create.Invoke(null, new object[] { MixerPath });

            var masterGroup = ctrlType.GetProperty("masterGroup").GetValue(ctrl);
            var createGroup = ctrlType.GetMethod("CreateNewGroup", new[] { typeof(string), typeof(bool) });
            var addChild = ctrlType.GetMethod("AddChildToParent");
            var addExposed = ctrlType.GetMethod("AddExposedParameter");
            var preAlloc = groupType.GetMethod("PreallocateGUIDs");
            var guidForVolume = groupType.GetMethod("GetGUIDForVolume");

            ExposeVolume(ctrl, masterGroup, groupType, pathType, addExposed, preAlloc, guidForVolume, "MasterVolume");

            foreach (var name in new[] { "Music", "SFX", "UI" })
            {
                var g = createGroup.Invoke(ctrl, new object[] { name, false });
                addChild.Invoke(ctrl, new[] { g, masterGroup });
                string param = name == "SFX" ? "SFXVolume" : name == "UI" ? "UIVolume" : "MusicVolume";
                ExposeVolume(ctrl, g, groupType, pathType, addExposed, preAlloc, guidForVolume, param);
            }

            EditorUtility.SetDirty((UnityEngine.Object)ctrl);
            AssetDatabase.SaveAssets();
            return (AudioMixer)ctrl;
        }

        static void ExposeVolume(object ctrl, object group, Type groupType, Type pathType,
            MethodInfo addExposed, MethodInfo preAlloc, MethodInfo guidForVolume, string paramName)
        {
            preAlloc.Invoke(group, null);
            var guid = guidForVolume.Invoke(group, null);
            var path = Activator.CreateInstance(pathType, new[] { group, guid });
            addExposed.Invoke(ctrl, new[] { path });

            // rename the freshly added exposed parameter
            var exposedProp = ctrl.GetType().GetProperty("exposedParameters");
            var arr = (Array)exposedProp.GetValue(ctrl);
            var elemType = arr.GetType().GetElementType();
            var nameField = elemType.GetField("name");
            var guidField = elemType.GetField("guid");
            for (int i = 0; i < arr.Length; i++)
            {
                var e = arr.GetValue(i);
                if (Equals(guidField.GetValue(e), guid))
                {
                    nameField.SetValue(e, paramName);
                    arr.SetValue(e, i);
                }
            }
            exposedProp.SetValue(ctrl, arr);
        }

        // ---------------- sound bank ----------------

        static void BuildBank(AudioMixer mixer)
        {
            var bank = AssetDatabase.LoadAssetAtPath<SoundBank>(BankPath);
            if (bank == null)
            {
                bank = ScriptableObject.CreateInstance<SoundBank>();
                AssetDatabase.CreateAsset(bank, BankPath);
            }
            bank.entries.Clear();
            Add(bank, Sfx.UiSelect, "ui_select", SfxBus.Ui, 0.55f);
            Add(bank, Sfx.MechButton, "mech_button", SfxBus.Ui, 0.7f);
            Add(bank, Sfx.CorrectionRotate, "correction_rotate", SfxBus.Sfx, 0.7f);
            Add(bank, Sfx.CorrectionReplace, "correction_replace", SfxBus.Sfx, 0.75f);
            Add(bank, Sfx.PrinterStart, "printer_start", SfxBus.Sfx, 0.6f);
            Add(bank, Sfx.RollerFeed, "roller_feed", SfxBus.Sfx, 0.5f);
            Add(bank, Sfx.ProofLever, "proof_lever", SfxBus.Sfx, 0.8f);
            Add(bank, Sfx.RouteStep, "route_step", SfxBus.Sfx, 0.45f);
            Add(bank, Sfx.RouteTurn, "route_turn", SfxBus.Sfx, 0.45f);
            Add(bank, Sfx.ProofApproved, "proof_approved", SfxBus.Sfx, 0.8f);
            Add(bank, Sfx.ProofRejected, "proof_rejected", SfxBus.Sfx, 0.7f);
            Add(bank, Sfx.TicketMove, "ticket_move", SfxBus.Ui, 0.5f);
            Add(bank, Sfx.StampMark, "stamp_mark", SfxBus.Sfx, 0.75f);
            bank.ambience = LoadClip("ambience_bench");
            EditorUtility.SetDirty(bank);
        }

        static void Add(SoundBank bank, Sfx id, string file, SfxBus bus, float vol)
        {
            var clip = LoadClip(file);
            if (clip == null) { Debug.LogWarning($"[Misprint] missing clip {file}"); return; }
            bank.entries.Add(new SoundBank.Entry { id = id, clip = clip, bus = bus, volume = vol });
        }

        static AudioClip LoadClip(string file) =>
            AssetDatabase.LoadAssetAtPath<AudioClip>($"{SfxDir}/{file}.wav");

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
