using UnityEngine;
using UnityEngine.InputSystem;

namespace Blind
{
    // Wheeled vehicle physics:
    // - W/S: accelerate/reverse with momentum
    // - A/D: turn, scaled by speed (can't spin in place)
    // - No strafing — lateral friction prevents sideways sliding
    // - Natural deceleration via Rigidbody drag
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : MonoBehaviour
    {
        [Tooltip("Force applied when pressing W/S")]
        [SerializeField] float accelerationForce = 18f;

        [Tooltip("Maximum forward/reverse speed (m/s)")]
        [SerializeField] float maxSpeed = 7f;

        [Tooltip("How fast the robot turns (degrees/sec at full speed)")]
        [SerializeField] float turnSpeed = 90f;

        [Tooltip("How strongly sideways sliding is cancelled (wheel friction)")]
        [SerializeField] float lateralFriction = 12f;

        [Tooltip("Drag applied when no input — controls how quickly it coasts to a stop")]
        [SerializeField] float drivingDrag = 1.5f;

        [Tooltip("Extra drag when braking (S pressed while moving forward)")]
        [SerializeField] float brakingDrag = 4f;

        Rigidbody rb;
        float forwardInput;
        float turnInput;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeRotationX
                           | RigidbodyConstraints.FreezeRotationZ
                           | RigidbodyConstraints.FreezePositionY;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.drag = drivingDrag;
        }

        void Update()
        {
            if (!GameManager.Instance.IsPlaying()) return;

            forwardInput = Keyboard.current.wKey.ReadValue() - Keyboard.current.sKey.ReadValue();
            turnInput    = Keyboard.current.dKey.ReadValue() - Keyboard.current.aKey.ReadValue();
        }

        void FixedUpdate()
        {
            if (!GameManager.Instance.IsPlaying()) return;

            float currentSpeed = Vector3.Dot(rb.linearVelocity, transform.forward);

            // ── Acceleration ──────────────────────────────────────────────────
            if (Mathf.Abs(currentSpeed) < maxSpeed)
                rb.AddForce(transform.forward * forwardInput * accelerationForce, ForceMode.Acceleration);

            // ── Drag: heavier when braking ────────────────────────────────────
            bool braking = forwardInput < 0f && currentSpeed > 0f
                        || forwardInput > 0f && currentSpeed < 0f;
            rb.drag = braking ? brakingDrag : drivingDrag;

            // ── Turning — scaled by speed so you can't spin in place ──────────
            float speedFraction = Mathf.Clamp01(Mathf.Abs(currentSpeed) / maxSpeed);
            float turnDirection = Mathf.Sign(currentSpeed); // reverses turn direction when reversing
            float yaw = turnInput * turnSpeed * speedFraction * turnDirection * Time.fixedDeltaTime;
            transform.Rotate(0f, yaw, 0f, Space.World);

            // ── Lateral friction — kills sideways sliding ─────────────────────
            Vector3 lateralVel = Vector3.Dot(rb.linearVelocity, transform.right) * transform.right;
            rb.AddForce(-lateralVel * lateralFriction, ForceMode.Acceleration);
        }
    }
}
