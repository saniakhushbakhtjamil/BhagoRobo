using UnityEngine;

namespace Blind
{
    // Smoothly follows the player and rotates to match their facing direction.
    // The camera pitch (~60 degrees) is set in the Inspector on the camera itself.
    // Option 1: Delayed camera rotation — the camera waits until the robot has held
    // a direction for at least rotationDelay seconds before it starts rotating.
    // This prevents the camera from spinning on quick taps and keeps movement cinematic.
    public class CameraFollow : MonoBehaviour
    {
        [Tooltip("The player transform to follow")]
        [SerializeField] Transform target;

        [Tooltip("How quickly the camera position catches up to the player")]
        [SerializeField] float smoothSpeed = 8f;

        [Tooltip("How quickly the camera rotates to match the player (lower = lazier follow)")]
        [SerializeField] float rotationSmoothSpeed = 3f;

        [Tooltip("Offset from the player in local space (set Y to control height)")]
        [SerializeField] Vector3 offset = new Vector3(0f, 8f, -9f);

        [Tooltip("Seconds the robot must hold a direction before the camera starts rotating")]
        [SerializeField] float rotationDelay = 0.4f;

        // How long the robot's Y rotation has been stable (not changing)
        float directionHeldTime = 0f;

        // The robot's Y rotation recorded last frame, used to detect changes
        float lastTargetYRotation;

        void Start()
        {
            if (target != null)
                lastTargetYRotation = target.eulerAngles.y;
        }

        void LateUpdate()
        {
            if (target == null) return;

            // --- Position: always follows immediately ---
            Vector3 desiredPosition = target.TransformPoint(offset);
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

            // --- Rotation: delayed follow ---
            float currentTargetY = target.eulerAngles.y;
            float yDelta = Mathf.Abs(Mathf.DeltaAngle(currentTargetY, lastTargetYRotation));

            // If the robot's facing direction changed this frame, reset the hold timer
            if (yDelta > 0.1f)
            {
                directionHeldTime = 0f;
            }
            else
            {
                // Direction is stable — accumulate held time
                directionHeldTime += Time.deltaTime;
            }

            lastTargetYRotation = currentTargetY;

            // Only rotate the camera once the robot has held the direction long enough
            if (directionHeldTime >= rotationDelay)
            {
                Quaternion desiredRotation = Quaternion.Euler(
                    transform.eulerAngles.x,
                    currentTargetY,
                    0f
                );
                transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSmoothSpeed * Time.deltaTime);
            }
        }
    }
}
