# ARCHITECTURE — Misprint Department

Status: DRAFT (updated as systems land). Authoritative for how the project
is wired.

## Scene architecture
- **Bootstrap** — first scene. Initializes `SaveController`, `AudioController`,
  applies saved settings, then loads `MainWorkshop`. Nothing gameplay here.
- **MainWorkshop** — QC-desk main menu. Start Inspection (opens Job Select
  overlay), Job Files (job-ticket rack = level select), Manual, Settings,
  Credits. Authored hierarchy + prefabs.
- **Gameplay** — single scene, data-driven. `LevelController` receives the
  selected `LevelDefinition` (via a static `GameSession` handoff), builds the
  board, prints the corrupted strip, wires the Correction Tray, runs the
  proof, shows result.

## Managers / state flow
`GameSession` (static) holds the pending level id + return context.
`SaveController` owns persisted state. `AudioController` owns mixer + clips.
`LevelController` owns the active puzzle. `RouteSimulator` is pure C#, owned
transiently by `LevelController` during Run Proof.

## Gameplay architecture (MisprintDepartment.Core — engine-light)
- `PrintCommand` — enum: Move, TurnLeft, TurnRight, Wait, Reverse, Jump.
- `CommandToken` — a command + flags (faded, marked, outlined, striped, ...).
- `CommandStrip` — ordered list of `CommandToken` (value type semantics via
  copy). Clean strip and working strip are both `CommandStrip`.
- `BoardConfig` — grid width/height, blocked cells, start cell+facing,
  destination cell, optional hazards.
- `RouteSimulator.Run(BoardConfig, CommandStrip) -> RouteResult` —
  deterministic step machine. `RouteResult` = { Success, FailReason, Path,
  FinalCell, FinalFacing, `trace` (per-token `StepTrace` for animated
  playback + QA, incl. `skipped` / `illegal`) }. Ink flags: a `Faded` token
  is an instruction error; a `Striped` token is skipped; a `Marked` token
  runs twice.
- `InkFlags` — `Faded`, `Striped`, `Marked` (Color Pass), on `CommandToken`.
- `DefectRule` (abstract) — `Apply(CommandStrip clean) -> CommandStrip
  printed`; never mutates the input. Concrete: `MirrorDefect`,
  `RotateDefect`, `DuplicateDefect`, `ShiftDefect`, `FadeDefect`,
  `SwapDefect`, `ColorPassDefect` (striped / marked), `CompositeDefect`
  (ordered list, `[SerializeReference]`).
- `CorrectionTool` — enum (Rotate, Replace, Swap, CrossOut, Shift, Restore).
  `CorrectionOp` records one applied correction. `CorrectionEngine.Apply`
  validates the tool is allowed + indices in range; Restore lifts `Faded`
  (player names the command) or `Striped` (keeps the command). Budget is
  enforced by the caller counting ops.
- `LevelJudge.Evaluate(LevelDefinition, ops) -> ProofOutcome` — applies the
  ops to the printed strip, runs `RouteSimulator`, returns approval +
  `ProofRating` + `correctedStrip` + headline. Deterministic.
- `LevelDefinition` (ScriptableObject) — `levelId`, `chapter` (= batch 1..6;
  0/0 = Training), title, `BoardConfig`, clean `CommandStrip` (as
  `SerializableToken`s), serialized `DefectRule`, allowed tools + budget,
  reference solution (list of `CorrectionOp`), par, `tutorialKeys`.
- `LevelDatabase` (ScriptableObject) — `trainingLevel` + ordered `levels`
  (the 12). Built by `MisprintBuild.BuildLevelDatabase`.

## Determinism contract
`RouteSimulator` and every `DefectRule`/`CorrectionOp` use no
`UnityEngine.Random`, no time, no floating-point accumulation that varies by
platform. Same inputs -> identical `RouteResult`. This is what the level
validator and EditMode tests rely on.

## Save system
`SaveController` -> `SaveData` v-tagged JSON at
`Application.persistentDataPath/misprint_save.json`. Fields: version,
unlockedChapter, unlockedLevel, per-level { completed, rating,
bestCorrections }, tutorialFlags, musicVol, sfxVol, uiVol, hapticsEnabled.
Corrupt/missing -> fresh default + backup of the bad file.

## Audio architecture
`AudioController` (Bootstrap, DontDestroyOnLoad). Exposes SetMusicVolume /
SetSfxVolume / SetUiVolume -> `AudioMixer.SetFloat` (dB, log curve).
One-shot SFX via pooled `AudioSource`s. Clips referenced by a
`SoundBank` ScriptableObject.

## UI architecture
Root Canvas per screen, `CanvasScaler` (Scale With Screen Size, ref
1920x1080, match 0.5). `SafeArea` component drives a RectTransform to
`Screen.safeArea`. Reusable widgets as prefabs under
`Assets/Prefabs/UI/`. Navigation via a lightweight `ScreenStack`.

## Assembly definitions
Core (no asmdef refs beyond Unity) <- Game <- UI. Editor refs Core+Game.
Tests ref Core (+Game for integration).
