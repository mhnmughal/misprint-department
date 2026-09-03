# MOBILE_RELEASE_CHECKLIST — Misprint Department

Reviewed 2026-09-03 (Unity 6000.3.19f1, URP).

## Player Settings (set by `MisprintBuild.ConfigurePlayerSettings`)
| Item | Value |
|------|-------|
| Product name | Misprint Department |
| Company | Misprint Department team |
| Bundle identifier (iOS + Android) | `com.misprintdepartmentteam.misprintdepartment` |
| Orientation | Landscape only (left + right auto-rotate; portrait disabled) |
| Scripting backend (iOS + Android) | IL2CPP |
| Android target architecture | ARM64 |
| App icon | original 1024×1024 (`Assets/Art/Icon/AppIcon.png`) → default + all iOS + Android slots |
| Frame rate | `Application.targetFrameRate = 60`, vSync off (set in `Bootstrap`) |
| Screen sleep | `SleepTimeout.SystemSetting` |

## Build settings
Scenes: `Bootstrap` (0) → `MainWorkshop` (1) → `Gameplay` (2). `SampleScene`
removed.

## Builds produced (this machine — Windows)
| Target | Result | Output |
|--------|--------|--------|
| Standalone Windows 64 | **SUCCESS** — 0 errors, 0 warnings, ~70 s, 105 MB | `Build/Win64/MisprintDepartment.exe` |
| Android (IL2CPP / ARM64) | **SUCCESS** — `.apk` produced | `Build/Android/MisprintDepartment.apk` (~37 MB) |
| iOS | **NOT BUILDABLE HERE** — requires macOS + Xcode | external blocker |

## Permissions
No camera / microphone / contacts / location / photos / Bluetooth / ATT /
tracking permissions requested. The game makes no runtime network calls
(see OFFLINE_AUDIT.md), so no INTERNET permission is needed for gameplay;
strip it from the generated `AndroidManifest.xml` at submission if Unity
adds it by default.

## Safe area
Every screen canvas has a `SafeArea` child driven by `Screen.safeArea`
(`SafeAreaFitter`); all interactive UI lives under it. Verified at 16:9,
19.5:9, 20:9 and 4:3 — no element off-screen.

## Remaining before store submission
- ~~Remove `com.anklebreaker.unity-mcp` + unused packages~~ — **done**
  2026-09-03 (PACKAGE_AUDIT.md). manifest is now 32 deps; `manifest.json`
  backups `.pre-trim.bak` / `.pre-mcp-removal.bak` can be deleted once happy.
- Produce the iOS Xcode project on a Mac; supply signing.
- Store listing copy + screenshots from the finished build.
- On-device pass (touch targets, haptics, audio routing) — the editor pass
  and both desktop/Android player builds are clean.
