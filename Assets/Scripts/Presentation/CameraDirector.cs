using UnityEngine;

namespace CricketArena.Presentation
{
    public sealed class CameraDirector : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Transform lobbyCamera;
        [SerializeField] private Transform battingCamera;
        [SerializeField] private Transform replayCamera;
        [SerializeField] private float followSpeed = 6f;

        private Transform targetRig;
        private float shake;

        private void Awake()
        {
            targetRig = lobbyCamera != null ? lobbyCamera : battingCamera;
        }

        private void LateUpdate()
        {
            if (mainCamera == null || targetRig == null) return;

            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetRig.position, Time.deltaTime * followSpeed);
            mainCamera.transform.rotation = Quaternion.Slerp(mainCamera.transform.rotation, targetRig.rotation, Time.deltaTime * followSpeed);

            if (shake > 0.01f)
            {
                mainCamera.transform.position += Random.insideUnitSphere * shake;
                shake = Mathf.Lerp(shake, 0f, Time.deltaTime * 8f);
            }
        }

        public void ShowLobbyCamera()
        {
            targetRig = lobbyCamera != null ? lobbyCamera : battingCamera;
        }

        public void ShowBattingCamera()
        {
            targetRig = battingCamera;
        }

        public void ShowReplayCamera()
        {
            targetRig = replayCamera;
        }

        public void Shake(float amount)
        {
            shake = Mathf.Max(shake, amount);
        }
    }
}
