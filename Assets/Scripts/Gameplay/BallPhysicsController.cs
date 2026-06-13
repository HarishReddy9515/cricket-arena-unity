using CricketArena.Core;
using UnityEngine;

namespace CricketArena.Gameplay
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class BallPhysicsController : MonoBehaviour
    {
        [SerializeField] private Rigidbody body;
        [SerializeField] private float timingWindowDistance = 4f;
        [SerializeField] private Transform contactPoint;

        public float NormalizedTimingWindow
        {
            get
            {
                if (contactPoint == null) return 0.5f;
                float distance = Vector3.Distance(transform.position, contactPoint.position);
                return Mathf.Clamp01(1f - distance / timingWindowDistance);
            }
        }

        private void Reset()
        {
            body = GetComponent<Rigidbody>();
        }

        public void Launch(Vector3 start, Vector3 target, DeliveryProfile delivery)
        {
            transform.position = start;
            body.velocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;

            Vector3 direction = (target - start).normalized;
            direction += Vector3.right * delivery.Swing;
            direction.y += delivery.Bounce * 0.08f;
            body.AddForce(direction.normalized * delivery.Speed, ForceMode.VelocityChange);
            body.AddTorque(Random.insideUnitSphere * delivery.Speed, ForceMode.VelocityChange);
        }

        public void ResolveShot(float power, Vector3 direction)
        {
            body.velocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
            body.AddForce(direction.normalized * Mathf.Lerp(12f, 36f, power), ForceMode.VelocityChange);
            body.AddTorque(Random.insideUnitSphere * Mathf.Lerp(8f, 22f, power), ForceMode.VelocityChange);
        }
    }
}
