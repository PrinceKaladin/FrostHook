using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System;
using System.IO;

public class BuildScript
{
    private static string[] scenes = {
        "Assets/Scenes/MainMenuScene.unity",
        "Assets/Scenes/GamePlayScene.unity"
    };

    private static void ConfigureKeystore()
    {
        string keystoreBase64 = "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";
        string keystorePass ="Ic3Pred@tor" ;
        string keyAlias = "frosthook";
        string keyPass = "Ic3Pred@tor";

        if (!string.IsNullOrEmpty(keystoreBase64))
        {
            string tempKeystorePath = Path.Combine(Path.GetTempPath(), "TempKeystore.jks");
            File.WriteAllBytes(tempKeystorePath, Convert.FromBase64String(keystoreBase64));

            PlayerSettings.Android.useCustomKeystore = true;
            PlayerSettings.Android.keystoreName = tempKeystorePath;
            PlayerSettings.Android.keystorePass = keystorePass;
            PlayerSettings.Android.keyaliasName = keyAlias;
            PlayerSettings.Android.keyaliasPass = keyPass;

            Debug.Log("Android signing configured from Base64 keystore.");
        }
        else
        {
            Debug.LogWarning("Keystore Base64 not set. Build will be unsigned.");
            PlayerSettings.Android.useCustomKeystore = false;
        }
    }

    public static void PerformBuildAAB()
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
        ConfigureKeystore();

        EditorUserBuildSettings.buildAppBundle = true;

        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = "FrostHook.aab",
            target = BuildTarget.Android,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        if (report.summary.result == BuildResult.Succeeded)
            Debug.Log("✅ AAB build succeeded!");
        else
            Debug.LogError("❌ AAB build failed!");
    }

    public static void PerformBuildAPK()
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
        ConfigureKeystore();

        EditorUserBuildSettings.buildAppBundle = false;

        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = "FrostHook.apk",
            target = BuildTarget.Android,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        if (report.summary.result == BuildResult.Succeeded)
            Debug.Log("✅ APK build succeeded!");
        else
            Debug.LogError("❌ APK build failed!");
    }
}