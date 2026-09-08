# MISPRINT DEPARTMENT — SUMMARY BANK

Living development memory. Update after every meaningful milestone
(AI_RULES.md Rules 3, 13).

## PROJECT STATUS
`IN_PROGRESS`  —  the requested **12-level** game (Training + 12, 6 batches ×
2) is feature-complete, all 6 correction tools + all defect families work with
full UI flows, 48 EditMode tests + 13/13 level validation pass, console is
clean, and **Windows + Android (IL2CPP/ARM64) player builds succeed**.
Remaining: iOS binary (needs macOS + Xcode — external blocker) and the
pre-submission package trim. See `Documentation/FINAL_AUDIT.md`.

## CURRENT MILESTONE
M3 — 12-level campaign + all tools + Color Pass + builds. **COMPLETE** (2026-09-03).
- Scope reset by the user to **12 levels** (from 42): 6 batches × 2, one
  defect family + one tool per batch, L12 combines all six tools.
- Engine: `RouteSimulator` now skips `Striped` tokens / runs `Marked` twice
  + `StepTrace`; `ColorPassDefect`; `CorrectionEngine.Restore` clears
  striped ink; `TotalLevels` 42 → 12; validator range 1..12 / batches 1..6.
- 12 levels authored + validated (13/13 incl. Training). Every correction
  tool has a real UI flow: Rotate/Replace/Shift/Restore-faded = popups,
  Cross Out + Restore-striped = tap-apply, Swap = two-tap. Tool stays
  selected to chain corrections.
- TokenView shows striped ("//" badge + metal frame) / marked ("x2") —
  border+icon, not colour-only. First-use hint toasts per new mechanic.
- Job Files: all 13 tickets, 4-col grid, batch labels; batch sign-off beat
  on even levels; "ALL BATCHES CLEARED" on L12.
- Environment: added drive gears, proof tray, paper guides, ink cartridge
  detail, stamp block, bench registration marks.
