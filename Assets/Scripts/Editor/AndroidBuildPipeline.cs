#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace CricketArena.EditorTools
{
    public static class AndroidBuildPipeline
    {
        private const string DefaultScenePath = "Assets/Scenes/ArenaPrototype.unity";
        private const string ApkPath = "Builds/Android/CricketArena.apk";
        private const string AabPath = "Builds/Android/CricketArena.aab";

        [MenuItem("Cricket Arena/Build Android APK")]
        public static void BuildAndroidApk()
        {
            BuildAndroid(false, ApkPath);
        }

        [MenuItem("Cricket Arena/Build Android AAB")]
        public static void BuildAndroidAab()
        {
            BuildAndroid(true, AabPath);
        }

        public static void BuildAndroidFromCommandLine()
        {
            bool appBundle = HasArg("-aab");
            string outputPath = GetArg("-outputPath", appBundle ? AabPath : ApkPath);
            BuildAndroid(appBundle, outputPath);
        }

        private static void BuildAndroid(bool appBundle, string outputPath)
        {
            MobileBuildConfigurator.ConfigureAndroid();
            EditorUserBuildSettings.buildAppBundle = appBundle;

            string[] scenes = ResolveBuildScenes();
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? "Builds/Android");

            BuildPlayerOptions options = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = outputPath,
                target = BuildTarget.Android,
                options = BuildOptions.StrictMode
            };

            BuildReport report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result != BuildResult.Succeeded)
            {
                throw new InvalidOperationException($"Android build failed: {report.summary.result}");
            }

            Debug.Log($"Android build succeeded: {outputPath} ({report.summary.totalSize} bytes)");
        }

        private static string[] ResolveBuildScenes()
        {
            if (File.Exists(DefaultScenePath))
            {
                return new[] { DefaultScenePath };
            }

            if (EditorBuildSettings.scenes.Length > 0)
            {
                return Array.ConvertAll(EditorBuildSettings.scenes, scene => scene.path);
            }

            throw new FileNotFoundException($"No build scene found. Generate and save {DefaultScenePath} first.");
        }

        private static bool HasArg(string name)
        {
            foreach (string arg in Environment.GetCommandLineArgs())
            {
                if (arg.Equals(name, StringComparison.OrdinalIgnoreCase)) return true;
            }

            return false;
        }

        private static string GetArg(string name, string fallback)
        {
            string[] args = Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length - 1; i++)
            {
                if (args[i].Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    return args[i + 1];
                }
            }

            return fallback;
        }
    }
}
#endif
