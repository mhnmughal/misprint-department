# PACKAGE_AUDIT — Misprint Department

Last reviewed 2026-09-08 (Unity 6000.3.19f1). Committed manifest = 32 direct
dependencies.

## Current state

**Direct dependencies that are real downloads (7):**

| Package | Why it stays |
|---------|--------------|
| `com.unity.render-pipelines.universal` 17.3.0 | the render pipeline |
| `com.unity.ugui` 2.0.0 | uGUI + TextMeshPro — every screen + all fonts |
| `com.unity.inputsystem` 1.19.0 | `InputSystemUIInputModule` (project is New-Input only) |
| `com.unity.test-framework` 1.6.0 | the 48 EditMode tests — editor-only, stripped from the player |
| `com.unity.ide.visualstudio` 2.0.26 | IDE project generation — editor-only |
| `com.unity.2d.sprite`, `com.unity.2d.tilemap` 1.0.0 | small; kept for the active Renderer2D path |

The other 25 direct entries are `com.unity.modules.*` **engine modules** —
they ship inside the Unity install, not as downloads, and Unity's build-time
code stripping (`stripEngineCode: 1` is set) drops any unreferenced engine
code from the actual player binary.

**Game code (`Assets/Scripts/`) uses only:** `audio`, `ui` / `uielements` /
`imgui`, `jsonserialize` (save file), `imageconversion` (editor icon PNG
encode). `physics` is load-bearing — `com.unity.render-pipelines.core`
depends on it. Everything else is either a transitive requirement or inert.

## Trims already done

- **2026-09-03 (MCP-0021):** removed `com.unity.timeline`,
  `com.unity.visualscripting`, `com.unity.multiplayer.center`,
  `com.unity.collab-proxy`, `com.unity.ide.rider`, the 2D tooling set
  (`2d.animation` / `2d.aseprite` / `2d.psdimporter` / `2d.spriteshape` /
  `2d.tilemap.extras` / `2d.tooling`), and engine modules `unityanalytics`,
  `vectorgraphics`, `cloth`, `terrain` + `terrainphysics`, `vehicles`,
  `wind`, `vr`, `xr`. Verified: 0 errors, 48/48 tests, Windows + Android
  builds. (`terrain` returns as a transitive of URP core — harmless.)
- **2026-09-08 (MCP-0021 / 0023):** removed `com.anklebreaker.unity-mcp`
  (editor bridge) and, after the reviewer video, `com.unity.recorder` +
  `com.unity.timeline`.

## Still prunable — but do it in the Package Manager UI, not a bulk edit

These `com.unity.modules.*` are not referenced by game code or by any scene /
prefab, and nothing else in the lock depends on them:

`ai` (NavMesh), `androidjni`, `assetbundle`, `particlesystem`,
`screencapture`, `video`, and the networking set `unitywebrequest` +
`unitywebrequestwww` + `unitywebrequestassetbundle` + `unitywebrequestaudio` +
`unitywebrequesttexture`.

They cost effectively nothing in the shipped binary (engine stripping removes
them), so this is cosmetic. **A past bulk manifest trim left the editor's
`isCompiling` state stuck for ~10 minutes** (see the 2026-09-03 note in git
history) — so if you want these gone, remove them **one at a time in
Window > Package Manager > In Project**, letting the editor settle and doing a
test build between each. Keep `manifest.json` backed up.

## Analytics / telemetry — OFF

- No `com.unity.services.*`, `com.unity.analytics`, `com.unity.ads`, or
  `com.unity.purchasing` packages. `com.unity.modules.unityanalytics` was
  removed in MCP-0021.
- No analytics / tracking / crash-reporting API calls in `Assets/Scripts/`.
- `ProjectSettings/UnityConnectSettings.asset` (2026-09-08): every flag is
  `0` — top-level `m_Enabled`, `InsightsSettings` (incl.
  `m_EngineDiagnosticsEnabled` and `m_CaptureEditorExceptions`),
  `UnityAnalyticsSettings` (incl. `m_InitializeOnStartup`), `UnityAdsSettings`
  (incl. `m_InitializeOnStartup`), `CrashReportingSettings`,
  `PerformanceReportingSettings`, `UnityPurchasingSettings`.

`com.unity.test-framework.performance` (a URP transitive) writes a
`PerformanceTestRunInfo.json` build-metadata TextAsset into every build — it
is not analytics, sends nothing, and Apple does not flag it. See
`APP_REVIEW_COMPLIANCE.md` §1.

## No third-party runtime SDKs

No Firebase / AdMob / Facebook / GameAnalytics / AppsFlyer / Unity Ads /
attribution / remote-config packages. **PASS.**
