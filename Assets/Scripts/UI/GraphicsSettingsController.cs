using CricketArena.Presentation;
using UnityEngine;
using UnityEngine.UI;

namespace CricketArena.UI
{
    public sealed class GraphicsSettingsController : MonoBehaviour
    {
        [SerializeField] private MobilePerformanceManager performanceManager;
        [SerializeField] private Text statusText;

        public void SetBattery()
        {
            performanceManager?.ApplyBattery();
            SetStatus("Graphics: Battery");
        }

        public void SetBalanced()
        {
            performanceManager?.ApplyBalanced();
            SetStatus("Graphics: Balanced");
        }

        public void SetPerformance()
        {
            performanceManager?.ApplyPerformance();
            SetStatus("Graphics: Performance");
        }

        private void SetStatus(string value)
        {
            if (statusText != null)
            {
                statusText.text = value;
            }
        }
    }
}
