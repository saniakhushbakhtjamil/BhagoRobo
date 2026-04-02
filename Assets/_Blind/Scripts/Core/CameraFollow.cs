using UnityEngine;

namespace Blind
{
    // Smoothly follows the player and rotates to match their facing direction.
    // The camera pitch (~60 degrees) is set in the Inspector on the camera itself.
    public class CameraFollow : MonoBehaviour
    {
        [Tooltip("The player transform to follow")]
        [SerializeField] Transform target;

        [Tooltip("How quickly the camera catches up to the player")]
        [SerializeField] float smoothSpeed = 8f;

        [Tooltip("Offset from the player in local space (set Y to control height)")]
        [SerializeField] Vector3 offset = new Vector3(0f, 10f, -6f);

        void LateUpdate()
        {
            if (target == null) return;

            // Desired position: behind and above the player, rotated with them
            Vector3 desiredPosition = target.TransformPoint(offset);
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

            // Rotate to match player's Y rotation, keeping the camera's own pitch
            Quaternion desiredRotation = Quaternion.Euler(
                transform.eulerAngles.x,
                target.eulerAngles.y,
                0f
            );
            transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, smoothSpeed * Time.deltaTime);
        }
    }
}
