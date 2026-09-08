# PROJECT_CONTEXT.md — Misprint Department

Permanent explanation of what this project is. Update when architecture
changes (AI_RULES.md Rule 14).

## Identity
- **Game:** Misprint Department
- **Genre:** 2.5D logic / route-correction puzzle
- **Platforms:** iOS primary, Android compatible
- **Orientation:** Landscape
- **Connectivity:** Completely offline
- **Camera:** Fixed, slightly angled top-down 2.5D
- **Unity:** 6000.3.19f1, URP (2D renderer present), Linear color space
- **Company:** Misprint Department team
- **Bundle id (target):** `com.misprintdepartmentteam.misprintdepartment`

## Visual identity
Retro industrial **print quality control** workstation. Mechanical proof
printer, rollers, subtle gears, printed paper strips, proof sheets, ink
cartridges, correction tape, inspection markers, metal switches, paper
guides, registration marks, QC stamps, mechanical counters, analog
indicators. It is a small industrial print inspection bench — **not** a
postal office. No mailboxes, envelopes, mail imagery, postal ledger/desk.

Palette: warm off-white paper, charcoal machine body, muted industrial
metal, dark ink, restrained warning red, subdued teal/blue for selected
states, small warm lamp highlights. No neon hypercasual colors.

## Core gameplay
A mechanical printing machine produces a route-instruction sequence
containing a **systematic printing defect**. The player knows the defect
rule and must correct the printed commands before executing the route.

Loop: **PRINT** (machine prints route instructions) -> **INSPECT** (study
the current defect) -> **CORRECT** (limited correction tools) -> **RUN
PROOF** (corrected route executes deterministically) -> **VERIFY** (pass =
job approved; fail = reprint required).

### Base commands
MOVE · TURN LEFT · TURN RIGHT · WAIT · REVERSE · JUMP — icon + text. The
route runner executes deterministically on a grid board.

### Correction tools
ROTATE (rotate a corrupted command) · REPLACE (swap in an allowed
replacement) · SWAP (exchange two positions) · CROSS OUT (remove an invalid
instruction) · SHIFT (move a command one slot) · RESTORE (recover a
faded/obscured instruction where allowed).

### Chapters
1. Registration Error (1-6) — execution, defect ID, Rotate, Replace
2. Mirror Plate (7-12) — mirrored commands, Swap
3. Double Impression (13-18) — duplicated instructions, Cross Out
4. Registration Shift (19-24) — commands shift between slots, Shift
5. Faded Ink (25-30) — unreadable instructions, Restore (logically inferable)
6. Color Pass (31-36) — ink-pass rules; pattern/icon/border/shape, not color-only
7. Final Proof (37-42) — combine all systems; L42 mastery challenge

## Ratings
VERIFIED (level complete) · CLEAN PROOF (met correction par) · MASTER PROOF
(optimal solution). Shown as print-inspection marks / seals. No three-star.

## Architecture (see Documentation/ARCHITECTURE.md for detail)
- **Scenes:** `Bootstrap` (init, load save, route to menu), `MainWorkshop`
  (menu = QC desk: Start Inspection, Job Files, Manual, Settings),
  `Gameplay` (one scene, data-driven by `LevelDefinition`).
- **Level data:** `LevelDefinition` ScriptableObject per level, 42 assets
  under `Assets/Content/Levels/ChapterNN/`.
- **Simulation:** deterministic `RouteSimulator` — pure C#, no engine
  randomness, unit-testable.
- **Defects:** `DefectRule` transforms the clean strip into the printed
  (corrupted) strip deterministically.
- **Corrections:** `CorrectionTool` operations mutate the working strip;
  budget-limited per level.
- **Save:** local versioned JSON via `SaveController` (Application.persistentDataPath).
- **Audio:** `AudioController` + `AudioMixer` (Master/Music/SFX/UI), original
  generated WAVs.
- **UI:** authored prefabs + scene hierarchy, TMP, safe-area, Canvas Scaler.

## C# layout
`Assets/Scripts/` with asmdefs:
- `MisprintDepartment.Core` — command language, RouteSimulator, DefectRule,
  CorrectionTool, board model (no UnityEngine gameplay deps beyond
  ScriptableObject/serialization).
- `MisprintDepartment.Game` — controllers, MonoBehaviours, scene glue.
- `MisprintDepartment.UI` — UI behaviours.
- `MisprintDepartment.Editor` — level validator, build tooling.
- `MisprintDepartment.Tests` — EditMode tests.

## Status
See `SUMMARY_BANK.md`. Currently `IN_PROGRESS` — foundation session.
