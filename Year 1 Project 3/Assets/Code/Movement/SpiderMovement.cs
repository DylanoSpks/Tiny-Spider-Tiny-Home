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

    // NEW: when true, skip all crawling/jumping/gravity logic
    [HideInInspector] public bool disableMovement;

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
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        if (disableMovement) return;               // ← STOP if swing‐hook is active

        DetectSurface();
        HandleJumpInput();
    }

    void FixedUpdate()
    {
        if (disableMovement) return;               // ← STOP if swing‐hook is active

        HandleMovement();
        ApplyCustomGravity();
    }

    /// <summary>
    /// Casts rays forward and downward to find a wall or floor.
    /// Now: if we hit a vertical wall, treat it as “grounded” so the spider sticks.
    /// </summary>
    private void DetectSurface()
    {
        if (_recentWallJump)
        {
            // Skip detection one frame after a wall‐jump so we don't immediately re‐stick.
            _surfaceNormal = Vector3.up;
            _isGrounded = false;
            _recentWallJump = false;
            return;
        }

        Vector3 origin = transform.position;
        RaycastHit hit;

        // 1) Check for a wall in camera’s forward direction
        if (Physics.Raycast(origin, cameraTransform.forward, out hit, raycastDistance, wallMask))
        {
            _surfaceNormal = hit.normal;

            // TREAT vertical wall as grounded to “stick” 
            if (!_isGrounded)
            {
                _isGrounded = true;
                _canJump = true;
            }
        }
        // 2) Else check for ground beneath
        else if (Physics.Raycast(origin, -transform.up, out hit, raycastDistance, wallMask))
        {
            _surfaceNormal = hit.normal;
            if (!_isGrounded)
            {
                _isGrounded = true;
                _canJump = true;
            }
        }
        else
        {
            // No wall or floor → in the air
            _surfaceNormal = Vector3.up;
            _isGrounded = false;
        }

        // Smoothly rotate spider so “up” aligns with _surfaceNormal
        Quaternion targetRot = Quaternion.FromToRotation(transform.up, _surfaceNormal) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);
    }

    /// <summary>
    /// Reads input (WASD), projects movement onto the current surface, and updates velocity.
    /// </summary>
    private void HandleMovement()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // Project camera’s forward/right onto the surface
        Vector3 camForward = Vector3.ProjectOnPlane(cameraTransform.forward, _surfaceNormal).normalized;
        Vector3 camRight   = Vector3.ProjectOnPlane(cameraTransform.right, _surfaceNormal).normalized;
        Vector3 moveDir    = (camForward * v + camRight * h).normalized;

        // Preserve the “vertical” component along surface normal
        Vector3 normalVel = Vector3.Project(_rb.linearVelocity, _surfaceNormal);

        Vector3 desiredVelocity = moveDir * speed + normalVel;

        // If input exists, do a spherecast to prevent clipping into walls
        if (moveDir.sqrMagnitude > 0.001f)
        {
            float checkDistance = speed * Time.fixedDeltaTime + 0.1f;
            Vector3 castOrigin = transform.position;
            float sphereRadius = 0.05f;

            if (Physics.SphereCast(castOrigin, sphereRadius, moveDir, out RaycastHit hit, checkDistance, wallMask))
            {
                float verticalDot = Mathf.Abs(Vector3.Dot(hit.normal, Vector3.up));
                if (verticalDot < 0.7f) // nearly vertical wall
                {
                    // Block horizontal movement into the wall
                    desiredVelocity = normalVel;
                }
                // If it was floor or ceiling, let it pass
            }
        }
        else
        {
            // No input → zero horizontal movement
            desiredVelocity = normalVel;
        }

        _rb.linearVelocity = desiredVelocity;
    }

    /// <summary>
    /// Applies gravity: if grounded (wall or floor), push into surface; otherwise, pull down.
    /// Because hitting a wall now sets _isGrounded = true, the spider will “stick” instead of sliding off.
    /// </summary>
    private void ApplyCustomGravity()
    {
        if (_isGrounded)
        {
            // Push into the surface (wall/floor) to keep the spider stuck
            _rb.AddForce(-_surfaceNormal * (gravityMultiplier * 2f), ForceMode.Acceleration);
        }
        else
        {
            // In the air → normal downward pull
            _rb.AddForce(Physics.gravity * gravityMultiplier, ForceMode.Acceleration);
        }
    }

    /// <summary>
    /// If on a wall or floor (_isGrounded), allow jump along normal. Then skip jumping until we land again.
    /// </summary>
    private void HandleJumpInput()
    {
        if (!_isGrounded || !_canJump)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Clear any “up/down” velocity along the surface normal
            Vector3 horizontalVel = Vector3.ProjectOnPlane(_rb.linearVelocity, _surfaceNormal);
            _rb.linearVelocity = horizontalVel;

            // Impulse along _surfaceNormal
            _rb.AddForce(_surfaceNormal * jumpForce, ForceMode.Impulse);

            _canJump = false;
            _recentWallJump = true;
        }
    }
}
