# AI_RULES.md — Misprint Department

Permanent rulebook for every AI agent (and human) working on this project.
Read this file first, every session.

## RULE 1 — READ PROJECT MEMORY FIRST
At the start of every session read, in order:
1. `AI_RULES.md`  2. `PROJECT_CONTEXT.md`  3. `SUMMARY_BANK.md`
4. `CLAUDE_MASTER_PROMPT.md`  5. relevant `Documentation/*`
No project modifications before you understand current state.

> **The `com.anklebreaker.unity-mcp` package is removed** from the project
> (pre-submission cleanup — removed 2026-09-03, briefly re-added for the
> reviewer video on 2026-09-08, removed again same day in MCP-0023). Rules 2–3
> below only apply if a future agent re-adds it (see SUMMARY_BANK.md "NOTE FOR
> NEXT AGENT"). Without it,
> make Unity changes in the Editor directly and still keep SUMMARY_BANK.md
> current. The build/level tooling under `MisprintDepartment.Editor`
> (`Misprint/*` menus) works with or without MCP.

## RULE 2 — UNITY CHANGES THROUGH MCP
Scene, prefab, component, hierarchy, Inspector and project-setting changes go
through the Unity MCP tooling (`unity_*` tools, port 7890) whenever supported.
No silent unrelated Unity changes outside that workflow.

## RULE 3 — LOG MCP MODIFICATIONS
Every meaningful Unity change is recorded in `SUMMARY_BANK.md` MCP CHANGE LOG
(hierarchy, prefab, component, scene, UI, environment, materials, lighting,
cameras, Player Settings, build settings, system integrations, level
additions, important fixes). Group related small actions into one entry. No
thousands of trivial lines.

## RULE 4 — NEVER CLAIM UNVERIFIED SUCCESS
Do not write "Fixed" / "Complete" / "Working" / "Passed" unless actually
tested.

## RULE 5 — NO PLACEHOLDERS IN FINAL GAME
Brief dev placeholders are allowed; none may remain. No TODO buttons, fake
screens, Lorem Ipsum, "Sample"/"Test"/"Temp"/"Coming Soon", dummy rewards,
fake settings, inaccessible features, unfinished icons.

## RULE 6 — COMPLETE UI MUST BE SCENE AUTHORED
Production UI exists as GameObjects in scenes/prefabs. Do not build the whole
UI at runtime with `new GameObject(...)` from Awake/Start. Reusable UI =
authored prefabs + scene hierarchy.

## RULE 7 — ENVIRONMENT MUST BE SCENE AUTHORED
Major environment composition visible in Edit Mode. Do not generate the whole
production environment only at Play.

## RULE 8 — GAME IS COMPLETELY OFFLINE
No runtime network requests, cloud, remote config, ads, analytics, tracking,
telemetry, auth, login, accounts, web views, multiplayer, online
leaderboards, DLC, remote APIs.

## RULE 9 — NO THIRD-PARTY SDKs
No Firebase, AdMob, Facebook SDK, GameAnalytics, AppsFlyer, Unity Ads,
external analytics, attribution/remote-config/social SDKs, unnecessary
plugins. Final game is self-contained.

## RULE 10 — ORIGINAL PRODUCT
Do not copy another game's UI, layouts, icon, sounds, code, artwork,
screenshots, or branding. No purchased/imported game template.

## RULE 11 — DO NOT RESKIN PREVIOUS PROJECTS
Misprint Department has its own architecture, navigation, hierarchy, UI,
visual identity, level data, icon, audio, menus, progression, effects.

## RULE 12 — PRESERVE FUNCTIONALITY
Before modifying a system: inspect it, understand it, identify dependencies,
make controlled changes, test afterward.

## RULE 13 — UPDATE SUMMARY BANK
After every meaningful milestone update `SUMMARY_BANK.md`.

## RULE 14 — DOCUMENT ARCHITECTURAL CHANGES
Architecture change -> update `PROJECT_CONTEXT.md`, `SUMMARY_BANK.md`, and
relevant docs.

## RULE 15 — NO DEAD BUTTONS
Every visible interactive control in the production build performs its
advertised action.

## RULE 16 — SETTINGS MUST BE REAL
Music slider changes music. Sound slider changes sounds. Haptics toggle
controls haptics. Reset Progress actually resets progress. All persist.

## RULE 17 — TEST AFTER CHANGES
Implementation without testing is incomplete.

## RULE 18 — MOBILE FIRST
Every UI/gameplay decision considers touch input, safe areas, mobile
performance, readable text, comfortable touch targets.

## RULE 19 — SOUND ASSET PROVENANCE
Never ship audio without documented legal origin. Prefer project-generated
original audio. Log every clip in `Documentation/AUDIO_PROVENANCE.md`.

## RULE 20 — DO NOT STOP EARLY
Not complete until the completion checklist is fully satisfied and
`FINAL_AUDIT.md` is all PASS. If a real external blocker (Apple cert, dev
account, unavailable OS permission) forces a stop, keep status
`IN_PROGRESS` and record continuation state. Compiler errors, broken refs,
missing UI, bad alignment, failing levels, missing audio, incomplete icon,
poor performance, code bugs, test failures, broken navigation, MCP setup
issues are NOT external blockers — fix them.

## Working conventions
- Unity instance: project "Misprint Department", port **7890**. Pass
  `port: 7890` on every `unity_*` call.
- Do NOT `git init` or push to any remote (not authorized; no repo exists).
- Scratchpad for throwaway files:
  `C:\Users\mhn13\AppData\Local\Temp\claude\...\scratchpad`
- Namespace all C#: `MisprintDepartment.*`. Asmdefs per layer.
