# GAME_DESIGN — Misprint Department

Status: DRAFT. Must stay synchronized with the implemented game (AI_RULES
Rule 14). Do not describe removed mechanics as current.

## Fantasy
You run the proof-inspection bench of an old print house. The command
printer has a mechanical fault. Every job, you know which fault the machine
has today; you must fix the misprinted route commands with the limited
correction tools on your bench, then pull the proof lever and watch the
route run.

## Board & route
- Rectangular grid (typ. 5x5 to 8x8), some cells blocked (machine parts).
- One start cell + facing, one destination cell.
- The route "carriage" starts at start/facing and executes the corrected
  command strip token by token.

### Commands
| Command | Effect |
|---------|--------|
| MOVE | advance 1 cell in facing direction |
| TURN LEFT | rotate facing 90 deg CCW (no move) |
| TURN RIGHT | rotate facing 90 deg CW (no move) |
| WAIT | consume one step, no change |
| REVERSE | flip facing 180 deg (no move) |
| JUMP | advance 2 cells in facing direction, ignoring the middle cell's block state but not the landing cell's |

### Fail reasons
RouteInvalid (ran off board / into block), TargetMissed (ended not on
destination), CorrectionLimitExceeded, InstructionError (illegal token after
corrections), ProofFailed (generic).

## Defect rules (the printer fault of the job)
Each level's `DefectRule` deterministically turns the clean strip into the
printed strip the player sees.
- **MirrorDefect** — listed command types are swapped with their mirror
  (TurnLeft<->TurnRight; optionally Move<->Reverse). Variants: all, every
  Nth, alternating.
- **DuplicateDefect** — a token is duplicated (every Nth, or the token after
  each WAIT, or tokens carrying a given ink marker).
- **ShiftDefect** — a token moves one slot right/left (every Nth, first after
  a marker, alternating drift).
- **FadeDefect** — a token is rendered unreadable (faded flag). Must remain
  logically inferable from route geometry + known rule.
- **RotateDefect** — a token is "rotated" to an adjacent command on a fixed
  wheel (Move->TurnRight->Reverse->TurnLeft->Move; Wait/Jump pass through or
  per-level).
- **SwapDefect** — two specific slots are swapped.
- **ColorPassDefect** — the ink pass flags a token: *striped* (the machine
  skips it) or *marked* (the machine runs it twice). Shown by a metal border
  + "//" badge / a "x2" badge — never colour alone.
- **Composite** — ordered application of the above (batch 6 / L12).

## Correction tools
| Tool | Action | Typical use |
|------|--------|-------------|
| ROTATE | step one token backward/forward on the command wheel | undo RotateDefect |
| REPLACE | set a token to an allowed replacement command | fix a clearly wrong token |
| SWAP | exchange two token slots | undo SwapDefect / ShiftDefect pairs |
| CROSS OUT | delete a token | undo DuplicateDefect |
| SHIFT | move a token one slot left/right (← / → popup) | undo ShiftDefect |
| RESTORE | lift obscuring ink — reveal a faded token to a chosen command, or clear a stripe | undo FadeDefect / ColorPassDefect(striped) |

Interaction: Rotate / Replace / Shift / Restore-on-faded open a small popup by
the tapped token; Cross Out and Restore-on-striped apply on the tap; Swap is
two-tap (first token, then the one to swap it with). A tool stays selected
after a correction so multi-corrections chain; it clears when the budget runs
out or the player picks another tool.

Each level lists allowed tools + a total correction budget. Par = designer's
intended count; optimal <= par earns MASTER PROOF.

## Ratings
- VERIFIED — job passed inspection.
- CLEAN PROOF — passed using <= par corrections.
- MASTER PROOF — passed using the minimum possible corrections.

## Campaign — 6 batches × 2 levels (12), plus the Training Job
The campaign was scoped to **12 levels**. See `LEVEL_DESIGN.md` for every
board / strip / defect / solution. Summary:
1. **Registration Error** (L01-02) — read a strip, spot the misprint; Replace, Rotate.
2. **Mirror Plate** (L03-04) — the plate swaps two command slots; Swap.
3. **Double Impression** (L05-06) — the plate prints a command twice; Cross Out.
4. **Registration Shift** (L07-08) — a command drifts between slots; Shift (+ Swap).
5. **Faded Ink** (L09-10) — deduce unreadable commands from geometry; Restore.
6. **Color Pass / Final Proof** (L11-12) — striped commands are skipped
   (Restore clears the stripe); L12 composites mirror + duplication + fade
   under an exact budget and offers all six tools — the mastery job.
An even-numbered level completing a batch shows a "BATCH NN SIGNED OFF" beat.

## Training Job
A dedicated guided level (`levelId 0`, `chapter 0`, asset
`Assets/Content/Levels/Training/L00_Training_Job.asset`) plays once before
Job 001. 3×3 board, one mirrored turn, Replace only, budget 2. A single
re-pointing callout ("TRAINING" bubble) runs a 6-step script:
1. read the printed strip · 2. read the DEFECT card · 3. take REPLACE ·
4. tap the wrong command and pick the fix · 5. note the correction budget ·
6. pull RUN PROOF.
Info steps advance on a "GOT IT" tap; action steps advance when the player
does the thing. Completion sets `tutorialsSeen: "training"` and flows into
Job 001. The Training Job also appears as a replayable "TR" ticket at the top
of the Job Files rack. It is **not** one of the 12 and does not affect
progression. `LevelDatabase.trainingLevel` holds it; `LevelValidator` treats
id 0 / chapter 0 as the training exception.

Beyond training, the first time a level offers a new tool or a new defect
family, a single ~3 s hint toast names it (keyed off the level's
`tutorialKeys`, persisted so it shows once).
