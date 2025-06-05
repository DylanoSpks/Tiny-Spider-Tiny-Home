using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(Rigidbody))]
public class GrapplingHook : MonoBehaviour
{
    [Header("Grapple Settings")] [Tooltip("Maximum distance the grapple can reach")]
    public float maxGrappleDistance = 25f;

    [Tooltip("Layers the web can attach to")]
    public LayerMask grappleMask;

    [Tooltip("Delay (seconds) before destroying joint when detaching")]
    public float detachDelay = 0.1f;

    [Header("Joint Settings")] [Tooltip("Spring force of the joint")]
    public float jointSpring = 100f;

    [Tooltip("Damping of the joint")] public float jointDamping = 5f;
    [Tooltip("Mass scale for the joint")] public float jointMassScale = 2f;

    [Header("References")] [Tooltip("Camera transform to shoot ray from")]
    public Transform cameraTransform;

    private LineRenderer _lineRenderer;
    private SpringJoint _springJoint;
    private Vector3 _grapplePoint;
    private bool _isGrappling;

    void Start()
    {
        GetComponent<Rigidbody>();
        _lineRenderer = GetComponent<LineRenderer>();

        // Ensure the LineRenderer has exactly two positions
        _lineRenderer.positionCount = 2;
        _lineRenderer.enabled = false;

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        if (!PauseMenu.isPaused)
        {
            HandleInput();

            if (_isGrappling)
            {
                DrawRope();
            }
        }
    }

    private void HandleInput()
    {
        // 1) Right-click to shoot
        if (Input.GetMouseButtonDown(1))
        {
            StartGrapple();
        }

        // 2) Release right-click to detach
        if (Input.GetMouseButtonUp(1) && _isGrappling)
        {
            StopGrapple();
        }
    }

    private void StartGrapple()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, maxGrappleDistance, grappleMask))
        {
            _grapplePoint = hit.point;
            _isGrappling = true;

            // Create a SpringJoint to latch onto the hit point
            _springJoint = gameObject.AddComponent<SpringJoint>();
            _springJoint.autoConfigureConnectedAnchor = false;
            _springJoint.connectedAnchor = _grapplePoint;

            float distanceFromPoint = Vector3.Distance(transform.position, _grapplePoint);

            // Tweak these to adjust “rope slack”
            _springJoint.maxDistance = distanceFromPoint * 0.8f;
            _springJoint.minDistance = distanceFromPoint * 0.25f;
            _springJoint.spring = jointSpring;
            _springJoint.damper = jointDamping;
            _springJoint.massScale = jointMassScale;

            // Enable and draw the rope once
            _lineRenderer.enabled = true;
            _lineRenderer.SetPosition(0, transform.position);
            _lineRenderer.SetPosition(1, _grapplePoint);

            // DISABLE SpiderMovement entirely while swinging
            var spiderMove = GetComponent<SpiderMovement>();
            if (spiderMove != null)
            {
                spiderMove.disableMovement = true;
            }
        }
    }

    private void DrawRope()
    {
        if (_springJoint == null) return;

        // Update rope endpoints each frame
        _lineRenderer.SetPosition(0, transform.position);
        _lineRenderer.SetPosition(1, _grapplePoint);
    }

    private void StopGrapple()
    {
        if (_springJoint != null)
        {
            Destroy(_springJoint, detachDelay);
        }

        _lineRenderer.enabled = false;
        _isGrappling = false;

        // RE‐ENABLE SpiderMovement when we let go
        var spiderMove = GetComponent<SpiderMovement>();
        if (spiderMove != null)
        {
            spiderMove.disableMovement = false;
        }
    }
}
