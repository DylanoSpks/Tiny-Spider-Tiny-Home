using UnityEngine;

public class SpiderCameraController : MonoBehaviour
{
    public Transform target;
    public float distance = 6f;
    public float heightOffset = 1f;
    public float sensitivityX = 3f;
    public float sensitivityY = 2f;
    public float minY = -40f;
    public float maxY = 80f;
    public LayerMask collisionMask;

    private float _rotationX;
    private float _rotationY;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        _rotationX = angles.y;
        _rotationY = angles.x;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (target == null) return;

        _rotationX += Input.GetAxis("Mouse X") * sensitivityX;
        _rotationY -= Input.GetAxis("Mouse Y") * sensitivityY;
        _rotationY = Mathf.Clamp(_rotationY, minY, maxY);

        Quaternion rotation = Quaternion.Euler(_rotationY, _rotationX, 0);
        Vector3 desiredPosition = target.position - (rotation * Vector3.forward * distance) + Vector3.up * heightOffset;

        // Raycast to avoid clipping through objects
        RaycastHit hit;
        Vector3 targetCenter = target.position + Vector3.up * heightOffset;

        if (Physics.Linecast(targetCenter, desiredPosition, out hit, collisionMask))
        {
            desiredPosition = hit.point;
        }

        transform.position = desiredPosition;
        transform.rotation = rotation;
    }
}