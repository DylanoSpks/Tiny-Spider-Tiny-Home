using UnityEngine;

public class SpiderCameraController : MonoBehaviour
{
    [Header("Camera Orbit Settings")]
    public Transform target;            // your ball/spider parent
    public float distance = 3f;
    public float heightOffset = 0.3f;
    public float sensitivityX = 1f;
    public float sensitivityY = 1f;
    public float minY = -80f;
    public float maxY = 80f;
    public LayerMask collisionMask;     // for the camera clip‐avoidance

    [Header("Spider Facing Settings")]
    public Transform spiderModel;       // drag your spider child here
    public float spiderTurnSpeed = 1440f; // degrees per second (increase for faster snap)
    public LayerMask spiderSurfaceMask;   // layers for spider to stick to
    public float spiderRaycastDistance = 1.5f; // how far to check for surface

    private float _rotationX;
    private float _rotationY;

    void Start()
    {
        // Initialize camera rotation based on current position relative to target
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
        // Cursor and pause logic unchanged
        if (PauseMenu.IsPaused)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible   = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible   = false;
        }

        // Update spider rotation each frame when unpaused
        if (!PauseMenu.IsPaused && spiderModel != null)
            UpdateSpiderRotation();
    }

    void LateUpdate()
    {
        if (!PauseMenu.IsPaused)
            RotateCamera();   // existing orbit + collision code
    }

      
    private void RotateCamera()
    {
        if (!PauseMenu.IsPaused)
        {
            if (!target) return;

            _rotationX += Input.GetAxis("Mouse X") * sensitivityX;
            _rotationY -= Input.GetAxis("Mouse Y") * sensitivityY;
            _rotationY = Mathf.Clamp(_rotationY, minY, maxY);

            Quaternion rotation = Quaternion.Euler(_rotationY, _rotationX, 0);
            Vector3 desiredPosition =
                target.position - (rotation * Vector3.forward * distance) + Vector3.up * heightOffset;

            // Raycast to avoid clipping through objects
            RaycastHit hit;
            Vector3 targetCenter = target.position + target.up * heightOffset;

            if (Physics.Linecast(targetCenter, desiredPosition, out hit, collisionMask))
            {
                const float wallOffset = 0.1f;
                desiredPosition = hit.point + (hit.normal * wallOffset);
            }

            transform.position = desiredPosition;
            transform.rotation = rotation;
        }
    }

    private void UpdateSpiderRotation()
    {
        // Try raycast along spider's local down to find surface normal
        RaycastHit groundHit;
        Vector3 downDir = -spiderModel.up;
        bool hitSurface = Physics.Raycast(spiderModel.position, downDir, out groundHit, spiderRaycastDistance, spiderSurfaceMask);

        // If no hit on down, try opposite direction (for ceiling surfaces)
        if (!hitSurface)
        {
            Vector3 upDir = spiderModel.up;
            hitSurface = Physics.Raycast(spiderModel.position, upDir, out groundHit, spiderRaycastDistance, spiderSurfaceMask);
        }

        if (!hitSurface) return;

        Vector3 surfaceNormal = groundHit.normal;

        // Project camera forward onto surface plane
        Vector3 camF = transform.forward;
        Vector3 forwardOnSurface = Vector3.ProjectOnPlane(camF, surfaceNormal).normalized;
        if (forwardOnSurface.sqrMagnitude < 0.001f) return;

        // Build target rotation: spider's up = surfaceNormal, forward along projected vector
        Quaternion targetRot = Quaternion.LookRotation(forwardOnSurface, surfaceNormal);

        // Smooth rotate spider toward target
        spiderModel.rotation = Quaternion.RotateTowards(
            spiderModel.rotation,
            targetRot,
            spiderTurnSpeed * Time.deltaTime
        );
    }
}