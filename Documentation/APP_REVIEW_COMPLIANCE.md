# APP REVIEW COMPLIANCE — Misprint Department

Date: 2026-09-08 · Focus: **Guideline 5.6 (Developer Code of Conduct)** and
**Guideline 5.1.1 (Privacy — data collection, storage, and the privacy
policy)**. Verified against the actual project, not just intent.

---

## 1. The Xcode build itself

**Cannot be produced or verified on this machine.** The project lives on
Windows; an Xcode build, code signing, `xcarchive`/`ipa` export, Xcode
Organizer "Validate App", and the App Store Connect upload checks are all
macOS + Xcode only. This is a toolchain limit, not a project defect — the
Android IL2CPP / ARM64 player build succeeds, which is strong evidence the
iOS build will too once on a Mac.

Player Settings are already iOS-ready: IL2CPP, landscape only, bundle id
`com.misprintdepartmentteam.misprintdepartment`, version 1.0 / build 1,
deployment target iOS 15.0, original 1024² icon assigned, no ARKit, no
background modes, no custom URL schemes.

### To build + verify on a Mac

1. Copy the repo to a Mac with Unity 6000.3.19f1 (same version) + iOS Build
   Support, and Xcode installed.
2. `Misprint / Build / 0 · REBUILD EVERYTHING` (regenerates assets + scenes +
   player settings), then `File > Build Settings > iOS > Build` to emit the
   Xcode project.
3. Open `Unity-iPhone.xcodeproj`, set the Team / signing, set
   `ITSAppUsesNonExemptEncryption = NO` (see §4), Product > Archive.
4. Xcode Organizer > **Validate App** — this is the "verify" step. It runs
   Apple's static checks (Info.plist, entitlements, icon set, bitcode/arch,
   privacy manifest). Fix anything it flags, then Distribute.
5. Alternative to owning a Mac: Xcode Cloud, or a hosted Mac
   (MacStadium / Scaleway / AWS EC2 mac). Unity Build Automation can also
   produce the archive.

No shortcut avoids the Mac. Everything below is what *can* be checked now,
and it all passes.

---

## 2. Guideline 5.1.1 — Privacy

### 2.1 Privacy policy (5.1.1(i))

| Requirement | Status |
|---|---|
| Privacy policy reachable at a public URL | **PASS** — `https://sites.google.com/view/misprint-department/home` loads as a public page (no 404, no sign-in wall). Fetched and read 2026-09-08. |
| Names the app | PASS — "Misprint Department" stated explicitly. |
| States what data is / is not collected | PASS — "does not collect, transmit, or share any personal data. The app makes no network connections." |
| Covers third-party data sharing | PASS — "no advertising, no analytics, and no third-party software development kits." |
| Describes local storage | PASS — "game progress and settings are saved in a single file on your own device. That file is never uploaded anywhere." |
| Retention / deletion | PASS — deleting the app removes the data. |
| Contact point | PASS — `aclyvanblar@icloud.com`. |
| Last-updated date | PASS — 08-09-2026. |
| Link entered in App Store Connect **and** reachable in-app | **TO DO** — paste the URL into ASC App Privacy → Privacy Policy URL. In-app: the policy text is reproduced in `Documentation/APP_STORE_LISTING.md` §07; if Apple asks for an in-app link, add it to the Manual or Credits panel (both are authored screens). |

### 2.2 The disclosure matches the binary's actual behavior

Apple rejects apps whose privacy answers don't match runtime behavior. Code
scan (`Assets/Scripts/`, all `.cs`):

| Looked for | Found |
|---|---|
| `UnityWebRequest`, `HttpClient`, `System.Net`, `Socket`, `TcpClient` | **none** |
| `Application.OpenURL`, hard-coded `http(s)://` endpoints | **none** |
| Analytics / Firebase / AdMob / AppsFlyer / Advertisement / RemoteConfig | **none** |
| `SystemInfo.deviceUniqueIdentifier`, IDFA, `ATTrackingManager` | **none** |
| `Microphone`, `WebCamTexture`, `Input.location`, `LocationService` | **none** |
| Device access of any kind | only `Handheld.Vibrate()` (haptics), gated behind the user's Haptics toggle — no permission, no data |

`com.unity.services.*` packages: **not in the manifest.** Unity Analytics,
Ads, Crash/Cloud Diagnostics, Performance Reporting: all `m_Enabled: 0` in
`UnityConnectSettings.asset`, and on 2026-09-08 the top-level
`UnityConnectSettings.m_Enabled` and `InsightsSettings.m_EngineDiagnosticsEnabled`
were also set to `0` so nothing in the Unity services layer can initialize.

**App Store Connect "App Privacy" answer: Data Not Collected.** No data types,
no tracking, no ATT prompt.

### 2.3 Permissions / Info.plist

`cameraUsageDescription`, `locationUsageDescription`, `microphoneUsageDescription`,
`bluetoothUsageDescription` are all **empty** in Player Settings, so Unity
injects no `NS*UsageDescription` keys into Info.plist. The app shows **zero**
permission prompts. Nothing to justify to review under 5.1.1 / 5.1.2.

