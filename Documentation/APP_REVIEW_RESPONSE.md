# APP REVIEW RESPONSE

## 2026-09-11: Guideline 2.1(a), Performance, App Completeness (bug)

A second rejection, on the resubmission: iPad Air 11" (M4), iPadOS 27.0
reported the Training Job's **"GOT IT" tutorial button unresponsive**
(screenshot showed Job 000, step-0 "Welcome" bubble still up, a REPLACE WITH
popup open, 1 correction already used). This is a real bug, **fixed** in
commit on 2026-09-12 (see `SUMMARY_BANK.md` MCP-0032 for the root cause: the
tutorial callout only blocked taps on its own small bubble, so a player
could skip past it into the game underneath and strand it on step 0).

This kind of message has no information request. Apple's own "Next Steps"
just says to fix it and resubmit. **No Resolution Center reply is required.**
Do this instead:

1. Build fresh from `main` (this commit is already newer than what shipped)
   on the Mac, validate, upload as a new build.
2. On a physical device, manually play the Training Job at least once,
   including tapping GOT IT with a tool already selected/a popup open, to
   confirm the fix before resubmitting, since this is exactly the class of
   bug Apple is now watching for on this app, so don't skip the manual pass.
3. Resubmit for review. If App Store Connect gives you a free-text box on
   the resubmission (not all bug-only rejections do), a one-line note like
   below is enough, no need for the long letter:

   > Fixed: the Training Job's GOT IT tutorial callout could be bypassed by
   > interacting with a tool/token underneath it, leaving it stuck and
   > appearing unresponsive. It now blocks the rest of the screen until
   > dismissed. Verified on a physical device.

---

## 2026-09-09: Guideline 2.1, Information Needed (first submission)

Apple's 2026-09-09 note is the **standard new-account information request**, not
a rejection for a defect. The app is complete; a device QA pass since the
original submission fixed one issue (the in-game Instructions panel Close
button) and tightened the menu panels, so **upload a fresh build from the
latest `main` and record that build** (see below).

Paste the reply below into the Resolution Center (~3,950 chars, under the
4,000 limit) and paste the condensed version into **App Review Information →
Notes** so it carries to future submissions.

---

## What you must still produce yourself

**A screen recording captured on a physical iPhone (iOS 26.6.2).** The
editor-recorded reviewer video in `Recordings/` does **not** satisfy this:
Apple requires real hardware, current OS, starting from launch.

1. Confirm the iPhone is on the latest public iOS (Settings → General →
   Software Update). 26.6.2 is fine if it is current.
2. Build a **fresh binary from the latest `main`** on the Mac (it has fixes
   the build in App Store Connect does not), validate it in Xcode, upload it,
   and install *that* build on the iPhone via TestFlight. Record this build.
3. On the device: Settings → Control Center → add **Screen Recording**.
4. Force-quit the app. Start the screen recording, wait 1 second, then launch
   Misprint Department from the home screen.
5. Do this in one take (about 2–3 minutes, no rush, pauses are fine):
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
     being unlocked (demonstrates the progression is real)
6. Stop the recording (tap the red status bar → Stop). Trim the dead ends in
   Photos. Upload it in the App Store Connect reply **and** attach it under
   App Review Information for future submissions.

Also before resubmitting: run the build on each supported device once (2.1
bugs/crashes) and swap the App Store screenshots so the first ones are
**gameplay**, not the workshop menu (2.3.3, see APP_STORE_LISTING.md §09;
do not upload `01_workshop`).

---

## Full reply: paste into the Resolution Center message

Letter format, ~3,970 characters (under the 4,000 Resolution Center limit).

