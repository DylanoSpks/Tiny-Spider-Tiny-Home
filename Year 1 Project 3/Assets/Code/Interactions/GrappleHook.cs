using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GrappleHook : MonoBehaviour
{
    [Header("Grapple Settings")]
    public float maxDistance = 20f;
    public LayerMask grappleMask;
    public float springStrength = 100f;
    public float springDamping = 5f;
    public float ropeWidth = 0.02f;
    public Material ropeMaterial;

    private SpringJoint _joint;
    private LineRenderer _line;
    private Camera _cam;
    private Vector3 _anchorPoint;

    void Start()
    {
        _cam = Camera.main;
        _line = gameObject.AddComponent<LineRenderer>();
        _line.enabled = false;
        _line.startWidth = _line.endWidth = ropeWidth;
        _line.material = ropeMaterial;
        _line.positionCount = 2;
    }

    void Update()
    {
        // Start grapple
        if (Input.GetMouseButtonDown(1)) // Right‐click (or change to your input)
            TryStartGrapple();

        // Release grapple
        if (Input.GetMouseButtonUp(1))
            StopGrapple();

        // Update rope visual
        if (_joint)
        {
            _line.SetPosition(0, transform.position);
            _line.SetPosition(1, _anchorPoint);
        }
    }

    void TryStartGrapple()
    {
        Ray ray = new Ray(_cam.transform.position, _cam.transform.forward);
        if (Physics.Raycast(ray, out var hit, maxDistance, grappleMask))
        {
            _anchorPoint = hit.point;

            // Create a spring joint
            _joint = gameObject.AddComponent<SpringJoint>();
            _joint.autoConfigureConnectedAnchor = false;
            _joint.connectedAnchor = _anchorPoint;

            float dist = Vector3.Distance(transform.position, _anchorPoint);
            _joint.maxDistance = dist * 0.8f;
            _joint.minDistance = dist * 0.25f;

            _joint.spring = springStrength;
            _joint.damper = springDamping;
            _joint.massScale = 4.5f;

            // Enable rope
            _line.enabled = true;
        }
    }

    void StopGrapple()
    {
        if (_joint)
        {
            Destroy(_joint);
            _line.enabled = false;
        }
    }
}

