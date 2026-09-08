# APP REVIEW COMPLIANCE — Misprint Department

Date: 2026-09-08 · Focus: **Guideline 5.6 (Developer Code of Conduct)** and
**Guideline 5.1.1 (Privacy — data collection, storage, and the privacy
policy)**. Verified against the actual project, not just intent.

---

## 1. The Xcode build itself

**The Xcode *project* is generated and was inspected here (2026-09-08).**
Unity on Windows can emit the `Build/iOS/` Xcode project (Unity-iPhone.xcodeproj
+ the IL2CPP-generated C++). What still needs a Mac: compiling that to an app,
code signing, `xcarchive`/`ipa` export, Xcode Organizer "Validate App", and
the App Store Connect upload checks. The Android IL2CPP / ARM64 player build
also succeeds, which is strong evidence the iOS compile will too once on a Mac.

### What the generated project looks like (verified)

| Item | Value |
|---|---|
| `CFBundleIdentifier` | `com.misprintdepartmentteam.misprintdepartment` |
| `CFBundleShortVersionString` / `CFBundleVersion` | `1.0` / `1` |
| `CFBundleDisplayName` | `Misprint Department` |
| Orientation | `UIInterfaceOrientationLandscapeRight` only; `UIRequiresFullScreen = true` |
| Deployment target | iOS 15.0; `TARGETED_DEVICE_FAMILY = 1,2` |
| `NS*UsageDescription` keys | **none** (no camera/mic/location/contacts/photos/Bluetooth/motion/tracking prompts) |
| `NSAppTransportSecurity`, `NSAllowsArbitraryLoads` | **not present** (no networking) |
| `CFBundleURLTypes`, `UIBackgroundModes` | **not present** |
| `.entitlements` file | **none** (no push, no associated domains, no capabilities) |
| `ITSAppUsesNonExemptEncryption` | `false` — set automatically by `IosPostProcess` (see §4) |
| Engine privacy manifest | `UnityFramework/PrivacyInfo.xcprivacy` — declares SystemBootTime, DiskSpace, UserDefaults (`CA92.1`), FileTimestamp required-reason APIs |
| App privacy manifest | `PrivacyInfo.xcprivacy` on the Unity-iPhone target — `NSPrivacyTracking = false`, empty tracking domains / collected data / API arrays. Added automatically by `IosPostProcess` |
| `ENABLE_BITCODE` | `NO` (correct — Apple deprecated bitcode) |
| `boot.config` | no analytics / Unity Connect entries |

### Caveat — regenerate the submission build with the MCP bridge removed

The `Build/iOS` folder generated here was built with `com.anklebreaker.unity-mcp`
in the manifest. That bridge pulls `com.unity.collections` →
`com.unity.test-framework.performance`, which drops a `PerformanceTestRunInfo.json`
TextAsset into `Assets/Resources/` at build time and compiles the
`Unity.PerformanceTesting` assembly into the player. Neither belongs in a
shipping app. **Before the real submission build, remove
`com.anklebreaker.unity-mcp` from `Packages/manifest.json`** (as in MCP-0023),
let the package manager re-resolve, then build — that removes both.

### To build + verify on a Mac

1. Copy the repo to a Mac with Unity 6000.3.19f1 + iOS Build Support and Xcode.
2. Remove `com.anklebreaker.unity-mcp` from `Packages/manifest.json` (see the
   caveat above), then `Misprint / Build / 0 · REBUILD EVERYTHING`, then
   `File > Build Settings > iOS > Build`.
3. Open `Unity-iPhone.xcodeproj`, set the Team / signing, Product > Archive.
   `ITSAppUsesNonExemptEncryption` and the app privacy manifest are already
   handled by `IosPostProcess` — no manual plist edit needed.
4. Xcode Organizer > **Validate App** — Apple's static checks (Info.plist,
   entitlements, icon set, arch, privacy manifest). Then Distribute.
5. No Mac? Xcode Cloud, a hosted Mac (MacStadium / Scaleway / AWS EC2 mac), or
   Unity Build Automation.

No shortcut avoids the Mac for the compile + Validate step. Everything else
below is verified and passes.

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

### 2.4 Privacy manifest (`PrivacyInfo.xcprivacy`) — verified

Two manifests are in the generated project:

1. **`UnityFramework/PrivacyInfo.xcprivacy`** (shipped by Unity) declares the
   engine's required-reason APIs: `SystemBootTime` (35F9.1), `DiskSpace`
   (E174.1), `UserDefaults` (CA92.1), `FileTimestamp` (0A2A.1 / C617.1). No
   collected data, no tracking.
2. **`PrivacyInfo.xcprivacy` on the Unity-iPhone (app) target** — added
   automatically by `Assets/Scripts/Editor/Build/IosPostProcess.cs`:
   `NSPrivacyTracking = false`, `NSPrivacyTrackingDomains` empty,
   `NSPrivacyCollectedDataTypes` empty, `NSPrivacyAccessedAPITypes` empty
   (the engine manifest covers the API reasons).

Both were confirmed present and correct in the 2026-09-08 generated build.

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

## 4. Export compliance / encryption — handled automatically

The app uses no encryption beyond what Apple exempts (and makes no network
calls at all). `IosPostProcess` writes `ITSAppUsesNonExemptEncryption = false`
into the built `Info.plist` on every iOS build, so the App Store Connect
"Export Compliance" question is answered automatically as exempt — no annual
self-classification report, no manual plist edit on the Mac. Verified present
in the 2026-09-08 build.

---

## 5. Related non-blocking items

- **Android** (not a 5.6 / ASC concern): `ForceInternetPermission = 0` and no
  network code, so the generated `AndroidManifest.xml` should carry no
  `INTERNET` permission — confirm in the built manifest.
- **In-app privacy link**: if Apple insists on a tappable privacy link inside
  the app (they sometimes do for 5.1.1), add one line to the Manual panel
  pointing at the Google Sites URL. Low effort, authored-UI only. (The
  Credits panel was removed 2026-09-08.)
- **Support URL**: still owed for the ASC listing (a page with a contact
  email — the privacy policy page's contact address can be reused).

---

## Verdict

- **5.6 Developer Code of Conduct:** no issues in the build or the prepared
  metadata. Two identity attestations are the developer's to make in ASC.
- **5.1.1 Privacy:** privacy policy is live, accurate, and consistent with a
  binary that collects nothing and makes no network connections. Both privacy
  manifests and the export-compliance flag are in the generated project.
  Remaining work is data entry in App Store Connect.
- **Xcode project:** generates on Windows and inspects clean (§1). Compiling
  the binary + Xcode "Validate App" is macOS-only. Regenerate the submission
  build with the MCP bridge removed.
