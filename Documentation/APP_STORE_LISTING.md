# App Store Connect Listing Kit

Ready-to-paste metadata for **Misprint Department** on the Apple App Store.
US English. Every field checked against Apple's current field limits and the
App Review Guidelines (2.3, 4.2, 5.1.1). Written from scratch for this app, so
the metadata is unique and matches what the game actually does.

Interactive version (with per-field copy buttons):
<https://claude.ai/code/artifact/9c25ae5b-adc7-4403-9b95-7d60862da87d>

---

## 01. App name  (30 characters max)

Shown on the product page and in search. Must be unique across the App Store.
No prices, no "free", no category words, no other app names (Guideline 2.3.7).

| Option | Count | Note |
|--------|:-----:|------|
| `Misprint Department` | 19 / 30 | **Use this.** |
| `Misprint Department: Proof` | 25 / 30 | Fallback if the name is taken. |
| `Misprint Department QC` | 22 / 30 | Fallback. |

Keep the two words together so the brand reads first.

---

## 02. Subtitle  (30 characters max)

Sits under the name and **is indexed for search**, so it should carry a real
term someone would type. Do not repeat the app name. Same content rules as the
name.

| Option | Count |
|--------|:-----:|
| `Correct the printer, pass QC` | 28 / 30  **(recommended)** |
| `Printing-defect logic puzzles` | 29 / 30 |
| `Read the defect, fix the proof` | 30 / 30 |
| `Deduce, correct, run the proof` | 30 / 30 |
| `Correct the printer's mistakes` | 30 / 30 |
| ~~`Fix the misprint, run the proof`~~ | 31 — over |
| ~~`Spot the misprint, fix the route`~~ | 32 — over |

Pick one keyword theme for the subtitle, then choose the matching keyword set
in section 05 so you are not spending the same word twice.

---

## 03. Promotional text  (170 characters max)

Appears above the description and **can be changed any time without a new build
or review**. Good for a seasonal line later. Not indexed for search.

**Primary (157 / 170):**

```
The command printer keeps misprinting your route. You know the day's defect, so correct the strip and pull the proof lever. Twelve jobs. No ads, no internet.
```

**Alternate (168 / 170):**

```
A machine with a known fault. A strip of misprinted commands. Six correction tools, a limited budget. Fix the route, pull the lever, get it stamped. No ads, no internet.
```

---

## 04. Description  (4000 characters max)

The first two or three lines show before "more", so the hook is up top. No
pricing, no mention of other platforms (Guideline 2.3.10), no unverifiable
superlatives, no keyword lists. **No em dashes anywhere.** Roughly 1,800
characters, well inside the limit.

```
The command printer in the corner has a fault. Every job it runs, it prints your route instructions with the same systematic defect, and it is your bench that signs off the proof.

You know the fault before you start. It is written on the defect card. Turns come out mirrored. A command prints twice. The plate drifts an instruction one slot along. A command fades past reading. The ink pass skips a striped command. Read the printed strip, work out what the machine did to it, and put it right with the tools on the bench.

The loop is short. The printer runs the strip. You inspect it against the known defect. You correct it, with a limited number of corrections per job. You pull the proof lever and the route runs one command at a time along the board. If it reaches the destination, the proof is approved and stamped. If it does not, you get a reprint and another look.

Six correction tools, one introduced per batch: rotate a command around the wheel, replace it outright, swap two slots, cross out a stray repeat, shift a command along, restore faded or striped ink. By the final job all six are on the bench at once and the budget is exact.

Everything is deterministic. The same corrected strip always runs the same route, so a solved job is solved by reasoning, not by a lucky attempt. Where the ink pass marks a command, the marking is a border and an icon and a shape, never color on its own.

Contents: a short guided training job, then twelve handcrafted jobs across six batches. Every job carries a rating. Clear it for a Verified stamp, meet the correction par for a Clean Proof, find the shortest fix for a Master Proof.

It is a quiet, self-contained game. It runs entirely on your device. There is no internet requirement, no advertising, no analytics, no tracking, no account, and no in-app purchase. Turn on airplane mode and it plays exactly the same. Progress and settings are stored on the device.

Landscape only. Music, sound effects, and haptics each have their own control. All of the audio was generated for this game.
```

