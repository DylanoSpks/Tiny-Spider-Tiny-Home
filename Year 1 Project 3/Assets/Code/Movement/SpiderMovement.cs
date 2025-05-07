using System;
using UnityEngine;

public class SpiderMovement : MonoBehaviour
{
    public float speed = 15f;
    public float rotationSpeed = 10f;
    public float raycastDistance = 1.5f;
    public LayerMask wallMask;
    public float jumpForce = 8f;
    public float jumpCooldown = 0.5f;
    public float gravityMultiplier = 2f;
    public float wallJumpForce = 14f;
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
        {
            cameraTransform = Camera.main.transform;
        }
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

        Vector3 moveDir = cameraTransform.forward * vertical + cameraTransform.right * horizontal;
        moveDir = Vector3.ProjectOnPlane(moveDir, _surfaceNormal).normalized;

        if (moveDir.sqrMagnitude > 0.01f)
        {
            _rb.MovePosition(_rb.position + moveDir * (speed * Time.deltaTime));
        }
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

        if (Physics.Raycast(origin, cameraTransform.forward, out hit, raycastDistance, wallMask))
        {
            _targetNormal = hit.normal;
            _isClimbing = true;
        }
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

        _surfaceNormal = Vector3.Slerp(_surfaceNormal, _targetNormal, Time.deltaTime * rotationSpeed);
        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, _surfaceNormal) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    void HandleGravity()
    {
        if (_isClimbing && !_recentWallJump)
        {
            _rb.AddForce(-_surfaceNormal * (gravityMultiplier * 3f), ForceMode.Acceleration);
        }
        else
        {
            _rb.AddForce(Physics.gravity * gravityMultiplier, ForceMode.Acceleration);
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    [Obsolete("Obsolete")]
    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _canJump)
        {
            Vector3 jumpDirection;

            if (_isClimbing && !_isGrounded)
            {
                // Jump away from the surface in the direction the camera is pointing, plus slight normal push
                jumpDirection = (cameraTransform.forward + _surfaceNormal * 0.5f).normalized;
                _targetNormal = Vector3.up;
                _isClimbing = false;
                _recentWallJump = true;
                Invoke(nameof(ClearRecentWallJump), 0.5f);
            }
            else
            {
                // Normal jump
                jumpDirection = (_surfaceNormal * 0.6f + cameraTransform.forward * 0.4f).normalized;
            }

            _rb.velocity = Vector3.zero; // Reset velocity before jumping
            _rb.AddForce(jumpDirection * wallJumpForce, ForceMode.Impulse);
            _canJump = false;
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    void ResetJump()
    {
        _canJump = true;
    }

    void ClearRecentWallJump()
    {
        _recentWallJump = false;
    }
}
