using UnityEngine;

public class SpiderCameraController : MonoBehaviour
{
    [Header("Camera Orbit Settings")]
    public Transform target;
    public float distance = 3f;
    public float heightOffset = 0.3f;
    public float sensitivityX = 1f;
    public float sensitivityY = 1f;
    public float minY = -80f;
    public float maxY = 80f;
    public LayerMask collisionMask;

    [Header("Spider Facing Settings")]
    public Transform spiderModel;
    [Tooltip("Degrees per second speed for smooth normal interpolation (higher = faster detection).")]
    public float spiderTurnSpeed = 600f;
    public LayerMask spiderSurfaceMask;
    public float spiderRaycastDistance = 1.5f;
    [Tooltip("SphereCast radius for detecting surface normals.")]
    public float sphereCastRadius = 0.2f;
    [Tooltip("Offset amount for the corner raycasts.")]
    public float raycastOffset = 0.05f;

    private float _rotationX;
    private float _rotationY;
    private Vector3 _smoothedNormal = Vector3.up;

    void Start()
    {
        if (target)
        {
            Vector3 dir = target.position - transform.position;
            if (dir.sqrMagnitude > 0.001f)
            {
                Quaternion init = Quaternion.LookRotation(dir);
                _rotationX = init.eulerAngles.y;
                _rotationY = init.eulerAngles.x;
            }
        }
    }

    void Update()
    {
        if (PauseMenu.IsPaused)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            if (spiderModel != null)
                UpdateSpiderRotation();
        }
    }

    void LateUpdate()
    {
        if (!PauseMenu.IsPaused)
            RotateCamera();
    }

    private void RotateCamera()
    {
        if (!target) return;

        // Read mouse input
        _rotationX += Input.GetAxis("Mouse X") * sensitivityX;
        _rotationY -= Input.GetAxis("Mouse Y") * sensitivityY;
        _rotationY = Mathf.Clamp(_rotationY, minY, maxY);

        // Compute camera rotation
        Quaternion camRot = Quaternion.Euler(_rotationY, _rotationX, 0);

        // Pivot using spider orientation
        Vector3 pivot = spiderModel != null ? spiderModel.position : target.position;
        Vector3 upVector = spiderModel != null ? spiderModel.up : Vector3.up;

        // Desired camera position
        Vector3 desiredPos = pivot - (camRot * Vector3.forward * distance) + (upVector * heightOffset);

        // Collision check
        Vector3 rayStart = pivot + upVector * heightOffset;
        if (Physics.Linecast(rayStart, desiredPos, out RaycastHit hit, collisionMask))
        {
            desiredPos = hit.point + hit.normal * 0.1f;
        }

        transform.position = desiredPos;
        transform.rotation = camRot;
    }

    private void UpdateSpiderRotation()
    {
        // Get averaged surface normal
        Vector3 avgNormal = GetAverageSurfaceNormal();

        // Smoothly interpolate toward the target normal (higher spiderTurnSpeed = faster detection)
        _smoothedNormal = Vector3.Slerp(_smoothedNormal, avgNormal, Time.deltaTime * spiderTurnSpeed);

        // Project camera forward onto surface
        Vector3 camForward = transform.forward;
        Vector3 forwardOnSurface = Vector3.ProjectOnPlane(camForward, _smoothedNormal).normalized;
        if (forwardOnSurface.sqrMagnitude < 0.001f) return;

        // Compute target rotation for spider
        Quaternion targetRot = Quaternion.LookRotation(forwardOnSurface, _smoothedNormal);

        // Apply rotation with tween speed
        spiderModel.rotation = Quaternion.RotateTowards(
            spiderModel.rotation,
            targetRot,
            spiderTurnSpeed * Time.deltaTime
        );
    }

    private Vector3 GetAverageSurfaceNormal()
    {
        Vector3 sum = Vector3.zero;
        int count = 0;
        RaycastHit hit;

        // Center spherecast
        if (Physics.SphereCast(spiderModel.position, sphereCastRadius, -spiderModel.up,
                               out hit, spiderRaycastDistance, spiderSurfaceMask))
        {
            sum += hit.normal;
            count++;
        }

        // Four corner raycasts
        Vector3[] offsets = {
            spiderModel.forward * raycastOffset,
            -spiderModel.forward * raycastOffset,
            spiderModel.right * raycastOffset,
            -spiderModel.right * raycastOffset
        };

        foreach (var off in offsets)
        {
            Vector3 origin = spiderModel.position + off;
            if (Physics.Raycast(origin, -spiderModel.up, out hit, spiderRaycastDistance, spiderSurfaceMask))
            {
                sum += hit.normal;
                count++;
            }
        }

        return (count > 0) ? (sum / count).normalized : Vector3.up;
    }
}
