using UnityEngine;
using UnityEngine.InputSystem;

namespace Blind
{
    // WASD moves and rotates the robot. Camera follows the robot's facing direction.
    // Mouse controls only the flashlight (handled by FlashlightAimer).
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : MonoBehaviour
    {
        [Tooltip("Movement speed in metres per second")]
        [SerializeField] float moveSpeed = 5f;

        [Tooltip("How smoothly the robot eases into the movement direction (lower = lazier turn)")]
        [SerializeField] float rotationSmoothing = 6f;

        Rigidbody rb;
        Camera mainCamera;
        Vector2 moveInput;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            mainCamera = Camera.main;

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

            // Rotate robot body toward movement direction (camera-relative)
            if (moveInput.sqrMagnitude > 0.01f)
            {
                Vector3 camForward = Vector3.ProjectOnPlane(mainCamera.transform.forward, Vector3.up).normalized;
                Vector3 camRight   = Vector3.ProjectOnPlane(mainCamera.transform.right,   Vector3.up).normalized;
                Vector3 moveDir    = (camForward * moveInput.y + camRight * moveInput.x).normalized;

                Quaternion targetRotation = Quaternion.LookRotation(moveDir);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation, targetRotation, rotationSmoothing * Time.deltaTime);
            }
        }

        void FixedUpdate()
        {
            if (!GameManager.Instance.IsPlaying()) return;

            // Move in camera-relative direction
            Vector3 camForward = Vector3.ProjectOnPlane(mainCamera.transform.forward, Vector3.up).normalized;
            Vector3 camRight   = Vector3.ProjectOnPlane(mainCamera.transform.right,   Vector3.up).normalized;
            Vector3 move       = (camForward * moveInput.y + camRight * moveInput.x);
            rb.MovePosition(rb.position + move * moveSpeed * Time.fixedDeltaTime);
        }
    }
}
