#if UNITY_IOS
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;

namespace MisprintDepartment.Editor.Build
{
    /// <summary>
    /// Makes the generated Xcode project submission-ready without hand edits on
    /// the Mac:
    ///  - declares no non-exempt encryption (the app makes no network calls), so
    ///    App Store Connect's export-compliance question is answered automatically;
    ///  - adds an app-level privacy manifest stating no tracking and no data
    ///    collection. Unity's own UnityFramework/PrivacyInfo.xcprivacy still
    ///    declares the engine's required-reason API usage.
    /// Runs only for iOS builds; no effect on any other platform or at runtime.
    /// </summary>
    public static class IosPostProcess
    {
        [PostProcessBuild(1000)]
        public static void OnPostprocessBuild(BuildTarget target, string buildPath)
        {
            if (target != BuildTarget.iOS) return;

            // 1. Export compliance.
            var plistPath = Path.Combine(buildPath, "Info.plist");
            var plist = new PlistDocument();
            plist.ReadFromFile(plistPath);
            plist.root.SetBoolean("ITSAppUsesNonExemptEncryption", false);
            plist.WriteToFile(plistPath);

            // 2. App-level privacy manifest: no tracking, no collected data.
            const string manifestName = "PrivacyInfo.xcprivacy";
            const string manifest =
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" +
                "<!DOCTYPE plist PUBLIC \"-//Apple//DTD PLIST 1.0//EN\" \"http://www.apple.com/DTDs/PropertyList-1.0.dtd\">\n" +
                "<plist version=\"1.0\">\n" +
                "<dict>\n" +
                "  <key>NSPrivacyTracking</key>\n" +
                "  <false/>\n" +
                "  <key>NSPrivacyTrackingDomains</key>\n" +
                "  <array/>\n" +
                "  <key>NSPrivacyCollectedDataTypes</key>\n" +
                "  <array/>\n" +
                "  <key>NSPrivacyAccessedAPITypes</key>\n" +
                "  <array/>\n" +
                "</dict>\n" +
                "</plist>\n";
            File.WriteAllText(Path.Combine(buildPath, manifestName), manifest);

            var pbxPath = PBXProject.GetPBXProjectPath(buildPath);
            var pbx = new PBXProject();
            pbx.ReadFromFile(pbxPath);
            string appGuid = pbx.GetUnityMainTargetGuid();
            string fileGuid = pbx.AddFile(manifestName, manifestName);
            pbx.AddFileToBuildSection(appGuid, pbx.GetResourcesBuildPhaseByTarget(appGuid), fileGuid);
            pbx.WriteToFile(pbxPath);

            Debug.Log("[Misprint] iOS post-process: ITSAppUsesNonExemptEncryption=NO; " +
                      "app-level PrivacyInfo.xcprivacy added to the main target.");
        }
    }
}
#endif