---

## 05. Keywords  (100 characters total)

One field, comma separated, **no spaces after commas** (a space costs a
character). Do not use "game", "free", "app", category names, other app names,
or plurals of a word you already used. Pick **one** set that fits your chosen
subtitle.

**Set A — broad reach (93 / 100):**

```
logic,deduction,brain,puzzles,offline,minimalist,factory,machine,robot,grid,sequence,strategy
```

**Set B — calm / casual lean (85 / 100):**

```
logic,brain,relax,cozy,casual,offline,minimal,think,printer,proof,route,pattern,solve
```

**Set C — mechanic forward (97 / 100):**

```
logic,deduction,programming,automation,factory,robot,offline,minimalist,indie,grid,sequence,proof
```

**Pairing.** Subtitle "Correct the printer, pass QC" goes with Set A or Set C.
Subtitle "Printing-defect logic puzzles" already spends *logic* and *puzzle*,
so drop those from the set you pick and add two of: *reasoning*, *tricky*,
*relaxing*, *lateral*.

---

## 06. Category

Primary category is **Games**. App Store Connect then asks for a primary game
subcategory and, optionally, a secondary one.

| Slot | Pick | Why |
|------|------|-----|
| **Primary subcategory** | **Puzzle** | It is a single-player logic and deduction puzzle. Clean fit, and where players of this style browse. |
| **Secondary subcategory** | **Strategy** | Planning the corrections within a fixed budget and reading the route ahead is the strategic layer. Broadest overlap with the same audience. |

If you prefer a different second choice:

- **Board** is defensible (the route runs on a grid board).
- **Family** is defensible (rated 4+, no text chat, no purchases, plays for all ages).
- Avoid **Educational**; Apple expects explicit teaching content there.

---

## 07. Age rating and app privacy

Answer the age questionnaire honestly; for this app every content question is
"None" or "No", which lands at **4+**.

| Field | Value |
|-------|-------|
| Age rating result | 4+ (no violence, no mature themes, no gambling or simulated gambling, no user-generated content, no unrestricted web access, no ads) |
| Data collection | **Data Not Collected.** No network calls, no analytics, no third-party SDKs. The only stored data is a local save file that never leaves the device. |
| Tracking | No. No App Tracking Transparency prompt, no IDFA use. |
| Account | No account, no sign-in, no Sign in with Apple requirement. |
| Privacy policy URL | `https://sites.google.com/view/misprint-department/home` (live). Hosts the text below. |

### Privacy policy — host this text at a public URL

```
Privacy Policy for Misprint Department

Misprint Department does not collect, transmit, or share any personal
data. The app makes no network connections. It contains no advertising,
no analytics, and no third-party software development kits.

Your game progress and settings are saved in a single file on your own
device. That file is never uploaded anywhere and is not accessible to us
or to any other party. Deleting the app removes it.

The app requests no special permissions and does not use the device
camera, microphone, contacts, location, photos, or Bluetooth.

Questions: [your support email]

Last updated: [date]
```

---

## 08. URLs, copyright, and review notes

| Field | Value |
|-------|-------|
| Support URL | Required. A simple page with a contact email is enough (GitHub Pages or Notion both qualify). |
| Marketing URL | Optional. Leave blank for v1 rather than pointing at a placeholder. |
| Copyright | `2026 Misprint Department team` |
| Primary language | English (U.S.) |
| Bundle ID | `com.misprintdepartmentteam.misprintdepartment` |
| Version / build | `1.0` / `1` |
| Content rights | No third-party content that needs additional rights. Code, art, and audio are original. The typeface is Liberation Sans under the SIL Open Font License, which permits commercial use. |

