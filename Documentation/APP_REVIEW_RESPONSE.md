# APP REVIEW RESPONSE — Guideline 2.1, Information Needed

Apple's 2026-09-09 note is the **standard new-account information request**, not
a rejection for a defect. Nothing in the app is wrong. Paste the reply below
into the Resolution Center (it is ~2,600 chars, under the 4,000 limit) and
paste the condensed version into **App Review Information → Notes** so it
carries to future submissions.

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

## Full reply — paste into the Resolution Center message

Under 4000 characters. Facts only, no preamble.

```
Misprint Department is a free, single-player, offline logic puzzle game.
No accounts, no in-app purchase, no ads, no user-generated content, no
network connections.

1. SCREEN RECORDING
Attached: a recording from a physical iPhone on iOS 26.6.1, starting from
launch. It shows the workshop screen, the guided Training Job to
completion, Job 001 solved with the correction tools and the Run Proof
lever, and the Settings screen.
- Account registration / login / deletion: none. No account is created.
- User-generated content: none. No creation, sharing, reporting, blocking.
- Paid content or features: none. All content ships in the app.

2. PURPOSE AND AUDIENCE
A deduction puzzle. A printer outputs a route as instruction tiles (Move,
Turn Left, Turn Right, Wait, Reverse, Jump) with one known printing defect
per job. The player reads the defect, corrects the tiles with a limited
set of bench tools (Rotate, Replace, Swap, Cross Out, Shift, Restore),
then runs the corrected route to the target. Jobs are scored Verified /
Clean Proof / Master Proof.
Audience: logic and programming-puzzle players; anyone wanting a calm
offline puzzle game. Rated 4+.

3. SETUP AND ACCESS
No setup, login, credentials, or sample files. Launch and play.
- Opens to the workshop screen; tap Start.
- First launch gives the guided Training Job.
- Job Files lists all 12 jobs; they unlock in order. Any job can be opened
  directly from Job Files for review.
- In a job: tap a tool in the tray, tap the misprinted command, then pull
  the Run Proof lever.
- Settings (music, sound, haptics, reset progress) is on the workshop
  screen.
- Progress saves to one local file on the device.

4. EXTERNAL SERVICES
None. No backend or API, no data providers, no authentication, no payment
processor, no analytics, crash reporting, advertising, or attribution SDK,
no AI service, no cloud storage or sync. No network connections. Built
with the Unity engine; all content is bundled. The only device capability
used is optional haptics, toggleable in Settings.

5. REGIONAL DIFFERENCES
None. Identical behavior in every region. No region-specific content, no
geo-gating, no location or locale logic.

6. REGULATED INDUSTRY / PROTECTED MATERIAL
Not applicable. Original game, not in any regulated industry. All code,
art, level design, and audio are original. The only third-party asset is
the Liberation Sans typeface under the SIL Open Font License (commercial
use permitted). No licenses or authorizations required.

The app is free with no monetization and is for the general public, not an
enterprise or internal app. No demo account is needed.
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
