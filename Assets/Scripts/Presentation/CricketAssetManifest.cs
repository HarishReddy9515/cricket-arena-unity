using UnityEngine;

namespace CricketArena.Presentation
{
    [CreateAssetMenu(menuName = "Cricket Arena/Asset Manifest", fileName = "CricketAssetManifest")]
    public sealed class CricketAssetManifest : ScriptableObject
    {
        [Header("Models")]
        public GameObject stadiumPrefab;
        public GameObject batterPrefab;
        public GameObject bowlerPrefab;
        public GameObject batPrefab;
        public GameObject wicketPrefab;

        [Header("Animation")]
        public RuntimeAnimatorController batterAnimator;
        public RuntimeAnimatorController bowlerAnimator;
        public AnimationClip[] battingClips;
        public AnimationClip[] bowlingClips;

        [Header("Audio")]
        public AudioClip batHit;
        public AudioClip crowdLoop;
        public AudioClip wicket;
        public AudioClip boundary;

        [Header("Materials")]
        public Material grassMaterial;
        public Material pitchMaterial;
        public Material ballMaterial;
        public Material kitPrimaryMaterial;
        public Material kitOpponentMaterial;

        public bool HasPlayableCoreAssets =>
            stadiumPrefab != null &&
            batterPrefab != null &&
            bowlerPrefab != null &&
            batPrefab != null &&
            wicketPrefab != null;
    }
}
