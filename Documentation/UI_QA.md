# UI_QA — Misprint Department

Session 2 — Chapter 1 vertical slice. Method: Play Mode navigation +
rendered screenshots + programmatic RectTransform bounds checks against
`Screen` at multiple aspect ratios.

## Screens
| Screen | Rendered & navigated | No dead buttons | No placeholder | Status |
|--------|:--:|:--:|:--:|--------|
| Splash (Bootstrap) | ✔ | n/a | ✔ | PASS |
| Main Workshop (MainInteractionLayer) | ✔ | ✔ Start/JobFiles/Manual/Settings all act | ✔ | PASS |
| Job Files (Batch 01 rack) | ✔ | ✔ tickets load job, lock/rating shown, Back works | ✔ | PASS |
| Gameplay HUD | ✔ | ✔ pause works | ✔ | PASS |
| Defect Rule Card | ✔ shows `DefectRule.Describe()` | n/a | ✔ | PASS |
| Instruction Strip | ✔ glyph+label, states | ✔ tokens open action popup | ✔ | PASS |
| Correction Tray | ✔ only allowed tools shown | ✔ tool select + Undo | ✔ | PASS |
| Token Action Popup | ✔ Rotate/Replace pickers | ✔ chips apply, close works | ✔ | PASS |
| Run Proof lever | ✔ animated throw | ✔ fires proof | ✔ | PASS |
| Proof Approved | ✔ stats + rating seal | ✔ Next/Replay/JobFiles | ✔ | PASS |
| Reprint Required | ✔ accurate reason | ✔ Correct Again/Restart | ✔ | PASS |
| Pause | ✔ (built, wired) | ✔ Resume/Restart/Instructions/Workshop | ✔ | PASS |
| Settings | ✔ | ✔ sliders drive mixer, toggle, Reset (confirm), Back | ✔ | PASS |
| Manual | ✔ authored copy | ✔ Back | ✔ | PASS |
| Confirm Dialog | ✔ | ✔ confirm/cancel | ✔ | PASS |
| Tutorial (Job 1) | ✔ 4 action-gated steps, persists | n/a | ✔ | PASS |
| Toast | ✔ | n/a | ✔ | PASS |

## Responsive (landscape)
RectTransform world-corner bounds checked against `Screen` — no element
off-screen at any ratio tested.

| Ratio | Resolution | Clipping / off-screen | Status |
|-------|-----------|:--:|--------|
| 16:9 (older phone) | 1920×1080 ref | none (screenshotted) | PASS |
| 19.5:9 (modern iPhone) | 2340×1080 | none | PASS |
| 4:3 (iPad) | 2732×2048 | none | PASS |
| 20:9 (wide phone) | 2400×1080 | none (custom size registered) | PASS |
| iPad Pro-like | 2732×2048 | none | PASS |

CanvasScaler: Scale With Screen Size, ref 1920×1080, match 0.5. `SafeArea`
child driven by `Screen.safeArea` on every screen canvas.

## Typography / alignment
- Font: LiberationSans SDF (SIL OFL). TMP essential resources imported.
- Command glyphs restricted to in-font characters (`↑ ← → ↓ • ↑↑`) — the
  earlier missing-glyph warning (`↰`/`↱`) is resolved; console clean.
- Labels centred in buttons via stretched child text with 6 px inset.
- Uppercase for labels/ratings; consistent.

## Correction-tool UI flows (Session 3 — all verified in Play Mode)
| Tool | Interaction | Verified on |
|------|-------------|-------------|
| Rotate | tap token → ← / → popup | L02 |
| Replace | tap token → command-chip popup | L01, L11 |
| Swap | tap token A → "tap the one to swap with" toast → tap token B | L03, L04, L08 |
| Cross Out | tap token → applied (tool stays selected, chains) | L05, L06, L12 |
| Shift | tap token → left / right popup | L07, L08 |
| Restore | faded → command-chip popup; striped → applied on tap | L09, L10, L11, L12 |

- Striped tokens show a metal keyline frame + a "//" corner badge; marked
  tokens a "x2" badge — pattern/border/icon, not colour alone.
- First-use of a new tool or defect shows a ~3 s hint toast, once (persisted).
- Job Files: 13 tickets (TR + 12) in a 4-column grid, lock / rating / batch
  labels; even-level completion shows a "BATCH NN SIGNED OFF" beat on Result.

## Fixed since first pass
- BUG-021 (selected-state covered the label) — FIXED, selection is a keyline frame.
- BUG-022 (tutorial bubble overlapped the defect card) — FIXED, repositioned + own group.
- BUG-023 (board off-centre) — FIXED, board x -0.3 + camera retune.

## Still open (non-blocking)
- BUG-024: PlayMode test runner enumeration via MCP is opaque (runs + passes).
- BUG-025: locked Job ticket shows its paper punch-hole through the overlay.
- `unity_screenshot_game` intermittently doesn't flush while the editor is
  unfocused — `unity_graphics_game_capture` (camera only) is the fallback.

## Console
Zero errors, zero warnings across the full Chapter 1 play session
(navigation, tutorial, corrections, popup, proof, result, settings, save).