```
Hello App Review Team,

Thank you for your message. The requested information is below.

1. PHYSICAL DEVICE SCREEN RECORDING

A screen recording captured on a physical iPhone (iOS 26.6.2), from
launch, showing the typical user flow:

- The main workshop screen
- Completing the guided Training Job
- Solving Job 001 and Job 002
- Selecting a correction tool and correcting a misprinted command
- Pulling the RUN PROOF lever, with the route reaching the destination
  (PROOF APPROVED)
- Opening Job Files, the Machine Manual, and Settings

The app has no account registration, login, account deletion,
user-generated content, paid content, subscriptions, or in-app purchases,
so those flows do not appear.

2. APP PURPOSE AND TARGET AUDIENCE

Misprint Department is a single player 2.5D logic puzzle game. A mechanical
printer prints a route as instruction tiles (Move, Turn Left, Turn Right,
Wait, Reverse, Jump). Each job the plate has one known printing defect
(for example tiles swapped or shifted). The player reads the defect,
corrects the tiles with a limited set of bench tools (Rotate, Replace,
Swap, Cross Out, Shift, Restore), then pulls a lever to run the corrected
route to the destination. Each job is scored Verified, Clean Proof, or
Master Proof.

It is a calm, deterministic puzzle with no time pressure or randomness.

Target audience: players who enjoy logic and puzzle games. Rated 4+, no
objectionable content.

3. SETUP AND ACCESSING THE MAIN FEATURES

No account, credentials, configuration, or sample files. Works offline
from first launch.

1. Launch the app.
2. Tap "START TRAINING" for the guided Training Job.
3. Tap a correction tool, then tap the misprinted command and choose the
   fix.
4. Tap the RUN PROOF lever to run the corrected route.
5. Reach the destination within the correction budget to complete the job.

The Training Job unlocks Job 001; each completed job unlocks the next.
There are 13 jobs total (Training Job plus 12). Any unlocked job can be
opened directly from Job Files.

Other menus: JOB FILES (job select), MACHINE MANUAL (command and tool
explanations), SETTINGS (music, sound, interface volume, haptics, reset
progress). No demo account is needed.

4. EXTERNAL SERVICES, TOOLS, OR PLATFORMS

None. The app has no backend, no external API or data provider, no
authentication service, no payment processor, no in-app purchases, no
advertising, no AI services, and no analytics, telemetry, attribution, or
crash-reporting SDK (Unity Analytics and diagnostics are disabled). It
makes no network connections and works fully offline.

Built with the Unity game engine; all levels, art, audio, and text are
bundled locally.

5. REGIONAL DIFFERENCES

None. Misprint Department functions consistently in every region. There
are no regional feature flags, no geographically varied content, no
region-specific functionality, and no location services.

6. REGULATED SERVICES OR PROTECTED THIRD-PARTY MATERIAL

Not applicable. It is a puzzle game and does not operate in a regulated
industry or provide regulated services.

The content is our original work: level designs, the route simulation,
the audio (synthesized in-code, stored as WAV), artwork, icon, and text.
There is no third-party music, characters, trademarks, or branded content.
The only third-party asset is the typeface Liberation Sans, under the SIL
Open Font License, which permits commercial use. No additional
documentation or authorization is required.

ADDITIONAL INFORMATION

The app is free, with no monetization and no hidden or reviewer-only
features. Between submissions, review found the Training Job's GOT IT
tutorial button unresponsive on iPad (it could be bypassed and left
stuck); that is fixed now, along with an earlier Close button issue on
the instructions screen. This build includes both fixes and was tested on
a physical iPhone running iOS 26.6.2.

Thank you for reviewing Misprint Department.

Best regards,
Misprint Department Team
```

---

## Condensed version: paste into App Review Information → Notes

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
| 2.1 bugs/crashes: test on physical devices | Two issues found and fixed by device passes: the Instructions panel Close button (commit `3a5125b`) and the Training Job GOT IT tutorial button (commit `e8b8bd8`). Editor tests + Windows + Android player builds are clean. Do a fresh launch-to-finish pass on the new build before resubmitting. |
| 2.1 accessing the app: demo credentials | None needed; no login. Stated in the reply. |
| 2.3.3 screenshots: show the app in use | Swap to gameplay-first; drop `01_workshop` (menu). See APP_STORE_LISTING.md §09. |
| 3.1.1 in-app purchase | None. App is free ($0.00), no subscription. |
| 3.2 other business models: B2B/internal apps | N/A. Free consumer game, no monetization, public distribution. Stated in the reply. |
