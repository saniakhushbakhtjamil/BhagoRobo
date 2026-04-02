using UnityEngine;
using UnityEngine.InputSystem;

namespace Blind
{
    // WASD moves the robot. Mouse cursor aims the robot's facing direction.
    // Camera rotates with the robot, so WASD is always relative to where you're looking.
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : MonoBehaviour
    {
        [Tooltip("Movement speed in metres per second")]
        [SerializeField] float moveSpeed = 5f;

        [Tooltip("How quickly the robot snaps to face the mouse (degrees/sec)")]
        [SerializeField] float rotationSpeed = 720f;

        Rigidbody rb;
        Camera mainCamera;
        Vector2 moveInput;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            mainCamera = Camera.main;

            // Keep the robot upright — physics shouldn't tip it over
            rb.constraints = RigidbodyConstraints.FreezeRotationX
                           | RigidbodyConstraints.FreezeRotationZ
                           | RigidbodyConstraints.FreezePositionY;
        }

        void Update()
        {
            if (!GameManager.Instance.IsPlaying()) return;

            // Read WASD
            moveInput = new Vector2(
                Keyboard.current.dKey.ReadValue() - Keyboard.current.aKey.ReadValue(),
                Keyboard.current.wKey.ReadValue() - Keyboard.current.sKey.ReadValue()
            );

            // Aim robot toward mouse cursor
            RotateTowardMouse();
        }

        void FixedUpdate()
        {
            if (!GameManager.Instance.IsPlaying()) return;

            // Move relative to robot's current facing so forward is always "up" on screen
            Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
            rb.MovePosition(rb.position + move * moveSpeed * Time.fixedDeltaTime);
        }

        void RotateTowardMouse()
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            // The floor is at Y = 0
            Plane floor = new Plane(Vector3.up, Vector3.zero);

            if (floor.Raycast(ray, out float distance))
            {
                Vector3 target = ray.GetPoint(distance);
                Vector3 direction = (target - transform.position).normalized;
                direction.y = 0f;

                if (direction.sqrMagnitude > 0.01f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.RotateTowards(
                        transform.rotation, targetRotation, rotationSpeed * Time.deltaTime
                    );
                }
            }
        }
    }
}