- `Application.targetFrameRate = 60`, vSync off.
- 48 EditMode tests pass. Windows + Android `.apk` builds succeed (0 errors,
  0 warnings from our code). All Documentation/* updated + FINAL_AUDIT.

## PREVIOUS MILESTONE
M2.1 — polish + icon + tutorial level. **COMPLETE** (2026-09-03).
- Original 1024×1024 app icon (misprinted strip through a printer roller,
  one instruction corrected) built by `IconBuilder`, assigned to default /
  iOS (all 5 slots) / Android.
- UI polish: EdgeFrame keylines on all paper cards, registration-mark corner
  details, rotated double-ring QC "PASS" / rating stamp, button bottom lip,
  heading rules, character/line spacing, selection = teal keyline frame
  (BUG-021 fixed), tutorial bubble repositioned + own blocking group
  (BUG-022 fixed), gameplay board recentred (BUG-023 fixed).
- SFX rewritten: struck-partial voices, one-pole low-pass, smooth ADSR,
  per-clip normalisation; volumes retuned. 14 clips regenerated.
- **Training Job** (level 0): dedicated 6-step guided walkthrough that flows
  into Job 001; replayable "TR" ticket in Job Files; persisted as
  `tutorialsSeen:"training"`. 44/44 EditMode tests pass (added training test).
- Verified full new-player flow in Play Mode: Start Training → 6 steps →
  Master Proof → Next Job → Job 001 (no tutorial). Console clean.

## PREVIOUS MILESTONE
M2 — Chapter 1 vertical slice. **COMPLETE** (2026-09-03).
A player can: launch → Main Workshop → Start Inspection → Job 1 → learn via
contextual tutorial → apply Rotate/Replace corrections → Run Proof (animated
route) → succeed/fail → retry → earn rating → unlock next job → return to
Workshop → quit → relaunch → progress retained → change working settings.
All verified in Play Mode. 43/43 EditMode tests pass. Console clean.

NEXT MILESTONE: **M3 — Chapters 2–7 Production** (see NEXT EXACT ACTIONS).
PROJECT STATUS remains IN_PROGRESS.

### M2.1 polish backlog (non-blocking, do alongside M3)
- BUG-021 ToolButton selected-state inner panel covers the tool label.
- BUG-022 tutorial bubble overlaps Defect card on steps 0/2 at 16:9.
- BUG-023 Gameplay board renders slightly right of centre.
- Environment is blocky primitive massing — add printer detail, registration
  marks, proof-tray, gears; richer materials.
- PlayMode test runner enumerates the flow suite as 1 test via MCP (runs +
  passes, but count is opaque) — investigate or convert to per-level cases.

## LAST VERIFIED BUILD
2026-09-03, Unity 6000.3.19f1, URP.
- **StandaloneWindows64** → `Build/Win64/MisprintDepartment.exe` — SUCCESS,
  0 errors, 0 warnings, ~70 s, 105 MB.
- **Android (IL2CPP / ARM64)** → `Build/Android/MisprintDepartment.apk` —
  SUCCESS, ~37 MB.
- iOS — needs macOS + Xcode (external blocker). Player Settings are iOS-ready.
Player Settings: product "Misprint Department", company "Misprint Department
team", bundle `com.misprintdepartmentteam.misprintdepartment`, landscape-only,
iOS+Android IL2CPP, Android ARM64, original 1024² icon, 60 fps. Build Settings scenes:
Bootstrap(0), MainWorkshop(1), Gameplay(2). SampleScene deleted.

## SCENES
| Scene | Responsibility | Status |
|-------|----------------|--------|
| Bootstrap | Init services + save + audio, splash, → MainWorkshop | built, verified |
| MainWorkshop | QC-desk menu + Job Files + Settings + Manual | built, verified |
| Gameplay | Data-driven level play (board + strip + tray + proof) | built, verified |

All three authored by editor `*SceneBuilder` scripts (menu `Misprint/Build/*`);
re-run `Misprint/Build/0 · REBUILD EVERYTHING` to regenerate from code.

## IMPORTANT PREFABS
`Assets/Prefabs/UI/`: TokenView, ToolButton, JobTicket, Chip.
`Assets/Prefabs/Board/`: CellTile, BlockedTile, Carriage.

## ASSETS BUILT
- `Assets/Art/Materials/` — 13 URP materials (palette).
- `Assets/Audio/` — MisprintMixer.mixer (Master/Music/SFX/UI), SoundBank.asset,
  `SFX/*.wav` ×14 (all procedurally generated, see AUDIO_PROVENANCE).
- `Assets/Content/LevelDatabase.asset` — 6 levels.
- `Assets/TextMesh Pro/` — TMP essential resources (LiberationSans SDF).

## GAMEPLAY SYSTEMS
| System | Status | Scripts | Tests |
|--------|--------|---------|-------|
| Command language | DONE | Core/PrintCommand.cs, CommandToken.cs, CommandStrip.cs, Cell.cs | 32 EditMode pass |
| Route simulation | DONE (deterministic) | Core/RouteSimulator.cs, BoardConfig.cs | covered |
| Defect rules | DONE (Rotate/Mirror/Duplicate/Shift/Fade/Swap/Composite) | Core/DefectRule.cs | covered |
| Correction tools | DONE (all 6) | Core/Correction.cs | covered |
| Level data | DONE | Core/LevelDefinition.cs | n/a |
| Proof judge + ratings | DONE | Core/LevelJudge.cs | covered |
| Level validator | DONE | Editor/LevelValidator.cs, ChapterBuilders.cs | LevelAssetTests |
| Save system | DONE | Game/SaveService.cs, SaveData.cs | 9 tests pass |
| Scene handoff | DONE | Game/GameSession.cs | n/a |
| Audio system | DONE | Game/Audio/AudioController.cs, SoundBank.cs, Sfx.cs; Editor/Build/AudioBuilder.cs | mixer params verified in Play Mode |
| Scene glue / LevelController | DONE | Game/Gameplay/LevelController.cs, RouteView.cs, GameplayFlow.cs | Play Mode verified L1/L2 full loop |
| Route visualisation | DONE | Game/Gameplay/RouteView.cs (uses RouteResult.trace) | carriage animates board grid |
| UI (all Chapter-1 screens) | DONE (authored) | Game/UI/*.cs + Editor/Build/*SceneBuilder.cs | see UI_QA.md — all PASS |
| Tutorial (Job 1) | DONE | Game/UI/TutorialLayer.cs | 4 action-gated steps, persists |
| Environment / camera / lighting | DONE (primitive massing) | Editor/Build/EnvBuilder.cs | readable; detail pass = M2.1 |
| Icon | not started | — | M3 |
| Mobile player settings | DONE | Editor/Build/MisprintBuild.ConfigurePlayerSettings | landscape, bundle id, IL2CPP/ARM64 |

## UI SYSTEM
Authored (scene + prefab), no runtime construction. Canvas per scene
(Overlay, CanvasScaler ref 1920x1080 match 0.5), SafeArea child +
`SafeAreaFitter`, EventSystem + InputSystemUIInputModule. Screens listed in
`Documentation/UI_UX_SYSTEM.md`. Navigation: `WorkshopNav`
(menu panels) + `GameplayFlow` (gameplay bindings + job flow). Palette +
spacing tokens in `UiKit`. Font LiberationSans SDF (SIL OFL).

## AUDIO SYSTEM
`MisprintMixer.mixer` — Master → Music / SFX / UI; exposed params
MasterVolume / MusicVolume / SFXVolume / UIVolume. `AudioController` maps
0–1 sliders to dB and plays pooled one-shots via `SoundBank`. 14 procedurally
generated WAVs. Settings sliders verified driving mixer params in Play Mode.
Provenance: `Documentation/AUDIO_PROVENANCE.md` (all Project-generated
original, no external dependency).

## LEVEL PROGRESS
```
Scope: Training + 12 levels (6 batches x 2). All implemented, validated, QA'd.
Training       L00  done  (guided walkthrough, flow hand-tested)
Batch 1 Reg.Error   L01-02  done  (Replace, Rotate)        validated + QA
Batch 2 Mirror Plate L03-04 done  (Swap)                   validated + QA
Batch 3 Double Impr. L05-06 done  (Cross Out)              validated + QA
Batch 4 Reg. Shift   L07-08 done  (Shift, Swap)            validated + QA
Batch 5 Faded Ink    L09-10 done  (Restore)                validated + QA
Batch 6 Color/Final  L11-12 done  (striped + all 6 tools)  validated + QA
```

## KNOWN ISSUES
```
BUG-021  ToolButton selected-state covered the tool label.
         Status: VERIFIED FIXED (selection is now a teal keyline frame)
BUG-022  Tutorial bubble overlapped the Defect card.
         Status: VERIFIED FIXED (repositioned + own blocking CanvasGroup)
BUG-023  Gameplay proof board rendered slightly right of centre.
         Status: VERIFIED FIXED (board x -0.3, camera/anchor retune)
BUG-024  PlayMode flow test reports as 1 opaque test via MCP runner (runs +
         passes). EditMode suite (48) is the authoritative regression gate.
         Status: OPEN (cosmetic — tooling quirk, not a game bug)
BUG-025  Locked Job tickets show the paper "punch hole" square through the
         dark overlay. Minor. Status: OPEN (cosmetic)
NOTE     unity_screenshot_game intermittently fails to flush a PNG while the
         editor is unfocused; unity_graphics_game_capture (camera-only) works.
```

## NEXT EXACT ACTIONS  (pre-submission)
1. Package trim **DONE** — removed timeline / visualscripting /
   multiplayer.center / collab-proxy / ide.rider / 2D tooling / unused engine
   modules, then `com.anklebreaker.unity-mcp` last. Verified: 0 errors,
   48/48 tests, Windows build OK. Backups: `manifest.json.pre-trim.bak`,
   `manifest.json.pre-mcp-removal.bak`. After the MCP removal a normal Unity
   session no longer needs a re-add; if MCP is wanted again, restore the line.
2. On a Mac: switch platform to iOS, build the Xcode project, add signing,
   archive. Player Settings are already iOS-ready (IL2CPP, landscape, bundle
   id, icons). This is the one true external blocker on the current machine.
3. Strip the INTERNET permission from the generated `AndroidManifest.xml`
   (the game is fully offline).
4. On-device smoke test (touch targets, haptics, audio routing).
5. Store listing copy + screenshots from the release build.
6. Optional polish: BUG-025 (locked-ticket punch-hole), richer environment
   materials, a bespoke industrial display font (licensed).

## OLD NEXT ACTIONS  (superseded — was 42-level plan)
1. By-hand Play Mode pass of L03–L06 (correct + incorrect path each); tick
   LEVEL_QA. Fix BUG-021..023 while in there.
2. Chapter 2 "Mirror Plate" (L07–L12): add `BuildChapter2()` to
   `ChapterBuilders.cs` — MirrorDefect variants (all / alternating / every-Nth),
   multiple corrupted tokens, introduce **Swap**. Validate + `Misprint/Build/4`
   to refresh LevelDatabase. Fill LEVEL_DESIGN Chapter 2.
3. Wire Swap into `GameplayFlow.OnTokenClicked` (currently Rotate/Replace have
   popups; Swap needs a two-tap "select A then B" interaction). Add EditMode
   coverage for the Swap UI path if practical.
4. Chapter 3 "Double Impression" (L13–L18): DuplicateDefect, introduce
   **Cross Out** (direct-apply already stubbed in `BuildSimpleOp`). Verify
   strip re-render handles length changes (InstructionStripUI pools — check
   pooled views hidden correctly when strip shrinks).
5. Chapter 4 "Registration Shift" (L19–L24): ShiftDefect, introduce **Shift**.
6. Chapter 5 "Faded Ink" (L25–L30): FadeDefect, introduce **Restore** — needs
   a Restore picker popup (like Replace) and TokenView faded visual is already
   there. Ensure every faded token is logically inferable (unique solution).
7. Chapter 6 "Color Pass" (L31–L36): ink-pass modifiers via InkFlags
   (Marked/Outlined/Striped). Add rule handling to a new DefectRule +
   TokenView must show icon/border/pattern, NOT colour alone.
8. Chapter 7 "Final Proof" (L37–L42): CompositeDefect combos, tight budgets.
   L42 uses every tool.
9. Job Select: show Chapters 2–7 as polished locked batch tiles; multi-batch
   navigation. Chapter-complete transition.
10. Original 1024×1024 icon; frame-rate cap (Application.targetFrameRate);
    haptics device test; performance profile of a full session.
11. Full QA: 42-level LEVEL_QA, UI_QA re-pass, PACKAGE_AUDIT, MOBILE_RELEASE_
    CHECKLIST, performance, console, production build/export, FINAL_AUDIT.
    Only when FINAL_AUDIT is all-PASS → PROJECT STATUS = COMPLETE.

## HOW TO RUN TESTS
`unity_testing_run_tests { mode:"EditMode", assemblies:["MisprintDepartment.Tests.EditMode"] }`
then `unity_testing_get_job { jobId }`. Last run: **44/44 pass** (2026-09-03).
Full regenerate: menu `Misprint/Build/0 · REBUILD EVERYTHING` (materials, audio,
prefabs, icon, levels incl. Training, LevelDatabase, 3 scenes, player settings).
Validate all levels: menu `Misprint/Validate All Levels` (7 → Training + Ch1).

## ORIGINALITY CHECKPOINT (post-M2, master prompt §27)
- Reads as a print-quality-control puzzle? **Yes** — machine feeds a proof
  strip, defect card, correction tray, RUN PROOF lever, QC ratings.
- UI specifically authored for Misprint Department? **Yes** — no template;
  paper/ink/charcoal palette, job tickets, maintenance card, no three-star,
  no giant Play button, no neon.
- Resembles a previous project / generic route puzzle with a skin?
  **No** — the mechanic *is* correcting deterministic printing defects; the
  grid route is the read-out, not the game. Correction tools + defect rules
  are the core loop.
- Printer/workstation integrated into gameplay? **Yes** — proof board is the
  paper coming out of the machine; lever runs it; carriage is the print head.
- Ratings/terminology distinct? **Yes** — VERIFIED / CLEAN PROOF / MASTER
  PROOF; PROOF APPROVED / REPRINT REQUIRED; jobs, batches, defects.
- Screenshots recognisable as this game? **Yes.**
- Weak spots to strengthen in M3: environment is primitive massing (needs
  mechanical detail); only 2 of 6 correction tools exercised so far.
  Identity is sound — proceed to M3.

## MCP CHANGE LOG

### MCP-0026
- Date: 2026-09-08
- Agent: Claude (Sonnet 5), session "reviewer video + privacy policy"
- Scene/Asset: `Packages/manifest.json`, `Packages/packages-lock.json`,
  `Documentation/APP_REVIEW_COMPLIANCE.md`, `SUMMARY_BANK.md`.
- Change: Removed `com.anklebreaker.unity-mcp` again (user request, post iOS
  build). `unity_*` MCP tools are offline. manifest + lock now byte-identical
  to committed HEAD (the 32-dep set) — working tree clean, nothing to commit
  for `Packages/`. Backup: `manifest.json.pre-mcp-removal-2.bak`.
- Also corrected MCP-0025's wrong diagnosis: `PerformanceTestRunInfo.json` in
  the player is NOT MCP contamination. `com.unity.test-framework.performance`
  is a URP transitive (universal→core→collections→test-framework.performance)
  present in the committed lock; its `TestRunBuilder` is an
  `IPreprocessBuildWithReport` that runs on every build regardless of MCP.
  It is benign build metadata. Docs updated.
- Reason: user instruction ("remove ankle breaker mcp now").
- Validation: `com.anklebreaker.unity-mcp` gone from manifest, lock, and
  `Library/PackageCache`. No `error CS` in Editor.log. All
  `MisprintDepartment.*` assemblies intact. `git status` clean.
- Result: PASS

### MCP-0025
- Date: 2026-09-08
- Agent: Claude (Sonnet 5), session "reviewer video + privacy policy"
- Scene/Asset: `Assets/Scripts/Game/UI/WorkshopScreen.cs`, `WorkshopNav.cs`,
  `SettingsScreen.cs`, `Assets/Scripts/Editor/Build/WorkshopSceneBuilder.cs`,
  `Assets/Scenes/MainWorkshop.unity`, new
  `Assets/Scripts/Editor/Build/IosPostProcess.cs`, `ProjectSettings.asset`,
  UI/architecture docs.
- Change: (a) **Removed the Credits panel** and every button/field/event that
  fed it — the CREDITS side button on the bench, the CREDITS row in Settings,
  the CreditsLayer panel, `WorkshopScreen.Credits`, `SettingsScreen.Credits`,
  `WorkshopNav.credits`/`creditsBackButton`, `CreditsText()`. MainWorkshop
  scene regenerated via `Misprint/Build/Scene · MainWorkshop`; hierarchy
  confirmed Credits-free. (b) New `IosPostProcess` `[PostProcessBuild]`
  (`#if UNITY_IOS`): writes `ITSAppUsesNonExemptEncryption=false` into the
  built Info.plist and adds an app-level `PrivacyInfo.xcprivacy`
  (NSPrivacyTracking false, empty collected-data / tracking-domains / API
  arrays) to the Unity-iPhone target. (c) Generated the iOS Xcode project on
  Windows (`Build/iOS`, gitignored) and inspected it.
- Reason: user asked to remove Credits and to "generate xcode only and verify
  those files".
- Validation: 0 compile errors / warnings; EditMode 48/48;
  `Misprint/Validate All Levels` 13/13. iOS build `Succeeded` (0/0),
  post-process log confirms both plist edits. App Info.plist verified: bundle
  `com.misprintdepartmentteam.misprintdepartment`, v1.0/1, LandscapeRight
  only, `UIRequiresFullScreen`, no `NS*UsageDescription`, no
  `NSAppTransportSecurity`, no `CFBundleURLTypes`, no `UIBackgroundModes`, no
  `.entitlements`, `ITSAppUsesNonExemptEncryption=false`. Both privacy
  manifests present (engine `UnityFramework/PrivacyInfo.xcprivacy` +
  app-level).
- Note (cause corrected in MCP-0026): the player's `resources.assets` carries
  a `PerformanceTestRunInfo.json` TextAsset. Source is
  `com.unity.test-framework.performance` — a **URP transitive**
  (universal→core→collections→test-framework.performance), already in the
  committed manifest set, **NOT** the MCP bridge. Its
  `TestRunBuilder : IPreprocessBuildWithReport` writes it on every build, all
  platforms. Content = build metadata only (Unity version, package list,
  player-settings snapshot); no user data; Apple does not flag it. Strippable
  with a higher-callbackOrder `IPreprocessBuildWithReport` if a spotless build
  is wanted.
- Result: PASS (Credits removed; Xcode project generates + inspects clean;
  compiling the binary is still macOS-only)

### MCP-0024
- Date: 2026-09-08
- Agent: Claude (Sonnet 5), session "reviewer video + privacy policy"
- Scene/Asset: `ProjectSettings/UnityConnectSettings.asset`,
  `Documentation/APP_REVIEW_COMPLIANCE.md` (new), `FINAL_AUDIT.md`,
  `README.md`.
- Change: App Review 5.6 (Developer Code of Conduct) + 5.1.1 (Privacy)
  verification pass. (a) Disabled the Unity services layer entirely —
  `UnityConnectSettings.m_Enabled` 1→0 and `InsightsSettings.
  m_EngineDiagnosticsEnabled` 1→0 (all individual services were already off).
  (b) Fetched the live privacy policy and confirmed its text matches the
  binary (collects nothing, no network, only `Handheld.Vibrate`). (c) Wrote
  `APP_REVIEW_COMPLIANCE.md`: 5.6 table (no dead UI, no review manipulation,
  no hidden features, accurate metadata), 5.1.1 table (policy live/accurate,
  Data Not Collected, no permission strings, privacy-manifest note), export-
  compliance (`ITSAppUsesNonExemptEncryption = NO`), and the macOS/Xcode
  build-and-Validate steps.
- Reason: user asked to generate + verify the Xcode build for 5.6 / privacy.
- Validation: code scan clean (no network/tracking/permission APIs);
  `com.unity.services.*` absent; iOS usage-description strings all empty;
  privacy policy page returns real public content. Xcode build itself is
  still the macOS-only external blocker.
- Result: PASS (Xcode binary blocked on toolchain, not on any code issue)

### MCP-0023
- Date: 2026-09-08
- Agent: Claude (Sonnet 5), session "reviewer video + privacy policy"
- Scene/Asset: `Packages/manifest.json`, `Packages/packages-lock.json`,
  deleted `Assets/Scripts/Editor/Recording/`.
- Change: Removed the two packages the user had re-added for the reviewer
  video once it was captured — `com.unity.recorder` (and its transitive
  `com.unity.timeline`, auto-dropped from the lock) and, as the final step,
  `com.anklebreaker.unity-mcp`. Deleted the `MisprintDepartment.Editor.Recording`
  asmdef + `ReviewerDemoRecorder.cs` first so nothing referenced
  `Unity.Recorder.Editor`. Backups: `manifest.json.pre-recorder-removal.bak`
  (plus the older `.pre-trim.bak` / `.pre-mcp-removal.bak`).
- Reverify: manifest is back to the exact 32-dependency set verified clean in
  MCP-0021 — nothing else to trim. `com.unity.modules.director` stays (base
  Playables module pulled by animation/UGUI, tiny, not Timeline).
- Reason: user instruction ("remove both packages now recorder and ankle
  breaker mcp then commit and push and reverify any unnecessary package").
- Validation (before the MCP removal): 0 compile errors / 0 warnings,
  EditMode 48/48, `Misprint/Validate All Levels` → 13/13 PASS. No player
  build re-run: recorder + timeline are editor-only and cannot affect it;
  MCP-0021 already verified this 32-dep set builds (Windows + Android).
  MCP removal: packages-lock no longer lists `anklebreaker`; `unity_*` MCP
  tools are offline again.
- Result: PASS

### MCP-0022
- Date: 2026-09-08
- Agent: Claude (Sonnet 5), session "reviewer video + privacy policy"
- Scene/Asset: `Assets/Scripts/Editor/Recording/` (new asmdef +
  `ReviewerDemoRecorder.cs`), `Documentation/APP_STORE_LISTING.md`,
  `README.md` intent unchanged.
- Change: (a) Privacy policy URL added to the listing —
  `https://sites.google.com/view/misprint-department/home` (supplied by user,
  live). (b) New dev-only editor tool `MisprintDepartment.Editor.Recording`:
  menu `Misprint/Recording/*` drives an unattended play-mode playthrough
  (Bootstrap → sound-settings tour → Training Job → one level per batch, each
  solved by its `referenceSolution` and run to PROOF APPROVED) while Unity
  Recorder captures the Game view at **2868x1320** (iPhone 17 landscape),
  H.264 MP4 + audio, 60fps, into `Recordings/`. User re-added
  `com.anklebreaker.unity-mcp` + `com.unity.recorder` for this.
  **Superseded by MCP-0023: the tool folder and both packages were removed
  once the video was captured.** The video itself
  (`Recordings/MisprintDepartment_ReviewerDemo_001.mp4`, gitignored) is kept.
- Reason: user asked for a reviewer demo video for App Store Connect and
  supplied the privacy policy link.
- Validation: 0 compile errors. Recorded run completed clean (no exceptions,
  no rejected reference-solution ops, all 8 proofs APPROVED).
  `Recordings/MisprintDepartment_ReviewerDemo_001.mp4` = 2868x1320, H.264
  (avc1) + AAC audio, 122.5 s, 30 MB. `Recordings/` is gitignored.
- Result: PASS

### MCP-0015
- Date: 2026-09-03
- Agent: Claude (Sonnet 5), session "S3 — 12 levels + all tools + builds"
- Scene/Asset: Assets/Scripts/Core/, Assets/Scripts/Editor/ChapterBuilders.cs,
  Assets/Content/Levels/**, LevelValidator, LevelDatabase, SaveService
- Change: Scope reset to **12 levels** (user request). Core — `RouteSimulator`
  skips `Striped` tokens / runs `Marked` twice, adds `StepTrace.skipped`;
  new `ColorPassDefect` (InkPass Striped/Marked); `CorrectionEngine.Restore`
  now clears striped ink (keeps command) as well as faded. `TotalLevels`
  42→12. `LevelValidator` range 1..12 / batches 1..6, keeps the id-0/chapter-0
  Training exception. `ChapterBuilders` rewritten: `BuildAllLevels()` authors
  Training + 12 levels in 6 batch folders (deletes old Chapter01..07), each
  batch = one defect family + one tool, L12 = composite + all six tools.
- Reason: user set the campaign to 12 levels and asked for the remaining
  tools + Color Pass mechanic.
- Validation: `Misprint/Validate All Levels` → 13/13 PASS. EditMode 48/48.
- Result: PASS

### MCP-0016
- Date: 2026-09-03
- Agent: Claude (Sonnet 5), session "S3 — 12 levels + all tools + builds"
- Scene/Asset: Game/UI/**, Game/Gameplay/GameplayFlow.cs, Gameplay scene,
  JobTicket / TokenView prefabs, EnvBuilder
- Change: Full UI flows for all six correction tools — Swap (two-tap),
  Cross Out (tap-apply, tool chains), Shift (← / → popup),
  Restore (faded → command popup, striped → tap-apply); tool stays selected
  after a correction until budget dry / tool switch. `TokenActionPopup`
  gained OpenShift / OpenRestore. `TokenView` shows striped ("//" badge +
  metal frame) / marked ("x2") — border+icon, not colour. `GameplayFlow`
  first-use hint toasts per new mechanic (persisted); batch sign-off beat on
  Result for even levels ("BATCH NN SIGNED OFF", "ALL BATCHES CLEARED" on
  L12). `BatchInfo` names the 6 batches. `JobSelectScreen` shows all 13
  tickets in a 4-column grid with batch labels. Environment — added drive
  gears, proof tray, paper guides, ink-cartridge detail, stamp block, bench
  registration marks. `Bootstrap` sets `targetFrameRate = 60`, vSync off.
- Reason: user request ("do the remaining things"); master prompt §6, 12-21,
  29, 41-44, 62.
- Validation: Play Mode — every tool flow exercised (L01-L12), batch beat
  shown on L12, Job Files renders all 13. Console 0/0 from game code.
  Scene stats: 602 GO, ~2.5k tris, 3 lights, 0 colliders.
- Result: PASS

### MCP-0020
- Date: 2026-09-03
- Agent: Claude (Sonnet 5), session "S3 — 12 levels + all tools + builds"
- Scene/Asset: ProjectSettings (via MisprintBuild.ConfigurePlayerSettings)
- Change: Per user request, company name → **"Misprint Department team"** and
  bundle identifier (iOS + Android) → **`com.misprintdepartmentteam.misprintdepartment`**
  (was `com.rehmanmughal.misprintdepartment`). Docs updated (PROJECT_CONTEXT,
  MOBILE_RELEASE_CHECKLIST, FINAL_AUDIT, CLAUDE_MASTER_PROMPT).
- Reason: user instruction.
- Validation: `PlayerSettings.GetApplicationIdentifier` confirms both targets;
  `companyName` confirmed. Note: `persistentDataPath` now
  `LocalLow/Misprint Department team/Misprint Department` — old test saves are
  orphaned (harmless, fresh save is created).
- Result: PASS

### MCP-0021
- Date: 2026-09-03
- Agent: Claude (Sonnet 5), session "S3 — 12 levels + all tools + builds"
- Scene/Asset: Packages/manifest.json
- Change: Package trim (user request). Removed `com.unity.timeline`,
  `com.unity.visualscripting`, `com.unity.multiplayer.center`,
  `com.unity.collab-proxy`, `com.unity.ide.rider`, the 2D tooling set
  (`2d.animation` / `2d.aseprite` / `2d.psdimporter` / `2d.spriteshape` /
  `2d.tilemap.extras` / `2d.tooling`), and unused engine modules
  (`unityanalytics`, `vectorgraphics`, `cloth`, `terrain`+`terrainphysics`,
  `vehicles`, `wind`, `vr`, `xr`). Kept `2d.sprite` + `2d.tilemap` (active
  `Renderer2D` path). Then removed **`com.anklebreaker.unity-mcp`** as the
  final action (editor bridge; unreferenced by game code).
  Backups: `manifest.json.pre-trim.bak`, `manifest.json.pre-mcp-removal.bak`.
- Reason: user instruction ("remove ankle breaker mcp ... and any other
  unnecessary package").
- Validation (non-MCP trim): 0 compile errors, 0 warnings, EditMode 48/48,
  12/12 level reference solutions approved by LevelJudge, StandaloneWindows64
  build SUCCESS (25 s / 102 MB), Play-Mode render unchanged.
  MCP removal: Editor.log shows one clean domain reload + recompile (1.1 s,
  no `error CS`), 99 package files deleted, all `MisprintDepartment.*`
  assemblies intact, bridge port 7890 closed. Manifest now 32 deps.
- Result: PASS

## REPOSITORY
Private GitHub repo: **github.com/mhnmughal/misprint-department** (branch
`main`). Unity `.gitignore` + `.gitattributes` + `README.md` at the root.
Commit or push only when asked.

## DOCUMENTATION LOCATION
2026-09-08: the docs folder was moved out of `Assets/` to the repo root:
**`Documentation/`** (was `Assets/Documentation/`). The Unity `.md.meta`
files were dropped since they are meaningless outside `Assets/`. All path
references in the root docs were updated. Unity no longer imports these as
TextAssets; nothing in code loaded them at runtime.

## APP STORE METADATA
A ready-to-paste App Store Connect kit (name / subtitle / keyword options
with character counts, a fresh US-English description with no em dashes,
category, age rating, privacy answers, guideline check) is in the Claude
artifact `9c25ae5b-adc7-4403-9b95-7d60862da87d` and in
`Documentation/APP_STORE_LISTING.md`. Recommended category:
**Puzzle** (primary) / **Strategy** (secondary).
- Privacy policy: **LIVE** at
  `https://sites.google.com/view/misprint-department/home` (2026-09-08).
- Reviewer demo video: recorded on 2026-09-08 —
  `Recordings/MisprintDepartment_ReviewerDemo_001.mp4` (2868x1320, H.264 +
  audio, 2:02; gitignored, delivered to the user). The one-shot recording
  tool that produced it was removed again in MCP-0023.
- Still outstanding from the developer: a Support URL, the screenshot set,
  and the price decision (no in-app purchase either way).

## NOTE FOR NEXT AGENT
The Unity MCP bridge (`com.anklebreaker.unity-mcp`) has been added and removed
several times (MCP-0021, re-added+removed for the reviewer video in
MCP-0022/0023, re-added+removed for the Credits removal + iOS build in
MCP-0025/0026). **As of MCP-0026 it is removed and the working tree is clean**
— `Packages/manifest.json` + `packages-lock.json` match committed HEAD (the
32-dependency set), which is the intended shipping set.
- `unity_*` MCP tools do NOT work. To restore: put
  `"com.anklebreaker.unity-mcp": "https://github.com/AnkleBreaker-Studio/unity-mcp-plugin.git"`
  back in `Packages/manifest.json` and let Unity resolve. Do NOT commit that.
- `com.unity.test-framework.performance` (a URP transitive, in the committed
  set) writes `PerformanceTestRunInfo.json` into every player build via an
  `IPreprocessBuildWithReport`. Benign build metadata, no user data, Apple
  does not flag it (see APP_REVIEW_COMPLIANCE.md §1). Optional to strip.
- All `Misprint/Build/*` + `Misprint/Validate All Levels` tooling is in
  `MisprintDepartment.Editor` and works with or without MCP.

### MCP-0019
- Date: 2026-09-03
- Agent: Claude (Sonnet 5), session "S3 — 12 levels + all tools + builds"
- Scene/Asset: Assets/Scripts/Game/App/Bootstrap.cs
- Change: `Bootstrap` splash-to-menu handoff moved from an `IEnumerator Start()`
  coroutine (intermittently stranded on the Bootstrap scene after a heavy
  domain reload) to an `Update`-driven unscaled timer with an idempotent
  `_handedOff` guard. Now transitions reliably every play.
- Reason: intermittent "stuck on Bootstrap" observed after the package
  revert / rebuild.
- Validation: 5 consecutive Play sessions → MainWorkshop every time;
  new-player flow Start → Training → approved → persisted. EditMode 48/48.
- Result: PASS

### MCP-0018
- Date: 2026-09-03
- Agent: Claude (Sonnet 5), session "S3 — 12 levels + all tools + builds"
- Scene/Asset: Packages/manifest.json
- Change: Attempted a headless trim of unused packages (timeline,
  visualscripting, multiplayer.center). Recompiled with 0 errors + all game
  assemblies loaded, but the MCP-observed `isCompiling` state did not settle,
  so the manifest was **reverted to the full known-good set**. Package trim
  moved to a manual pre-submission step (Editor Package Manager UI).
- Reason: PACKAGE_AUDIT follow-up.
- Validation: manifest restored + re-resolved; project back to known-good.
- Result: REVERTED (no net change)

### MCP-0017
- Date: 2026-09-03
- Agent: Claude (Sonnet 5), session "S3 — 12 levels + all tools + builds"
- Scene/Asset: ProjectSettings, Build/
- Change: Produced player builds via `unity_build`.
  - StandaloneWindows64 → `Build/Win64/MisprintDepartment.exe` — SUCCESS,
    0 errors, 0 warnings, ~70 s, 105 MB.
  - Android (IL2CPP / ARM64) → `Build/Android/MisprintDepartment.apk` —
    SUCCESS, ~37 MB. (MCP tool reported a poll timeout; the build itself
    completed — .apk on disk.)
  - iOS — not buildable on this Windows machine (needs macOS + Xcode).
- Reason: master prompt §55, 76 ("production build/export succeeds"),
  completion gate.
- Validation: both output binaries present on disk; build logs clean of
  game-code errors/warnings.
- Result: PASS (Win + Android); iOS = external blocker.

### MCP-0012
- Date: 2026-09-03
- Agent: Claude (Sonnet 5), session "S2.1 polish + tutorial"
- Scene/Asset: Assets/Art/Icon/, ProjectSettings (icons)
- Change: `IconBuilder` — draws the original 1024×1024 app icon procedurally
  (charcoal ground, cream command strip through two roller bars, three
  command cells, middle cell = red misprint with a cream correction-tape
  swipe + corrected up-arrow + teal QC tick). Saved `AppIcon.png`
  (uncompressed RGBA), assigned to `BuildTargetGroup.Unknown` (default) and
  every iOS + Android platform icon slot. Added to `Misprint/Build/0`.
- Reason: master prompt §54; S2 gate "icon integrated".
- Validation: rendered + visually checked at icon scale; PlayerSettings
  reports default icon set + 5/5 iOS slots textured.
- Result: PASS

### MCP-0013
- Date: 2026-09-03
- Agent: Claude (Sonnet 5), session "S2.1 polish + tutorial"
- Scene/Asset: Assets/Scripts/Editor/Build/ (UiKit, PrefabBuilder,
  *SceneBuilder), Assets/Scripts/Editor/Build/AudioBuilder.cs, all 3 scenes
- Change: UI polish — `UiKit.EdgeFrame` / `Keyline` / `RegistrationMark`
  helpers; keylines + registration marks on defect / result / dialog /
  instructions / job-ticket / token / chip; rotated double-ring QC stamp for
  ratings and ticket "PASS"; button bottom-lip; heading rules; character +
  line spacing; ToolButton & TokenView selection/highlight are now keyline
  frames (BUG-021); tutorial bubble repositioned with its own blocking
  CanvasGroup (BUG-022); gameplay board recentred + strip/defect-card
  re-anchored (BUG-023). SFX — rewrote synthesis (struck partials, one-pole
  low-pass, smooth ADSR, per-clip normalise + DC removal), retuned SoundBank
  volumes; 14 clips regenerated, peaks 0.34–0.82, no clipping.
- Reason: master prompt §20, 27, 30, 34, 39, 40, 42, 45, 49; user request.
- Validation: rebuild clean; Play Mode screenshots of workshop / training /
  result / job select confirm the new look; console 0 errors 0 warnings.
- Result: PASS

### MCP-0014
- Date: 2026-09-03
- Agent: Claude (Sonnet 5), session "S2.1 polish + tutorial"
- Scene/Asset: Assets/Content/Levels/Training/L00_Training_Job.asset,
  Assets/Content/LevelDatabase.asset, Game/UI/TutorialLayer.cs + flow, scenes
- Change: Added the **Training Job** — `ChapterBuilders.Training()` authors
  L00 (id 0 / chapter 0, 3×3, one mirrored turn, Replace, budget 2).
  `LevelValidator` exempts id 0 / chapter 0. `LevelDatabase.trainingLevel`
  field + `MisprintBuild` wires it and excludes it from `levels`; `Next(0)`
  and `ById(0)` route through it. Rewrote `TutorialLayer` as a 6-step
  script (info steps advance on "GOT IT", action steps on the action) with
  strip/tray/lever/defect/budget anchors + a continue button on the bubble.
  `GameplayFlow` starts the tutorial only for level 0 and marks
  `tutorialsSeen:"training"` on completion. `WorkshopScreen` shows
  "START TRAINING" / "NEXT: TRAINING JOB" until done; `JobSelectScreen`
  shows a replayable "TR" ticket first. Added `LevelAssetTests`
  training test.
- Reason: master prompt §10, 15, 29, 76 ("Tutorial polished"); user request.
- Validation: `Misprint/Validate All Levels` → 7/7 PASS. EditMode 44/44.
  Play Mode: fresh save → Start Training → 6 steps walked → Master Proof →
  training persisted → Next Job → Job 001 (tutorial does not re-run).
- Result: PASS


### MCP-0001
- Date: 2026-09-03
- Agent: Claude (Sonnet 5), session "foundation"
- Scene/Asset: n/a (connectivity verification)
- Change: Verified Unity MCP connection to instance "Misprint Department"
  port 7890. Created + inspected + deleted a temporary GameObject
  `__MCP_ConnectivityTest`.
- Reason: AI_RULES Rule 2 / master prompt Step 2.
- Validation: Create returned instanceId -63996; delete confirmed.
- Result: PASS

### MCP-0002
- Date: 2026-09-03
- Agent: Claude (Sonnet 5), session "foundation"
- Scene/Asset: repo root + Documentation/
- Change: Created project-memory documents (`CLAUDE_MASTER_PROMPT.md`,
  `AI_RULES.md`, `PROJECT_CONTEXT.md`, `SUMMARY_BANK.md`) and
  Documentation/ (ARCHITECTURE, GAME_DESIGN, LEVEL_DESIGN filled;
  LEVEL_QA, UI_UX_SYSTEM, UI_QA, AUDIO_PROVENANCE, OFFLINE_AUDIT,
  PACKAGE_AUDIT, MOBILE_RELEASE_CHECKLIST, FINAL_AUDIT stubbed).
- Reason: Master prompt sections 2-8, 66; project must carry its own memory.
- Validation: Files written and readable.
- Result: PASS

### MCP-0003
- Date: 2026-09-03
- Agent: Claude (Sonnet 5), session "foundation"
- Scene/Asset: Assets/Scripts/Core/, Assets/Tests/EditMode/
- Change: Implemented `MisprintDepartment.Core` assembly — deterministic
  gameplay engine, no UnityEngine gameplay deps beyond serialization:
  PrintCommand/Direction/command wheel, Cell, CommandToken (+InkFlags),
  CommandStrip, BoardConfig, RouteSimulator (+RouteResult/FailReason),
  DefectRule (Rotate/Mirror/Duplicate/Shift/Fade/Swap/Composite),
  CorrectionEngine (all 6 tools) + CorrectionOp, LevelDefinition
  ScriptableObject (CreateAssetMenu "Misprint/Level Definition"),
  LevelJudge (+ProofRating Verified/CleanProof/MasterProof). Added
  `MisprintDepartment.Tests.EditMode` assembly with 32 tests.
- Reason: Master prompt sections 13-14, 22-24, 30, 57; deterministic
  simulation is the foundation everything else builds on.
- Validation: `unity_testing_run_tests` EditMode — 32/32 passed, 0 failed
  (jobId 767080a181e1, 0.06s). No compilation errors/warnings from our code.
- Result: PASS

### MCP-0004
- Date: 2026-09-03
- Agent: Claude (Sonnet 5), session "foundation"
- Scene/Asset: Assets/Scripts/Game/
- Change: Added `MisprintDepartment.Game` assembly — `SaveData` +
  `SaveService` (versioned local JSON at persistentDataPath, corrupt/empty/
  future-version -> fresh default + .bak backup, hand-edit clamps,
  ResetProgress keeps settings) and `GameSession` static scene handoff.
  Added 9 EditMode save tests.
- Reason: Master prompt sections 36, 51, 58; persistence + scene flow.
- Validation: EditMode 41/41 (jobId 68c263266dba). No errors.
- Result: PASS

### MCP-0011
- Date: 2026-09-03
- Agent: Claude (Sonnet 5), session "S2 vertical slice"
- Scene/Asset: Documentation/
- Change: Filled OFFLINE_AUDIT (PASS — no runtime network / SDK / analytics),
  AUDIO_PROVENANCE (14 clips, all project-generated original + font licence),
  UI_UX_SYSTEM (canvas arch, screen list, palette, spacing), UI_QA (per-screen
  + responsive at 16:9 / 19.5:9 / 4:3 / 20:9 — all PASS, no off-screen
  clipping via RectTransform bounds checks), LEVEL_QA (Chapter 1). Recorded
  runtime-UI audit: only `new GameObject` in runtime code is the AudioController
  one-shot pool — no runtime UI construction anywhere.
- Reason: S2 completion-gate items 22, 24, 27; master prompt 47/52/60/61/70.
- Validation: grep clean; bounds checks pass; console clean.
- Result: PASS

### MCP-0010
- Date: 2026-09-03
- Agent: Claude (Sonnet 5), session "S2 vertical slice"
- Scene/Asset: Play Mode (Bootstrap → MainWorkshop → Gameplay)
- Change: End-to-end Play Mode verification of the Chapter 1 slice.
  Fixed: command glyphs restricted to in-font chars (missing-glyph warning
  gone); Bootstrap static guard → instance + RuntimeInitializeOnLoad reset;
  Services static reset; ToolButton serialized `tool` field; Gameplay camera
  /board/strip/defect-card layout re-tuned so the proof grid reads clearly
  in front of the machine.
- Reason: master prompt 28 ("run them / use them / verify").
- Validation: L01 played correct + incorrect paths; tutorial 4 steps
  action-gated + persisted; proof coroutine animates carriage; Proof Approved
  (MasterProof) + Reprint Required both shown with accurate reasons; Next Job
  / Correct Again / Replay flows; save JSON on disk shows unlocked=3, L1+L2
  completed rating 3. Console: 0 errors, 0 warnings. EditMode 43/43.
- Result: PASS

### MCP-0009
- Date: 2026-09-03
- Agent: Claude (Sonnet 5), session "S2 vertical slice"
- Scene/Asset: ProjectSettings, EditorBuildSettings
- Change: `MisprintBuild.ConfigurePlayerSettings/ConfigureBuildSettings` —
  product "Misprint Department", company "Misprint Department team", bundle
  `com.misprintdepartmentteam.misprintdepartment` (iOS+Android), landscape-only
  orientation, IL2CPP (iOS+Android), Android ARM64. Build Settings scene list
  = Bootstrap / MainWorkshop / Gameplay. Deleted SampleScene.
- Reason: master prompt 26, 55; S2 gate.
- Validation: menu ran clean; project_info reflects settings.
- Result: PASS

### MCP-0008
- Date: 2026-09-03
- Agent: Claude (Sonnet 5), session "S2 vertical slice"
- Scene/Asset: Assets/Audio/
- Change: `AudioBuilder` (menu `Misprint/Build/2`) — synthesised 14 original
  WAVs (procedural sine/noise/envelope, deterministic), created
  `MisprintMixer.mixer` via reflection into `UnityEditor.Audio.AudioMixerController`
  with Master→Music/SFX/UI groups + exposed MasterVolume/MusicVolume/
  SFXVolume/UIVolume, built `SoundBank.asset`. Imported TMP essential
  resources (LiberationSans SDF).
- Reason: master prompt 18, 45–49; S2 gate "audio foundation + mixer".
- Validation: `mixer.GetFloat("MusicVolume")` succeeds; SettingsScreen
  sliders move mixer params in Play Mode; 4 groups present.
- Result: PASS

### MCP-0007
- Date: 2026-09-03
- Agent: Claude (Sonnet 5), session "S2 vertical slice"
- Scene/Asset: Assets/Art/Materials/, Assets/Prefabs/, Assets/Scenes/
- Change: Editor build pipeline under `Assets/Scripts/Editor/Build/` — `UiKit`
  + `PanelKit` (authored-UI helpers), `ArtBuilder` (13 URP materials),
  `EnvBuilder` (camera rig, 3-point + lamp lighting, ambient, workstation
  massing), `PrefabBuilder` (TokenView/ToolButton/JobTicket/Chip/CellTile/
  BlockedTile/Carriage), `BootstrapSceneBuilder`, `WorkshopSceneBuilder`,
  `GameplaySceneBuilder`, `MisprintBuild` (master menu + LevelDatabase).
  `Misprint/Build/0 · REBUILD EVERYTHING` regenerates all art + audio +
  prefabs + levels + 3 scenes + settings from code. All component references
  wired via SerializedObject. Scenes authored, visible in Edit Mode.
- Reason: master prompt 3, 5, 25, 41–43; AI_RULE 6/7 (scene-authored UI + env).
- Validation: rebuild runs clean; scene hierarchies inspected; Play Mode OK.
- Result: PASS

### MCP-0006
- Date: 2026-09-03
- Agent: Claude (Sonnet 5), session "S2 vertical slice"
- Scene/Asset: Assets/Scripts/Core/, Assets/Scripts/Game/ (App, Audio, Gameplay, UI)
- Change: Core — added `StepTrace` list to `RouteResult` (deterministic
  per-token playback data) and `correctedStrip` + `ReasonHeadline` to
  `ProofOutcome`. Game — new runtime layer: `Services` locator, `Bootstrap`,
  `SceneNames`, `LevelDatabase` SO, `AudioController` + `SoundBank` + `Sfx`
  enum, `LevelController` (working strip, correction history, proof coroutine,
  events), `RouteView` (grid + carriage playback), `GameplayFlow`
  (orchestrator). UI behaviours (all bind to authored scene objects, none
  build UI): SafeAreaFitter, UIPanel, CommandVisuals, WorkshopScreen,
  WorkshopNav, JobSelectScreen, JobTicket, GameplayHUD, DefectCardUI,
  InstructionStripUI, TokenView, CorrectionTrayUI, ToolButton,
  TokenActionPopup, ProofControlsUI, ResultScreen, PauseScreen,
  SettingsScreen, ConfirmDialog, Toast, TutorialLayer.
  Game asmdef now references Unity.TextMeshPro + UnityEngine.UI.
- Reason: S2 objective — playable Chapter 1 flow, scene-authored UI rule.
- Validation: 0 compile errors; EditMode 43/43 (jobId 971aa75f6234).
- Result: PASS (scenes/prefabs/wiring still pending)

### MCP-0005
- Date: 2026-09-03
- Agent: Claude (Sonnet 5), session "foundation"
- Scene/Asset: Assets/Scripts/Editor/, Assets/Content/Levels/Chapter01/
- Change: Added `MisprintDepartment.Editor` assembly — `LevelValidator`
  (menu `Misprint/Validate All Levels`; checks id/chapter range + uniqueness,
  board validity, clean strip solves board, defect changes strip, reference
  solution within budget + allowed tools + approved by LevelJudge at
  MasterProof, par sane, printed strip is NOT already solved). `ChapterBuilders`
  authors LevelDefinition assets in code (menu `Misprint/Build Levels/...`).
  Built Chapter 1: 6 LevelDefinition assets (L01 First Proof ... L06 Chapter
  Sign-Off) — Registration Error, defects Mirror/Rotate/Composite, tools
  Replace + Rotate. Added 2 EditMode tests running the validator over all
  level assets.
- Reason: Master prompt sections 15, 22, 24; data-driven levels + validation.
- Validation: `Misprint/Validate All Levels` -> "ALL LEVELS PASS" (6 levels).
  EditMode 43/43 (jobId 95218cd313e7). No compile errors.
- Result: PASS
