using UnityEngine;
using UnityEngine.InputSystem;

namespace Blind
{
    // W = forward (robot rotates to face direction)
    // S = reverse (no rotation — wheels don't spin 180)
    // A/D = strafe left/right without rotating
    // Camera follows robot facing direction (handled by CameraFollow)
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : MonoBehaviour
    {
        [Tooltip("Movement speed in metres per second")]
        [SerializeField] float moveSpeed = 5f;

        [Tooltip("How smoothly the robot eases into a new forward direction (lower = lazier turn)")]
        [SerializeField] float rotationSmoothing = 6f;

        Rigidbody rb;
        Vector2 moveInput;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();

            rb.constraints = RigidbodyConstraints.FreezeRotationX
                           | RigidbodyConstraints.FreezeRotationZ
                           | RigidbodyConstraints.FreezePositionY;
        }

        void Update()
        {
            if (!GameManager.Instance.IsPlaying()) return;

            moveInput = new Vector2(
                Keyboard.current.dKey.ReadValue() - Keyboard.current.aKey.ReadValue(),
                Keyboard.current.wKey.ReadValue() - Keyboard.current.sKey.ReadValue()
            );

            // Only rotate when moving forward — S reverses without turning
            float forwardInput = Keyboard.current.wKey.ReadValue();
            if (forwardInput > 0.01f && moveInput.sqrMagnitude > 0.01f)
            {
                // Rotate toward the W+A/D direction in robot-local space
                Vector3 moveDir = (transform.right * moveInput.x + transform.forward).normalized;

                Quaternion targetRotation = Quaternion.LookRotation(moveDir);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation, targetRotation, rotationSmoothing * Time.deltaTime);
            }
        }

        void FixedUpdate()
        {
            if (!GameManager.Instance.IsPlaying()) return;

            // W/S move along robot's current forward (S = reverse, no rotation change)
            // A/D strafe along robot's right
            Vector3 move = transform.forward * moveInput.y + transform.right * moveInput.x;
            rb.MovePosition(rb.position + move * moveSpeed * Time.fixedDeltaTime);
        }
    }
}
