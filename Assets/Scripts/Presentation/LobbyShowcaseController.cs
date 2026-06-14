using UnityEngine;

namespace CricketArena.Presentation
{
    public sealed class LobbyShowcaseController : MonoBehaviour
    {
        [SerializeField] private Transform showcaseTarget;
        [SerializeField] private float rotationSpeed = 16f;
        [SerializeField] private float floatAmplitude = 0.08f;
        [SerializeField] private float floatSpeed = 1.4f;

        private Vector3 initialPosition;

        private void Awake()
        {
            if (showcaseTarget != null)
            {
                initialPosition = showcaseTarget.position;
            }
        }

        private void Update()
        {
            if (showcaseTarget == null) return;

            showcaseTarget.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
            showcaseTarget.position = initialPosition + Vector3.up * (Mathf.Sin(Time.time * floatSpeed) * floatAmplitude);
        }
    }
}
