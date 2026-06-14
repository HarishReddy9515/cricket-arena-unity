using CricketArena.Presentation;
using CricketArena.UI;
using UnityEngine;

namespace CricketArena.Core
{
    public sealed class SaveGameManager : MonoBehaviour
    {
        private const string SaveKey = "cricket-arena-save-v1";

        [SerializeField] private LoadoutController loadoutController;
        [SerializeField] private SeasonProgression seasonProgression;
        [SerializeField] private MobilePerformanceManager performanceManager;

        private void Start()
        {
            Load();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                Save();
            }
        }

        private void OnApplicationQuit()
        {
            Save();
        }

        public void Save()
        {
            SaveGameData data = new SaveGameData
            {
                Loadout = loadoutController != null ? loadoutController.CurrentLoadout : PlayerLoadout.Default,
                SeasonName = seasonProgression != null ? seasonProgression.SeasonName : SaveGameData.Default.SeasonName,
                SeasonTier = seasonProgression != null ? seasonProgression.Tier : 1,
                SeasonXp = seasonProgression != null ? seasonProgression.SeasonXp : 0,
                PerformanceTier = performanceManager != null ? performanceManager.CurrentTier : MobilePerformanceTier.Balanced
            };

            PlayerPrefs.SetString(SaveKey, JsonUtility.ToJson(data));
            PlayerPrefs.Save();
        }

        public void Load()
        {
            SaveGameData data = PlayerPrefs.HasKey(SaveKey)
                ? JsonUtility.FromJson<SaveGameData>(PlayerPrefs.GetString(SaveKey))
                : SaveGameData.Default;

            if (data == null)
            {
                data = SaveGameData.Default;
            }

            loadoutController?.ApplyLoadout(data.Loadout);
            seasonProgression?.ApplySeasonState(data.SeasonName, data.SeasonTier, data.SeasonXp);
            performanceManager?.ApplyTier(data.PerformanceTier);
        }

        public void ResetSave()
        {
            PlayerPrefs.DeleteKey(SaveKey);
            Load();
        }
    }
}
