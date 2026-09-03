# CLAUDE_MASTER_PROMPT.md

> This file is the authoritative build brief for **Misprint Department**.
> Any AI agent (Claude, Codex, other) or human developer opening this repo
> must read this file, then `AI_RULES.md`, `PROJECT_CONTEXT.md`, and
> `SUMMARY_BANK.md` before making changes. The repository — not any chat
> history — is the project's memory.

---

## MASTER CLAUDE UNITY MCP DEVELOPMENT PROMPT — MISPRINT DEPARTMENT

### Complete 2.5D Offline Mobile Puzzle Game

You are the Lead Unity Game Developer, Technical Director, Game Designer,
UI/UX Director, Level Designer, Audio Designer, QA Engineer, and Release
Engineer responsible for building a complete production-quality game called
**MISPRINT DEPARTMENT**.

This is NOT a prototype, proof of concept, or a request for sample levels.
Build the complete game from scratch, polish it, test it, audit it, configure
it for mobile release, and continue until every completion gate passes.

Do not declare completion while unfinished work, placeholders, broken
navigation, incomplete levels, missing audio, unpolished UI, visual alignment
problems, build problems, or fixable bugs remain.

### 0. CORE DEVELOPMENT PRINCIPLE

The Unity project itself must contain its own memory. Every important
decision, change, system, scene, hierarchy structure, known issue, test
result and next step must be documented inside the project. Do not depend on
conversational memory.

### Game identity

- Game: Misprint Department
- Genre: 2.5D logic / route-correction puzzle
- Platforms: iOS primary, Android compatible
- Orientation: Landscape
- Connectivity: Completely offline
- Camera: Fixed slightly angled top-down 2.5D camera
- Visual identity: **retro industrial print quality control** — mechanical
  proof printer, rollers, gears, printed paper strips, proof sheets, ink
  cartridges, correction tape, inspection markers, QC stamps. NOT a postal
  game. No mailboxes / envelopes / mail imagery.

### Core loop

PRINT (machine produces route instructions) -> INSPECT (player studies the
current printer defect) -> CORRECT (player uses limited correction tools) ->
RUN PROOF (corrected route executes deterministically) -> VERIFY (pass, or
reprint required).

### Content

- 42 handcrafted levels, 7 chapters, 6 levels each.
- Ch1 Registration Error (1-6): basic execution, defect ID, Rotate, Replace.
- Ch2 Mirror Plate (7-12): mirrored commands, Swap.
- Ch3 Double Impression (13-18): duplicated instructions, Cross Out.
- Ch4 Registration Shift (19-24): commands shift between slots, Shift.
- Ch5 Faded Ink (25-30): unreadable instructions inferable by logic, Restore.
- Ch6 Color Pass (31-36): ink-pass rules; never color-only (pattern/icon/border/shape).
- Ch7 Final Proof (37-42): combine all systems. Level 42 = mastery challenge.

### Base print commands

MOVE, TURN LEFT, TURN RIGHT, WAIT, REVERSE, JUMP. Clear icons + text.
Route runner executes instructions deterministically.

### Correction tools

ROTATE, REPLACE, SWAP, CROSS OUT, SHIFT, RESTORE. Each needs clear visual
behavior, real gameplay effect, animation, sound, selection state, disabled
state, tutorial support.

### Architecture requirements

- Data-driven levels via ScriptableObject `LevelDefinition` (ID, chapter,
  title, board config, start, destination, original strip, defect rule,
  corrupted result, available tools, tool limits, solution, par, tutorial
  state, visual modifiers). No 42 hard-coded switch cases.
- Deterministic route simulation (same corrected sequence -> same result).
- Editor level-validation tool; all 42 levels must pass.
- Major production content authored in scenes/prefabs, inspectable in Edit
  Mode. No building the whole UI or environment at runtime.
- Scenes: Bootstrap, MainWorkshop, Gameplay (level data, not 42 scenes).

### UI/UX

Physical print-inspection workstation visual language. No generic mobile-game
UI, no three-star results. Screens: Main Workshop, Job Select (job tickets),
Gameplay HUD, Correction Tray, Defect Rule card, Tutorial, Pause, Proof
Approved (not "YOU WIN"), Reprint Required (not "GAME OVER"), Settings,
Manual, Credits, Dialogs. Ratings: VERIFIED / CLEAN PROOF / MASTER PROOF.
Run Proof = physical mechanical control (lever/switch), not a floating green
button. Responsive Canvas Scaler + safe-area handling. TextMeshPro,
licensed fonts, consistent spacing tokens (8/16/24/32/48/64).

### Environment / audio

2.5D authored workstation (workbench, printer chassis, paper feed, rollers,
guides, correction tape, ink mechanism, metal buttons, proof tray, inspection
lamp). Palette: warm off-white paper, charcoal machine body, muted metal,
dark ink, restrained warning red, subdued teal/blue selected states, warm
lamp highlights. Polished mobile-friendly lighting. Tactile animations.
**Original / license-safe audio only** — prefer project-generated WAVs.
Document every clip in `Assets/Documentation/AUDIO_PROVENANCE.md`. Audio
mixer groups: Master, Music, SFX, UI. Restrained haptics.

### Persistence

Local versioned save: unlocked chapters/jobs, completion, ratings, best
correction counts, tutorial progress, sound settings, haptic setting. Handle
corrupt/missing saves safely.

### Offline / SDK rules

No runtime network, ads, analytics, telemetry, tracking, remote config,
login, cloud, web views, multiplayer, online leaderboards, DLC. No
third-party SDKs (Firebase, AdMob, etc.). Self-contained.

### Mobile

Landscape, IL2CPP + ARM64 for iOS release, Android arch retained, original
1024x1024 icon (misprinted command strip through a printer roller, one wrong
instruction being corrected), product name "Misprint Department".
Bundle id **`com.misprintdepartmentteam.misprintdepartment`**, company
"Misprint Department team" (set by the user 2026-09-03; the brief's original
fallback was `com.rehmanmughal.misprintdepartment`).
No sensitive permissions (camera, mic, contacts, location, photos, BT, ATT).

### QA

Automated tests: all 6 base commands, all 6 corrections, defect
transformation, route simulation, win/fail conditions. Save tests. Level
validator. 42-level manual QA -> `Assets/Documentation/LEVEL_QA.md`. UI QA ->
`Assets/Documentation/UI_QA.md`. Performance profiling. Clean console.
Production build/export succeeds.

### Original rules (AI_RULES.md summary)

Read project memory first; Unity changes through MCP; log MCP modifications
to SUMMARY_BANK.md; never claim unverified success; no placeholders in final
game; complete UI + environment scene-authored; completely offline; no
third-party SDKs; original product, no reskin; preserve functionality; no
dead buttons; settings must be real; test after changes; mobile first;
document audio provenance; do not stop early.

### Completion

The project is COMPLETE only when the full checklist in section 76 of the
original brief passes and `Assets/Documentation/FINAL_AUDIT.md` reports PASS
in every category with no release-blocking bugs. Until then
`SUMMARY_BANK.md` PROJECT STATUS stays `IN_PROGRESS`. A playable first
level, a prototype, or 41 levels is NOT complete.

### Session handoff

Before any session ends: update `SUMMARY_BANK.md` (completed, tested, bugs,
current scene, next exact actions), keep status `IN_PROGRESS`. Next session:
read memory first, resume from recorded state, do not restart from zero.

---

*Full original brief text is preserved in the project's initial commit
history is unavailable (no git); this document is the canonical condensed
form. The complete gameplay/chapter/QA specifications above are binding.*
