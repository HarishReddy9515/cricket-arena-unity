#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace CricketArena.EditorTools
{
    public static class AssetReadinessValidator
    {
        private static readonly string[] RecommendedFolders =
        {
            "Assets/Art/Models/Stadium",
            "Assets/Art/Models/Players",
            "Assets/Art/Animations/Batting",
            "Assets/Art/Animations/Bowling",
            "Assets/Audio",
            "Assets/VFX",
            "Assets/UI"
        };

        [MenuItem("Cricket Arena/Validate Asset Readiness")]
        public static void Validate()
        {
            int missing = 0;
            foreach (string folder in RecommendedFolders)
            {
                if (!AssetDatabase.IsValidFolder(folder))
                {
                    missing++;
                    Debug.LogWarning($"Missing recommended asset folder: {folder}");
                }
            }

            string[] modelGuids = AssetDatabase.FindAssets("t:Model", new[] { "Assets/Art" });
            string[] audioGuids = AssetDatabase.FindAssets("t:AudioClip", new[] { "Assets/Audio" });
            string[] materialGuids = AssetDatabase.FindAssets("t:Material", new[] { "Assets/Art" });

            Debug.Log($"Cricket Arena asset readiness: models={modelGuids.Length}, audio={audioGuids.Length}, materials={materialGuids.Length}, missingFolders={missing}");

            if (modelGuids.Length == 0)
            {
                Debug.LogWarning("No imported 3D models found. The scene builder will use procedural placeholders.");
            }

            if (audioGuids.Length == 0)
            {
                Debug.LogWarning("No audio clips found. Add legal bat-hit, crowd, and wicket audio for premium feel.");
            }
        }

        [MenuItem("Cricket Arena/Create Recommended Asset Folders")]
        public static void CreateFolders()
        {
            foreach (string folder in RecommendedFolders)
            {
                EnsureFolder(folder);
            }
            AssetDatabase.Refresh();
            Debug.Log("Cricket Arena asset folders are ready.");
        }

        private static void EnsureFolder(string folder)
        {
            string[] parts = folder.Split('/');
            string current = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                string next = $"{current}/{parts[i]}";
                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(current, parts[i]);
                }
                current = next;
            }
        }
    }
}
#endif
