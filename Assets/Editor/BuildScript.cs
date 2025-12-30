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
        string keystoreBase64 = Environment.GetEnvironmentVariable("CM_KEYSTORE_BASE64");
        string keystorePass = Environment.GetEnvironmentVariable("CM_KEYSTORE_PASSWORD");
        string keyAlias = Environment.GetEnvironmentVariable("CM_KEY_ALIAS");
        string keyPass = Environment.GetEnvironmentVariable("CM_KEY_PASSWORD");

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