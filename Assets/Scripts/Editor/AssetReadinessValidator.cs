#if UNITY_EDITOR
using System.IO;
using CricketArena.Presentation;
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

            CricketAssetManifest manifest = FindManifest();
            if (manifest == null)
            {
                Debug.LogWarning("No CricketAssetManifest found. Run Cricket Arena > Create Asset Manifest.");
                return;
            }

            ValidateManifest(manifest);
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

        [MenuItem("Cricket Arena/Create Asset Manifest")]
        public static void CreateManifest()
        {
            CreateFolders();
            const string path = "Assets/Art/CricketAssetManifest.asset";
            CricketAssetManifest existing = AssetDatabase.LoadAssetAtPath<CricketAssetManifest>(path);
            if (existing != null)
            {
                Selection.activeObject = existing;
                Debug.Log("CricketAssetManifest already exists.");
                return;
            }

            CricketAssetManifest manifest = ScriptableObject.CreateInstance<CricketAssetManifest>();
            AssetDatabase.CreateAsset(manifest, path);
            AssetDatabase.SaveAssets();
            Selection.activeObject = manifest;
            Debug.Log($"Created asset manifest at {path}.");
        }

        private static CricketAssetManifest FindManifest()
        {
            string[] guids = AssetDatabase.FindAssets("t:CricketAssetManifest", new[] { "Assets" });
            if (guids.Length == 0) return null;
            return AssetDatabase.LoadAssetAtPath<CricketAssetManifest>(AssetDatabase.GUIDToAssetPath(guids[0]));
        }

        private static void ValidateManifest(CricketAssetManifest manifest)
        {
            Check(manifest.stadiumPrefab, "stadiumPrefab");
            Check(manifest.batterPrefab, "batterPrefab");
            Check(manifest.bowlerPrefab, "bowlerPrefab");
            Check(manifest.batPrefab, "batPrefab");
            Check(manifest.wicketPrefab, "wicketPrefab");
            Check(manifest.batterAnimator, "batterAnimator");
            Check(manifest.bowlerAnimator, "bowlerAnimator");
            Check(manifest.batHit, "batHit");
            Check(manifest.crowdLoop, "crowdLoop");
            Check(manifest.wicket, "wicket");
        }

        private static void Check(Object asset, string field)
        {
            if (asset == null)
            {
                Debug.LogWarning($"CricketAssetManifest missing {field}.");
            }
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
