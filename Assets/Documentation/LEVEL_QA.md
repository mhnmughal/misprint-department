# LEVEL_QA — Misprint Department

Campaign scope: **Training Job + 12 levels (6 batches × 2)**.

## Automated coverage (authoritative regression gate)
- `Misprint/Validate All Levels` → **13/13 PASS**. Per level: unique id, batch
  fit, valid board, clean strip solves the board, defect changes the strip,
  reference solution within budget + only allowed tools + **approved by the
  same `LevelJudge` the game runs at runtime, at Master Proof**, par sane,
  and the uncorrected printed strip does **not** already pass.
- EditMode suite — **48 tests, all green**. Covers the deterministic
  `RouteSimulator` (all 6 commands, striped-skip, marked-twice, faded error,
  determinism), all 6 correction ops (incl. Restore on faded *and* striped),
  every defect rule, `LevelJudge`, `SaveService`, the 12-level structure,
  "every correction tool is used by some level", and the Training-Job flow.

## Manual Play Mode verification (Session 3)
Driven through the real `LevelController` + `GameplayFlow` UI (tool buttons,
token taps, action popups, Run Proof lever), not just the model:

| Level | Batch / tool | Verified |
|-------|--------------|----------|
| L00 Training | Replace + 6-step walkthrough | full flow, persists, → Job 001 |
| L01 First Proof | Replace | reference solution → Approved / Master Proof |
| L02 Rotated Impression | Rotate | reference solution → Approved / Master Proof |
| L03 Mirror Plate | **Swap** (two-tap) | tap-tap swap → clean strip → Approved |
| L04 Alternating Plate | Swap | reference solution → Approved / Master Proof |
| L05 Double Impression | **Cross Out** (chains) | 2× cross-out, tool stays selected → clean → Approved |
| L06 Ink Marker | Cross Out | reference solution → Approved / Master Proof |
| L07 Registration Shift | **Shift** (← / → popup) | shift-left → clean strip |
| L08 Registration Drift | Shift + Swap | reference solution → Approved / Master Proof; composite defect card shows both rules |
| L09 Faded Ink | **Restore** (command popup) | restore faded → TurnRight → clean strip |
| L10 Faded Registration | Restore ×2 | reference solution → Approved / Master Proof |
| L11 Color Pass | **Restore on striped** (direct) + Replace | clear stripe + fix mirror → clean strip; "//" badge + metal border shown |
| L12 Final Proof | all six tools | reference solution → Approved / **Master Proof**, "BATCH 06 SIGNED OFF — ALL BATCHES CLEARED" |

Also verified: Reprint Required with accurate reason on a wrong solution;
Correct Again keeps corrections; Replay / Restart / Next Job / Job Files;
Pause / Resume; first-use tool-hint toasts; batch sign-off beat on even
levels; Job Files shows all 13 tickets with lock / rating / batch labels;
progression + settings persist to disk across relaunch.

## Console
Zero errors. Zero warnings from game code (only Unity's own URP shader-library
`min16float` warnings and the deliberate corrupt-save test warnings).
