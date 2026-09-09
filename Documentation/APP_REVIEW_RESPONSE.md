# APP REVIEW RESPONSE — Guideline 2.1, Information Needed

Apple's 2026-09-09 note is the **standard new-account information request**, not
a rejection for a defect. Nothing in the app is wrong. Reply in App Store
Connect with the text below and also paste the condensed version into
**App Review Information → Notes** so it carries to future submissions.

Almost every question is N/A because Misprint Department is a single-player,
fully offline puzzle game: no accounts, no in-app purchase, no user-generated
content, no external services.

---

## What you must still produce yourself

**A screen recording captured on a physical iPhone (iOS 26.6.1).** The
editor-recorded reviewer video in `Recordings/` does **not** satisfy this —
Apple requires real hardware, current OS, starting from launch.

1. Confirm the iPhone is on the latest public iOS (Settings → General →
   Software Update). 26.6.1 is fine if it is current.
2. Install the build via TestFlight — the build is already in App Store
   Connect, so add yourself as an internal tester and install from TestFlight.
3. On the device: Settings → Control Center → add **Screen Recording**.
4. Force-quit the app. Start the screen recording, wait 1 second, then launch
   Misprint Department from the home screen.
5. Do this in one take (about 2–3 minutes, no rush — pauses are fine):
   - let the splash play, land on the workshop screen
   - tap **START TRAINING** → play the **Training Job** end to end; it is
     guided, so follow each callout (take Replace, tap the wrong turn, pick
     the right one, pull RUN PROOF)
   - on the result screen tap **JOB FILES** (or the back control) to reach
     the workshop, then open **Job Files**
   - open **Job 001**, solve it: tap a correction tool, tap the misprinted
     command, apply the fix, pull the **RUN PROOF** lever, let the route run,
     show the **PROOF APPROVED** card and its rating
   - go back to the workshop, open **Settings**, drag the Music and Sound
     sliders, toggle **Haptics** on/off
   - (optional) tap a locked job in Job Files to show it does nothing without
     being unlocked — demonstrates the progression is real
6. Stop the recording (tap the red status bar → Stop). Trim the dead ends in
   Photos. Upload it in the App Store Connect reply **and** attach it under
   App Review Information for future submissions.

Also before resubmitting: run the build on each supported device once (2.1
bugs/crashes) and swap the App Store screenshots so the first ones are
**gameplay**, not the workshop menu (2.3.3 — see APP_STORE_LISTING.md §09;
do not upload `01_workshop`).

---

## Full reply — paste into the App Store Connect message

