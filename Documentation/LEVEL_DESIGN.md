# LEVEL_DESIGN — Misprint Department

Campaign: **Training Job + 12 levels = 6 batches × 2**. Each batch introduces
one printer-defect family and one correction tool; batch 6 combines them.
All authored in `Assets/Scripts/Editor/ChapterBuilders.cs` (menu
`Misprint/Build Levels/Build All`), all pass `Misprint/Validate All Levels`
(13/13: routes solvable, defect changes the strip, reference solution within
budget + allowed tools + approved by `LevelJudge` at Master Proof, printed
strip not already solved).

`chapter` field = batch (1..6). `levelId` 0 = Training (not counted).
Slot numbers below are 1-based; code uses 0-based indices.

## Training Job (L00) — not one of the 12
Board 3×3, start (0,0) N, dest (2,2). Clean `Move Move TurnRight Move Move`.
Defect: Mirror (slot 3). Tool: Replace. Budget 2, par 1.
6-step guided walkthrough → flows into Job 001.

## Batch 1 — Registration Error  (tools: Replace, Rotate)
```
L01  First Proof         4×4  Mirror(slot 4)          Replace       B2 P1
     Move Move Move TurnRight Move Move  →  turn prints as its mirror.
     Solve: Replace #4 → TurnRight.
L02  Rotated Impression  5×5  Rotate(slot 4,+1)       Rotate,Replace B2 P1
     Move Move Move TurnRight Move Move Move  →  turn rotated on the wheel.
     Solve: Rotate #4 by -1.
```

## Batch 2 — Mirror Plate  (tool: Swap)
```
L03  Mirror Plate        5×5  Swap slots 3&4          Swap          B2 P1
     Move Move TurnLeft Move Move  →  two slots exchanged.
     Solve: Swap #3 ↔ #4.
L04  Alternating Plate   6×5  Swap slots 3&6          Swap,Replace  B2 P1
     Move Move TurnLeft Move Move TurnRight Move Move
     Solve: Swap #3 ↔ #6.
```

## Batch 3 — Double Impression  (tool: Cross Out)
```
L05  Double Impression   5×5  Duplicate every 3rd     Cross Out     B3 P2
     Move Move Move Move TurnRight Move Move  →  every 3rd command prints twice.
     Solve: Cross Out the two stray Moves.
L06  Ink Marker          5×5  Duplicate after WAIT    Cross Out     B2 P1
     Move Wait Move Move TurnLeft Move  →  the command after a WAIT doubles.
     Solve: Cross Out the extra Move.
```

## Batch 4 — Registration Shift  (tool: Shift)
```
L07  Registration Shift  6×5  Shift(every 5th,→)      Shift         B2 P1
     Move Move Move Move TurnLeft Move Move  →  a command drifts one slot right.
     Solve: Shift the misplaced turn back one slot left.
L08  Registration Drift  6×6  Shift(slot 3,→) + Swap(7&9)  Shift,Swap  B3 P2
     Move Move TurnLeft Move Move Move TurnRight Move Move
     Solve: Shift #4 left, Swap #7 ↔ #9.
```

## Batch 5 — Faded Ink  (tool: Restore)
```
L09  Faded Ink           5×5  Fade(slot 3)            Restore       B2 P1
     Move Move TurnRight Move Move  →  a command comes out unreadable.
     Deduce from geometry, then Restore #3 → TurnRight.
L10  Faded Registration  6×5  Fade(slots 3 & 6)       Restore       B3 P2
     Move Move TurnLeft Move Move TurnRight Move Move
     Solve: Restore #3 → TurnLeft, Restore #6 → TurnRight.
```

## Batch 6 — Color Pass / Final Proof  (all tools)
```
L11  Color Pass          5×5  Striped(slot 2) + Mirror(slot 4)   Restore,Replace  B3 P2
     Move Move Move TurnRight Move Move  →  a striped command is skipped by the
     ink pass (shown with a metal border + "//" badge, not colour alone) and
     one turn is mirrored.
     Solve: Restore #2 (clears the stripe), Replace #4 → TurnRight.
L12  Final Proof         5×4  Composite: Fade(slot 7) + Mirror(slot 3)
                              + Duplicate(every 4th)            ALL SIX  B5 P4
     Move Move TurnLeft Move Move Move TurnRight Move Move
     The mastery job — a mirrored turn, two duplicated Moves, and a faded turn,
     all in one strip, budget = exactly 4.
     Solve: Replace #3 → TurnLeft, Cross Out two duplicates, Restore the faded
     turn → TurnRight.  ("BATCH 06 SIGNED OFF — ALL BATCHES CLEARED")
```
