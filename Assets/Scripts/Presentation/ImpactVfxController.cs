using CricketArena.Core;
using UnityEngine;

namespace CricketArena.Presentation
{
    public sealed class ImpactVfxController : MonoBehaviour
    {
        [SerializeField] private ParticleSystem boundaryParticles;
        [SerializeField] private ParticleSystem wicketParticles;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip batHit;
        [SerializeField] private AudioClip wicketHit;
        [SerializeField] private CameraDirector cameraDirector;
        [SerializeField] private RuntimeAssetBinder assetBinder;

        public void Play(ShotOutcome outcome)
        {
            if (outcome.IsWicket)
            {
                wicketParticles?.Play();
                assetBinder?.PlayWicket();
                PlayClip(wicketHit);
                cameraDirector?.Shake(0.22f);
                return;
            }

            if (outcome.Runs >= 4)
            {
                boundaryParticles?.Play();
                assetBinder?.PlayBoundary();
                PlayClip(batHit);
                cameraDirector?.Shake(outcome.Runs >= 6 ? 0.18f : 0.11f);
                return;
            }

            assetBinder?.PlayBatHit();
        }

        private void PlayClip(AudioClip clip)
        {
            if (audioSource != null && clip != null)
            {
                audioSource.PlayOneShot(clip);
            }
        }
    }
}
