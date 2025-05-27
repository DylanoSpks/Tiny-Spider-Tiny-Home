using System;
using UnityEngine;

public class SpiderMovement : MonoBehaviour
{
    public float speed = 1.5f;
    public float rotationSpeed = 1f;
    public float raycastDistance = 0.8f;
    public LayerMask wallMask;           // Should include "Crawlable" layer
    public float jumpForce = 4f;
    public float jumpCooldown = 0.5f;
    public float gravityMultiplier = 0.8f;
    public float wallJumpForce = 6f;
    public Transform cameraTransform;

    private Rigidbody _rb;
    private Vector3 _surfaceNormal = Vector3.up;
    private bool _canJump = true;
    private Vector3 _targetNormal = Vector3.up;
    private bool _isClimbing;
    private bool _isGrounded = true;
    private bool _recentWallJump;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;
        _rb.constraints = RigidbodyConstraints.FreezeRotation;

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    [Obsolete("Obsolete")]
    void Update()
    {
        HandleSurfaceDetection();
        HandleMovement();
        HandleGravity();
        HandleJump();
    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Camera-relative movement
        Vector3 moveDir = cameraTransform.forward * vertical + cameraTransform.right * horizontal;
        moveDir = Vector3.ProjectOnPlane(moveDir, _surfaceNormal).normalized;

        if (moveDir.sqrMagnitude > 0.01f)
            _rb.MovePosition(_rb.position + moveDir * (speed * Time.deltaTime));
    }

    void HandleSurfaceDetection()
    {
        if (_recentWallJump)
        {
            _isClimbing = false;
            _isGrounded = false;
            _targetNormal = Vector3.up;
            return;
        }

        RaycastHit hit;
        Vector3 origin = transform.position;
        _isClimbing = false;
        _isGrounded = false;

        // Check forward for wall
        if (Physics.Raycast(origin, cameraTransform.forward, out hit, raycastDistance, wallMask))
        {
            _targetNormal = hit.normal;
            _isClimbing = true;
        }
        // Check downward for ground
        else if (Physics.Raycast(origin, -transform.up, out hit, raycastDistance, wallMask))
        {
            _targetNormal = hit.normal;
            _isClimbing = true;
            _isGrounded = true;
        }
        else
        {
            _targetNormal = Vector3.up;
        }

        // Smoothly align surface normal
        _surfaceNormal = Vector3.Slerp(_surfaceNormal, _targetNormal, Time.deltaTime * rotationSpeed);
        Quaternion targetRot = Quaternion.FromToRotation(transform.up, _surfaceNormal) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);
    }

    void HandleGravity()
    {
        if (_isClimbing && !_recentWallJump)
            _rb.AddForce(-_surfaceNormal * (gravityMultiplier * 0.8f), ForceMode.Acceleration);
        else
            _rb.AddForce(Physics.gravity * gravityMultiplier, ForceMode.Acceleration);
    }

    [Obsolete("Obsolete")]
    void HandleJump()
    {
        // Only allow jump when on ground or crawling
        if (Input.GetKeyDown(KeyCode.Space) && _canJump && (_isGrounded || _isClimbing))
        {
            Vector3 jumpDirection;

            if (_isClimbing && !_isGrounded)
            {
                // Wall jump: push off wall towards camera forward
                jumpDirection = (cameraTransform.forward + _surfaceNormal * 0.5f).normalized;
                _targetNormal      = Vector3.up;
                _isClimbing        = false;
                _recentWallJump    = true;
                Invoke(nameof(ClearRecentWallJump), 0.5f);
            }
            else
            {
                // Regular jump
                jumpDirection = (_surfaceNormal * 0.6f + cameraTransform.forward * 0.4f).normalized;
            }

            _rb.velocity = Vector3.zero;
            _rb.AddForce(jumpDirection * wallJumpForce, ForceMode.Impulse);
            _canJump = false;
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    void ResetJump() => _canJump = true;
    void ClearRecentWallJump() => _recentWallJump = false;
}