### 2.4 Privacy manifest (`PrivacyInfo.xcprivacy`)

Apple requires a privacy manifest for apps and for certain "required reason"
APIs. Misprint Department:
- collects no data → `NSPrivacyCollectedDataTypes` is empty.
- does no tracking → `NSPrivacyTracking = false`, `NSPrivacyTrackingDomains` empty.
- Required-reason APIs: Unity's engine may touch `UserDefaults` (reason
  `CA92.1`), file-timestamp, `systemBootTime`, and disk-space APIs. As of
  Unity 6000.3 the engine ships its own `PrivacyInfo.xcprivacy` inside
  `UnityFramework`; confirm it is present in the generated Xcode project
  (`Unity-iPhone/PrivacyInfo.xcprivacy` and/or inside the framework). If a
  first-party app-level manifest is missing at archive time, add one
  declaring no collection, no tracking, and the `CA92.1` UserDefaults reason.

**Verify at archive time on the Mac** — this is the one privacy item that
needs the generated project to eyeball.

---

## 3. Guideline 5.6 — Developer Code of Conduct

5.6 is about honest, respectful dealing — with users, with customers, and
with App Review. Checked against the build and the store metadata:

| 5.6 expectation | Status |
|---|---|
| Accurate metadata; app does what the listing says | **PASS** — description, subtitle, and keywords in `APP_STORE_LISTING.md` are written for this app and match the shipped loop (print → inspect → correct → run proof → verify). No exaggeration, no unshipped features. |
| No hidden, dormant, or undocumented features (also 2.3.1) | **PASS** — no debug menu or developer backdoor in the build. The only editor-only tooling (`Misprint/*` build menus, and the now-deleted reviewer-recording tool) is in Editor assemblies and never ships in the player. |
| No deceptive behavior / bait-and-switch / scam patterns | **PASS** — single self-contained paid-or-free game, no IAP, no subscriptions, no ads, no "unlock with review", no dark patterns. |
| Settings and buttons are real (no dead UI) | **PASS** — Settings sliders drive the AudioMixer and persist to the save; Haptics toggle is real; Reset Progress is real with a confirm dialog. Verified in `SettingsScreen.cs` and in the 2026-09-08 reviewer recording. |
| Ratings & reviews not manipulated (5.6.1) | **PASS** — the app contains no rating prompt, no `SKStoreReviewController`, no "rate us" nag, no incentivized-review flow. |
| Respect App Review; provide a working demo path | **PASS** — App Review notes (`APP_STORE_LISTING.md` §08) explain the app is fully offline with no account; a ~2-minute 2868×1320 reviewer walkthrough video is prepared (`Recordings/MisprintDepartment_ReviewerDemo_001.mp4`). |
| Don't misrepresent yourself / your entity | **TO CONFIRM** — the App Store Connect legal entity / D-U-N-S and the "Misprint Department team" seller name must be the developer's real, registered identity. Outside the repo's control; the developer confirms this in ASC. |
| No spam / duplicate apps / keyword squatting (also 4.3, 2.3.7) | **PASS** — one original title; keywords describe real mechanics; no competitor names, prices, or category words in the name/subtitle. |
| Safe for the stated age (4+) | **PASS** — no violence, profanity, gambling, UGC, or unrestricted web. |
| Charitable / payments conduct | N/A — no donations, no external payments, no crypto. |

**Nothing in the build or the prepared metadata conflicts with 5.6.** The two
"to confirm" rows are legal-identity facts only the developer can attest to in
App Store Connect; they are not code or metadata issues.

---

## 4. Export compliance / encryption (App Store Connect will ask)

The app uses no encryption beyond what Apple exempts (and in fact makes no
network calls at all). Set in the Xcode project before archiving:

```
ITSAppUsesNonExemptEncryption = NO   (Info.plist)
```

Then the ASC "Export Compliance" question is answered automatically as
exempt, with no annual self-classification report required.

---

## 5. Related non-blocking items

- **Android** (not a 5.6 / ASC concern): `ForceInternetPermission = 0` and no
  network code, so the generated `AndroidManifest.xml` should carry no
  `INTERNET` permission — confirm in the built manifest.
- **In-app privacy link**: if Apple insists on a tappable privacy link inside
  the app (they sometimes do for 5.1.1), add one line to the Manual or
  Credits panel pointing at the Google Sites URL. Low effort, authored-UI
  only.
- **Support URL**: still owed for the ASC listing (a page with a contact
  email — the privacy policy page's contact address can be reused).

---

## Verdict

- **5.6 Developer Code of Conduct:** no issues in the build or the prepared
  metadata. Two identity attestations are the developer's to make in ASC.
- **5.1.1 Privacy:** privacy policy is live, accurate, and consistent with a
  binary that collects nothing and makes no network connections. Remaining
  work is data entry in App Store Connect plus one archive-time check of the
  privacy manifest on the Mac.
- **Xcode build + Apple's automated verification:** blocked on macOS + Xcode.
  Steps in §1.
