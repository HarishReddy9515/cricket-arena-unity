using UnityEngine;

namespace CricketArena.Presentation
{
    public enum MobilePerformanceTier
    {
        Battery,
        Balanced,
        Performance
    }

    public sealed class MobilePerformanceManager : MonoBehaviour
    {
        [SerializeField] private MobilePerformanceTier defaultTier = MobilePerformanceTier.Balanced;
        [SerializeField] private bool applyOnStart = true;

        public MobilePerformanceTier CurrentTier { get; private set; }

        private void Start()
        {
            if (applyOnStart)
            {
                ApplyTier(defaultTier);
            }
        }

        public void ApplyBattery()
        {
            ApplyTier(MobilePerformanceTier.Battery);
        }

        public void ApplyBalanced()
        {
            ApplyTier(MobilePerformanceTier.Balanced);
        }

        public void ApplyPerformance()
        {
            ApplyTier(MobilePerformanceTier.Performance);
        }

        public void ApplyTier(MobilePerformanceTier tier)
        {
            CurrentTier = tier;
            QualitySettings.vSyncCount = 0;

            switch (tier)
            {
                case MobilePerformanceTier.Battery:
                    Application.targetFrameRate = 30;
                    QualitySettings.antiAliasing = 0;
                    QualitySettings.shadowDistance = 28f;
                    QualitySettings.lodBias = 0.7f;
                    break;
                case MobilePerformanceTier.Performance:
                    Application.targetFrameRate = 60;
                    QualitySettings.antiAliasing = 2;
                    QualitySettings.shadowDistance = 70f;
                    QualitySettings.lodBias = 1.3f;
                    break;
                default:
                    Application.targetFrameRate = 60;
                    QualitySettings.antiAliasing = 2;
                    QualitySettings.shadowDistance = 48f;
                    QualitySettings.lodBias = 1f;
                    break;
            }
        }
    }
}
