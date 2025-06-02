using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SpiderMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Walking speed along surfaces")]
    public float speed = 2f;
    [Tooltip("Rotation smoothing speed when aligning to surfaces")]
    public float rotationSpeed = 1f;
    [Tooltip("Max distance to check for walls and ground")]
    public float raycastDistance = 0.5f;
    [Tooltip("Layers considered crawlable (walls, floors)")]
    public LayerMask wallMask;

    [Header("Jump Settings")]
    [Tooltip("Force applied on jump")]
    public float jumpForce = 6f;

    [Header("Gravity Settings")]
    [Tooltip("Multiplier for gravity when in air")]
    public float gravityMultiplier = 2f;

    [Header("References")]
    [Tooltip("Assign Main Camera transform for direction references")]
    public Transform cameraTransform;

    private Rigidbody _rb;
    private Vector3 _surfaceNormal = Vector3.up;
    private bool _canJump = true;
    private bool _isGrounded;
    private bool _recentWallJump;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;                   // We apply gravity manually
        _rb.constraints = RigidbodyConstraints.FreezeRotation;

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        DetectSurface();
        HandleJumpInput();
    }

    void FixedUpdate()
    {
        HandleMovement();
        ApplyCustomGravity();
    }

    /// <summary>
    /// Casts rays to detect walls or ground, and updates the current surface normal and grounded state.
    /// When grounded, re-enable jumping. Skips detection for one frame after wall-jump.
    /// </summary>
    private void DetectSurface()
    {
        if (_recentWallJump)
        {
            // Skip detection for one FixedUpdate frame after wall-jump
            _surfaceNormal = Vector3.up;
            _isGrounded = false;
            _recentWallJump = false;
            return;
        }

        var origin = transform.position;

        // 1) Check for a wall in camera's forward direction
        if (Physics.Raycast(origin, cameraTransform.forward, out var hit, raycastDistance, wallMask))
        {
            _surfaceNormal = hit.normal;
            _isGrounded = false;
        }
        // 2) Else, check for ground directly beneath the spider
        else if (Physics.Raycast(origin, -transform.up, out hit, raycastDistance, wallMask))
        {
            _surfaceNormal = hit.normal;
            if (!_isGrounded)
            {
                // Landed on a crawlable surface: re-enable jump
                _isGrounded = true;
                _canJump = true;
            }
        }
        else
        {
            _surfaceNormal = Vector3.up;
            _isGrounded = false;
        }

        // Smoothly align the spider's up direction to the detected surface normal
        Quaternion targetRot = Quaternion.FromToRotation(transform.up, _surfaceNormal) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);
    }

    /// <summary>
    /// Handles movement by setting the Rigidbody's horizontal velocity directly.
    /// Uses a SphereCast from the spider's position to prevent clipping into near-vertical walls,
    /// but allows crawling under low geometry (ceilings) by only blocking truly vertical surfaces.
    /// </summary>
    private void HandleMovement()
    {
        // 1) Read raw input axes
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // 2) Compute camera-relative directions projected onto the current surface
        Vector3 camForward = Vector3.ProjectOnPlane(cameraTransform.forward, _surfaceNormal).normalized;
        Vector3 camRight   = Vector3.ProjectOnPlane(cameraTransform.right,   _surfaceNormal).normalized;
        Vector3 moveDir    = (camForward * v + camRight * h).normalized;

        // 3) Preserve vertical (surface-normal) component of current velocity
        Vector3 normalVel = Vector3.Project(_rb.linearVelocity, _surfaceNormal);

        // 4) Desired velocity = horizontal input + preserved vertical component
        Vector3 desiredVelocity = moveDir * speed + normalVel;

        // 5) If there is horizontal input, do a SphereCast to detect near-vertical walls
        if (moveDir.sqrMagnitude > 0.001f)
        {
            float checkDistance = speed * Time.fixedDeltaTime + 0.1f;
            Vector3 castOrigin = transform.position; 
            float sphereRadius = 0.1f;

            // SphereCast to see if there's a near-vertical wall in front
            if (Physics.SphereCast(castOrigin, sphereRadius, moveDir, out var hit, checkDistance, wallMask))
            {
                // Only block if the surface normal is near-vertical (dot with Vector3.up < 0.7)
                float verticalDot = Mathf.Abs(Vector3.Dot(hit.normal, Vector3.up));
                // verticalDot near zero => purely vertical wall
                if (verticalDot < 0.7f)
                {
                    // Wall detected: zero horizontal component so we don't move into it
                    desiredVelocity = normalVel;
                }
                // Otherwise (ceiling or floor), do not block movement—allows crawling under
            }
        }
        else
        {
            // No input → zero horizontal component immediately (prevents drift)
            desiredVelocity = normalVel;
        }

        // 6) Directly set the Rigidbody's velocity so there's no residual drift
        _rb.linearVelocity = desiredVelocity;
    }

    /// <summary>
    /// Applies downward gravity when airborne, or a small stick force when grounded.
    /// </summary>
    private void ApplyCustomGravity()
    {
        if (_isGrounded)
        {
            // Slight push into ground to prevent sliding off edges
            _rb.AddForce(-_surfaceNormal * (gravityMultiplier * 2f), ForceMode.Acceleration);
        }
        else
        {
            // Normal downward gravity
            _rb.AddForce(Physics.gravity * gravityMultiplier, ForceMode.Acceleration);
        }
    }

    /// <summary>
    /// Allows jumping only if currently grounded. Clears any vertical velocity,
    /// then applies an impulse along the surface normal. Jump can only occur again
    /// after DetectSurface sets _isGrounded = true on landing.
    /// </summary>
    private void HandleJumpInput()
    {
        if (!_isGrounded || !_canJump)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Remove any vertical component, preserving only horizontal
            Vector3 horizontalVel = Vector3.ProjectOnPlane(_rb.linearVelocity, _surfaceNormal);
            _rb.linearVelocity = horizontalVel;

            // Apply jump impulse along the surface normal
            _rb.AddForce(_surfaceNormal * jumpForce, ForceMode.Impulse);

            _canJump = false;
            _recentWallJump = true;
        }
    }
}
