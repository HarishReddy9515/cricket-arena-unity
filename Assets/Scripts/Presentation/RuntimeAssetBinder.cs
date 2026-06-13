using UnityEngine;

namespace CricketArena.Presentation
{
    public sealed class RuntimeAssetBinder : MonoBehaviour
    {
        [SerializeField] private CricketAssetManifest manifest;
        [SerializeField] private Transform stadiumMount;
        [SerializeField] private Transform batterMount;
        [SerializeField] private Transform bowlerMount;
        [SerializeField] private Transform batMount;
        [SerializeField] private Transform wicketMount;
        [SerializeField] private AudioSource crowdAudio;
        [SerializeField] private AudioSource effectsAudio;

        public bool AssetsApplied { get; private set; }

        private void Start()
        {
            ApplyManifest();
        }

        public void ApplyManifest()
        {
            if (manifest == null)
            {
                return;
            }

            Replace(stadiumMount, manifest.stadiumPrefab);
            Replace(batterMount, manifest.batterPrefab);
            Replace(bowlerMount, manifest.bowlerPrefab);
            Replace(batMount, manifest.batPrefab);
            Replace(wicketMount, manifest.wicketPrefab);

            if (crowdAudio != null && manifest.crowdLoop != null)
            {
                crowdAudio.clip = manifest.crowdLoop;
                crowdAudio.loop = true;
                crowdAudio.Play();
            }

            AssetsApplied = manifest.HasPlayableCoreAssets;
        }

        public void PlayBatHit()
        {
            PlayOneShot(manifest != null ? manifest.batHit : null);
        }

        public void PlayWicket()
        {
            PlayOneShot(manifest != null ? manifest.wicket : null);
        }

        public void PlayBoundary()
        {
            PlayOneShot(manifest != null ? manifest.boundary : null);
        }

        private void PlayOneShot(AudioClip clip)
        {
            if (effectsAudio != null && clip != null)
            {
                effectsAudio.PlayOneShot(clip);
            }
        }

        private static void Replace(Transform mount, GameObject prefab)
        {
            if (mount == null || prefab == null) return;

            for (int i = mount.childCount - 1; i >= 0; i--)
            {
                Destroy(mount.GetChild(i).gameObject);
            }

            GameObject instance = Instantiate(prefab, mount);
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localRotation = Quaternion.identity;
            instance.transform.localScale = Vector3.one;
        }
    }
}