### App Review notes — paste into "Notes"

```
Misprint Department runs fully offline. There is no login, no account,
no server, and no special configuration.

To review: launch the app, play the guided Training Job, then Job 001.
All content unlocks from a single local save as jobs are completed.
Settings (music, sound, haptics, reset progress) are on the maintenance
card from the main workshop screen.
```

Demo account: not needed.

A screen-recorded playthrough for the reviewer is included:
`Recordings/MisprintDepartment_ReviewerDemo_001.mp4` (2868x1320 iPhone 17
landscape, H.264 with audio, about two minutes) covers the Training Job, one
level from every batch played to PROOF APPROVED, and the sound settings.
Attach it in the "App Review Information" notes or use it as the basis for an
app preview. (`Recordings/` is gitignored; the file lives only on the build
machine.)

---

## 09. Screenshots

Landscape. Actual gameplay only, no device frames with fake status bars, no
"download" or price text baked in (Guideline 2.3.3). Required set: iPhone
6.9-inch and, if the app ships on iPad, iPad 13-inch.

**A captured set is in `Screenshots/`** (2868 x 1320, PNG/RGB, iPhone 6.9-inch
landscape — the size ASC requires and auto-scales down; `Screenshots/` is
gitignored). Recommended upload order, strongest first:

1. `03_inspection.png` — a job mid-inspection: printed route strip, the
   TODAY'S DEFECT card, the correction tray, the RUN PROOF lever.
2. `05_proof_approved.png` — the result card with the rotated MASTER PROOF
   stamp, corrections-used vs par, and the batch sign-off line.
3. `02_job_files.png` — the job-ticket rack: 6 batches, PASS stamps and
   earned ratings.
4. `06_settings.png` — the maintenance card: music / sound / interface
   sliders, haptics, reset progress.
5. `04_proof_running.png` — the corrected route executing, a command
   highlighted, the carriage part way to the target.
6. `01_workshop.png` — the proof bench and the CONTINUE INSPECTION control.

To regenerate: re-add `com.unity.recorder`, restore the
`Assets/Scripts/Editor/Store/` tool from git history, and run
**Misprint / Store / Capture App Store Screenshots (2868x1320)**.

Polish notes before final upload: `01_workshop` shows the 3D bench as flat
primitives (low detail); on `02_job_files` the PASS stamp overlaps the small
batch-label line. Neither blocks submission; both are art/layout passes if the
listing needs to shine.

Optional caption text overlaid on the images is fine if it describes what the
screen does. Keep it to a few words per shot.

---

## 10. Guideline check

| Guideline | Status |
|-----------|--------|
| 2.3.1 hidden features | None. No debug menu, no dormant modes in the shipped build. |
| 2.3.7 name / subtitle / keywords | No competitor names, no prices, no category words, no keyword stuffing. All terms describe the actual game. |
| 2.3.10 other platforms | The description does not name or link any non-Apple platform. |
| 2.3 accurate metadata | Description, subtitle, and keywords are written from scratch for this app and match what it does. No exaggeration. |
| 4.2 minimum functionality | A guided training job plus twelve handcrafted levels with six mechanics and a rating system. Not a thin wrapper. |
| 5.1.1 privacy | Data Not Collected, plus a hosted privacy policy URL (`https://sites.google.com/view/misprint-department/home`). No permission prompts. |
| 3.1.1 payments | No in-app purchase. If the app is paid, the price is set in App Store Connect and never stated in the metadata. |
| 1.x content | 4+, no objectionable content. |

---

## Still to supply before submitting

- A **Support URL** (page with a contact email).
- The **price** (the app ships with no in-app purchase either way).

Done: privacy policy hosted at
`https://sites.google.com/view/misprint-department/home`; reviewer demo video
via the recording menu above.

*Confirm the final character counts in App Store Connect, which shows a live
counter as you type.*
