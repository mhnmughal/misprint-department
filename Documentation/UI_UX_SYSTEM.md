# UI_UX_SYSTEM — Misprint Department

Status: Chapter 1 vertical slice complete. All production UI is authored in
scenes / prefabs by the editor builders under
`Assets/Scripts/Editor/Build/` (`UiKit`, `PanelKit`, `PrefabBuilder`,
`*SceneBuilder`). Runtime scripts only bind data and toggle state — they
never construct UI (verified, see FINAL_AUDIT / RUNTIME-UI audit).

## Canvas architecture
One `Canvas` per scene, Screen Space – Overlay, with:
- `CanvasScaler` — Scale With Screen Size, reference **1920×1080**, match 0.5.
- `GraphicRaycaster`.
- child `SafeArea` (anchored 0–1 stretch) driven by `SafeAreaFitter`
  (`Screen.safeArea`, re-applies on orientation/rect change). All screen
  content lives under SafeArea.
- `EventSystem` + `InputSystemUIInputModule` (project uses the Input System package).

## Screens / panels
| Panel | Scene | Script | Notes |
|-------|-------|--------|-------|
| Splash | Bootstrap | — | title + "PRINT QUALITY CONTROL" + registration bar |
| MainInteractionLayer | MainWorkshop | `WorkshopScreen` | Start/Continue proof-sheet control, current-job label, Job Files / Manual / Settings |
| JobFilesLayer | MainWorkshop | `JobSelectScreen` + `JobTicket` prefab | Batch 01 rack, 3×2 grid, lock / rating / complete state from save |
| SettingsLayer | MainWorkshop | `SettingsScreen` | 3 sliders + haptics toggle + Reset Progress (confirm) |
| ManualLayer | MainWorkshop | `UIPanel` | authored static copy |
| DialogLayer | both | `ConfirmDialog` | reusable yes/no |
| TopHUD | Gameplay | `GameplayHUD` | job id, title, correction budget, pause |
| DefectRuleCard | Gameplay | `DefectCardUI` | red edge; shows `DefectRule.Describe()` |
| InstructionStrip | Gameplay | `InstructionStripUI` + `TokenView` prefab | printed route; glyph + label; faded / marked / selected / proof-highlight states |
| CorrectionTray | Gameplay | `CorrectionTrayUI` + `ToolButton` prefab ×6 | only the level's allowed tools shown; Undo; selection state |
| TokenActionPopup | Gameplay | `TokenActionPopup` + `Chip` prefab | Rotate direction / Replace command picker, placed by tapped token |
| ProofControls | Gameplay | `ProofControlsUI` | mechanical lever, animated handle throw |
| ResultLayer | Gameplay | `ResultScreen` | PROOF APPROVED / REPRINT REQUIRED, stats, rating seal, contextual buttons |
| PauseLayer | Gameplay | `PauseScreen` | Resume / Restart / Instructions / Return to Workshop (Time.timeScale 0) |
| TutorialLayer | Gameplay | `TutorialLayer` | single re-pointing callout, 4 action-gated steps on Job 1, persisted `ch1.intro` |
| Toast | Gameplay | `Toast` | brief non-blocking messages |

## Navigation
- `WorkshopNav` owns MainWorkshop panel swaps and launches the Gameplay scene
  (`GameSession.PendingLevel` handoff).
- `GameplayFlow` wires every Gameplay UI element to `LevelController` events and
  owns job-to-job flow (Next / Replay / Restart / Correct Again / Return) and
  result persistence.

## Visual language / palette (`UiKit`)
Paper `#F2EAD8`, deep paper `#E4D8BE`, ink `#23201C`, charcoal `#2B2B2E`,
charcoal-up `#3A3A3E`, metal `#8A8F93`, warning red `#B53A2E`, teal (selected)
`#3F8E92`, lamp warm `#F3C97A`, panel ground `#302C27F0`. No neon.

## Spacing tokens
8 / 16 / 24 / 32 / 48 / 64 (used for padding, layout-group spacing, button gaps).

## Typography
TextMeshPro, `LiberationSans SDF` (SIL OFL). Headings Bold, uppercase for
labels/ratings. Command glyphs restricted to characters present in the font
(`↑ ← → ↓ • ↑↑`).

## Responsive / QA
See `UI_QA.md` for per-aspect-ratio results.
