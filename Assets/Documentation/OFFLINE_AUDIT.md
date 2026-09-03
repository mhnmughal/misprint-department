# OFFLINE_AUDIT — Misprint Department

Last run: 2026-09-03 (Session 3 — re-verified after the 12-level campaign,
Color Pass mechanic, and both player builds).

## Method
`grep -rE 'UnityWebRequest|HttpClient|System.Net|WebClient|Socket|TcpClient|
Firebase|Analytics|AdMob|Advertisement|RemoteConfig|WebView'` over
`Assets/Scripts/`.

## Results
| Check | Result |
|-------|--------|
| Runtime network code (UnityWebRequest / HttpClient / sockets) | NONE |
| Ads / ad SDKs | NONE |
| Analytics / telemetry / tracking | NONE |
| Remote config | NONE |
| Cloud save / accounts / login / auth | NONE |
| Web views | NONE |
| Multiplayer / online leaderboards | NONE |
| Third-party runtime SDKs (Firebase, AdMob, AppsFlyer, GameAnalytics, …) | NONE |
| `com.unity.services.*` packages | NOT PRESENT in manifest |

Save data is a single local JSON file at
`Application.persistentDataPath/misprint_save.json`. Audio is generated
in-project. No asset is fetched at runtime.

## Verdict
**PASS: NO RUNTIME NETWORK FUNCTIONALITY.**

The `com.unity.modules.unityanalytics` stub was also removed in the
2026-09-03 package trim (see PACKAGE_AUDIT.md). The `unitywebrequest`
engine modules remain as package-manager transitive deps but no game or
editor-build code references them.
