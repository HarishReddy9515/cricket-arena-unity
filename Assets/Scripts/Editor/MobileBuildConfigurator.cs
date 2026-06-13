#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace CricketArena.EditorTools
{
    public static class MobileBuildConfigurator
    {
        [MenuItem("Cricket Arena/Configure Android Mobile Build")]
        public static void ConfigureAndroid()
        {
            PlayerSettings.companyName = "HarishReddy";
            PlayerSettings.productName = "Cricket Arena";
            PlayerSettings.applicationIdentifier = "com.harishreddy.cricketarena";
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            PlayerSettings.Android.preferredInstallLocation = AndroidPreferredInstallLocation.Auto;
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel23;
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;
            EditorUserBuildSettings.androidBuildSubtarget = MobileTextureSubtarget.ASTC;
            QualitySettings.vSyncCount = 0;
            QualitySettings.antiAliasing = 2;
            QualitySettings.shadowDistance = 48f;
            QualitySettings.lodBias = 1f;
            Application.targetFrameRate = 60;
            Debug.Log("Android build settings configured for Cricket Arena.");
        }
    }
}
#endif
