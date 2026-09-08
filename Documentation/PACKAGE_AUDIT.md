# PACKAGE_AUDIT — Misprint Department

Reviewed + **trimmed** 2026-09-03 (Unity 6000.3.19f1).

## Trim performed (2026-09-03)
Removed from `Packages/manifest.json`, project re-resolved, **0 compile
errors / 0 warnings, 48/48 tests pass, Windows build succeeds** (25 s / 102 MB,
down from 70 s / 105 MB), rendering unchanged:
- `com.unity.timeline`, `com.unity.visualscripting`,
  `com.unity.multiplayer.center`, `com.unity.collab-proxy`,
  `com.unity.ide.rider`
- 2D tooling: `com.unity.2d.animation`, `2d.aseprite`, `2d.psdimporter`,
  `2d.spriteshape`, `2d.tilemap.extras`, `2d.tooling`
- unused engine modules: `unityanalytics`, `vectorgraphics`, `cloth`,
  `terrain`+`terrainphysics` (terrain re-added as a transitive dep — harmless),
  `vehicles`, `wind`, `vr`, `xr`

Kept `com.unity.2d.sprite` + `com.unity.2d.tilemap` (small; the active
`Renderer2D` renderer path). `manifest.json.pre-trim.bak` holds the previous
state.

**`com.anklebreaker.unity-mcp` was removed as the final step** — the editor
recompiled once cleanly (Editor.log: 1 domain reload, no `error CS`), 99 package
files deleted, all `MisprintDepartment.*` assemblies intact, and the bridge
port (7890) closed as expected. Nothing in the game references it. Backup:
`manifest.json.pre-mcp-removal.bak`. To use the MCP tooling again, re-add the
line `"com.anklebreaker.unity-mcp": "https://github.com/AnkleBreaker-Studio/unity-mcp-plugin.git"`.

## Original review (pre-trim) against `Packages/manifest.json`

## Required for the shipping game
| Package | Why |
|---------|-----|
| com.unity.render-pipelines.universal 17.3.0 | render pipeline (URP) |
| com.unity.ugui 2.0.0 | uGUI + TextMeshPro (all UI + fonts) |
| com.unity.inputsystem 1.19.0 | `InputSystemUIInputModule` (project is New-Input-only) |
| com.unity.modules.* (audio, ui, uielements, imgui, jsonserialize, imageconversion, screencapture, particlesystem, physics, animation, director) | engine modules pulled by URP / uGUI / TMP; the game code itself uses only audio + ui + jsonserialize + imageconversion (icon PNG encode) |

## Dev-only (not shipped / disabled in player builds)
| Package | Note |
|---------|------|
| com.anklebreaker.unity-mcp | editor tooling — **remove before a store submission** |
| com.unity.test-framework 1.6.0 | EditMode/PlayMode tests; editor-only, stripped from player |
| com.unity.ide.rider / ide.visualstudio | IDE integration, editor-only |
| com.unity.collab-proxy | VCS UI, editor-only |

## Removal attempt (2026-09-03) — REVERTED
A headless trim of `timeline`, `visualscripting` and `multiplayer.center` was
attempted via `manifest.json` + `Client.Resolve()`. The project recompiled
with **0 errors** and all game assemblies loaded, but the editor's
`isCompiling` state did not settle cleanly over ~10 min through the MCP
connection, so the manifest was **reverted to the full set** to keep a
known-good build state. Do this trim in the Editor's Package Manager UI
(where progress is visible) as a deliberate pre-submission step, one package
at a time, compiling + building between each.

## Unused — safe to remove (left in place; trim before submission)
com.unity.2d.animation, 2d.aseprite, 2d.psdimporter, 2d.sprite, 2d.spriteshape,
2d.tilemap, 2d.tilemap.extras, 2d.tooling, com.unity.timeline,
com.unity.visualscripting, com.unity.multiplayer.center,
com.unity.modules.{ai, cloth, terrain, terrainphysics, tilemap, vehicles,
video, vr, xr, wind, umbra, assetbundle, unityanalytics}, vectorgraphics,
adaptiveperformance.

None of these are referenced by game code (`Assets/Scripts/`). The four
`unitywebrequest*` modules and `unityanalytics` are engine stubs kept for
package-manager compatibility; **no game code calls them** (see
OFFLINE_AUDIT.md). Removing the 2D/timeline/visualscripting set is a clean
follow-up before submission.

## No third-party runtime SDKs
No Firebase / AdMob / Facebook / GameAnalytics / AppsFlyer / Unity Ads /
attribution / remote-config / analytics packages are present. **PASS.**
