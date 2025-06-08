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
    public float spiderTurnSpeed = 6000f; 
    public LayerMask spiderSurfaceMask;   
    public float spiderRaycastDistance = 1f; 
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
            Cursor.visible   = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible   = false;

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

        _rotationX += Input.GetAxis("Mouse X") * sensitivityX;
        _rotationY -= Input.GetAxis("Mouse Y") * sensitivityY;
        _rotationY = Mathf.Clamp(_rotationY, minY, maxY);

        Quaternion rotation = Quaternion.Euler(_rotationY, _rotationX, 0);
        Vector3 desiredPos =
            target.position - (rotation * Vector3.forward * distance) + Vector3.up * heightOffset;

        // camera collision
        RaycastHit hit;
        Vector3 targetCenter = target.position + target.up * heightOffset;
        if (Physics.Linecast(targetCenter, desiredPos, out hit, collisionMask))
        {
            desiredPos = hit.point + hit.normal * 0.1f;
        }

        transform.position = desiredPos;
        transform.rotation = rotation;
    }

    private void UpdateSpiderRotation()
    {
        // 1) get averaged surface normal
        Vector3 avgNormal = GetAverageSurfaceNormal();
        // 2) smooth toward it
        _smoothedNormal = Vector3.Slerp(_smoothedNormal, avgNormal, Time.deltaTime * (spiderTurnSpeed / 360f));
        
        // 3) build forward projected onto surface plane
        Vector3 camForward = transform.forward;
        Vector3 forwardOnSurface = Vector3.ProjectOnPlane(camForward, _smoothedNormal).normalized;
        if (forwardOnSurface.sqrMagnitude < 0.001f) 
            return;

        // 4) compute target rotation
        Quaternion targetRot = Quaternion.LookRotation(forwardOnSurface, _smoothedNormal);

        // 5) rotate spider smoothly
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

        if (count > 0)
            return (sum / count).normalized;
        else
            return Vector3.up; // fallback to flat
    }
}