```
Thank you for the review. Misprint Department is a free, single-player,
fully offline logic puzzle game. It has no user accounts, no in-app
purchases, no advertising, no user-generated content, and makes no network
connections. Answers to each point:

1. SCREEN RECORDING
A screen recording captured on a physical iPhone running iOS 26.6.1 is
attached. It begins by launching the app and shows the typical flow: the
workshop screen, the guided Training Job played to completion, Job 001
solved using the correction tools and the Run Proof lever with its result
screen, and the Settings screen (music, sound, interface, haptics, reset
progress).
- Account registration / login / deletion: none. The app never creates an
  account, so account deletion does not apply.
- User-generated content: none. There is no content creation, sharing,
  reporting, or blocking anywhere in the app.
- Paid content or features: none. The app is free. All 12 levels plus the
  Training Job are included; there is no in-app purchase, subscription, or
  advertising.

2. PURPOSE AND TARGET AUDIENCE
Misprint Department is a 2.5D deduction puzzle. A mechanical command
printer prints a route as a strip of instructions (Move, Turn Left, Turn
Right, Wait, Reverse, Jump) that contains one known, systematic printing
defect for that job - for example two commands swapped, a command
duplicated, or a slot shifted. The player reads the defect, uses a limited
set of bench tools (Rotate, Replace, Swap, Cross Out, Shift, Restore) to
correct the misprinted strip, then pulls a lever to run the corrected
route and see whether it reaches the target. Each job is scored Verified /
Clean Proof / Master Proof by how efficiently it was solved.

The value it provides: short, self-contained logic puzzles with a retro
"print quality control" workshop theme. A few minutes per level, no
timers, no penalties, playable entirely offline.

Target audience: players who enjoy logic, deduction, and programming-style
puzzles, and anyone wanting a calm offline puzzle game. Rated 4+; no
violence, mature themes, gambling, chat, or objectionable content.

3. SETUP AND ACCESSING MAIN FEATURES
No setup, login, credentials, or sample files are required. Launch the app
and everything is available immediately.
- The app opens to the workshop screen. Tap START INSPECTION (or CONTINUE
  INSPECTION) to begin.
- A first launch gives the guided Training Job, which teaches the loop
  step by step.
- Job Files shows all 12 jobs; they unlock in order as each is completed.
  For review you may open any job directly from Job Files.
- In a job: the printed route strip is at the top, the current defect is
  on the DEFECT card, the tools are in the tray at the lower left. Tap a
  tool, then tap the command to fix. Pull the RUN PROOF lever to execute.
- Settings (music, sound effects, interface volume, haptics, reset
  progress) is reached from the workshop screen.
- Progress saves automatically to one local file after each job.

4. EXTERNAL SERVICES, TOOLS, OR PLATFORMS
None. The app uses no external services of any kind: no backend or API, no
data providers, no authentication or account service, no payment
processor, no analytics, crash reporting, advertising, or attribution SDK,
no AI or machine-learning service, and no cloud storage or sync. The app
makes no network connections. It is built with the Unity engine; all game
content is bundled in the app. The only device capability used is optional
haptic feedback, which the player can turn off in Settings.

5. REGIONAL DIFFERENCES
There are none. The app behaves identically in every region. There is no
region-specific content and no geo-gating. Because it makes no network
calls and reads no location or locale data, its behavior does not vary by
country or device region.

6. REGULATED INDUSTRY / PROTECTED THIRD-PARTY MATERIAL
Not applicable. Misprint Department is an original game and does not
operate in any regulated industry. All code, artwork, level design, and
audio are original work by the developer. The only third-party asset is
the typeface Liberation Sans, used under the SIL Open Font License, which
permits commercial use. No licenses, credentials, or authorizations are
required.

The app is intended for the general public on the App Store; it is not an
enterprise, organization, or employee-only app. It is offered free of
charge with no monetization of any kind - no in-app purchases, no ads, no
subscriptions.

A demo account is not needed - the app has no login.
```

---

## Condensed version — paste into App Review Information → Notes

```
Misprint Department is a single-player, fully OFFLINE logic puzzle game.

- Free. No in-app purchases, no ads, no subscriptions, no monetization.
  All 12 levels + the Training Job are in the app.
- No accounts, no login, no account deletion (no account is ever created).
- No user-generated content, sharing, reporting, or blocking.
- No network connections. No external services: no backend/API, no
  analytics, ads, crash reporting, attribution, auth, payments, AI, or
  cloud. Built with Unity; all content is bundled.
- Only device feature used: optional haptics (toggle in Settings).
- Behaves identically in every region.
- Original content. Third-party asset: Liberation Sans font (SIL OFL).
- Consumer game for the general public, not an enterprise/internal app.

To review: launch the app, play the guided Training Job, then open Job
Files and play Job 001 (pick a correction tool, tap the wrong command,
pull the RUN PROOF lever). Settings (music/sound/haptics/reset) is on the
workshop screen. Any job can be opened directly from Job Files. No demo
account needed.
```

---

## Status of each "Prevent Common Issues" bullet

| Apple bullet | This app |
|---|---|
| 2.1 bugs/crashes — test on physical devices | Do a launch-to-finish pass on each supported device before resubmit. Editor + Windows + Android player builds are clean; on-device pass is the developer's. |
| 2.1 accessing the app — demo credentials | None needed; no login. Stated in the reply. |
| 2.3.3 screenshots — show the app in use | Swap to gameplay-first; drop `01_workshop` (menu). See APP_STORE_LISTING.md §09. |
| 3.1.1 in-app purchase | None. App is free ($0.00), no subscription. |
| 3.2 other business models — B2B/internal apps | N/A. Free consumer game, no monetization, public distribution. Stated in the reply. |
