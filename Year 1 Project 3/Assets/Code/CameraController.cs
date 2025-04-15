using UnityEngine;

public class SpiderCameraController : MonoBehaviour
{
    public Transform target; // Assign your spider object here
    public float smoothSpeed = 0.125f;
    public Vector3 offset = new Vector3(0, 5, -10); // Adjust this based on your desired camera angle
    public float mouseSensitivity = 2.0f;

    private float _yaw;
    private float _pitch;

    private void LateUpdate()
    {
        if (!target) return;

        // Get mouse inputs
        _yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        _pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        _pitch = Mathf.Clamp(_pitch, -35f, 60f); // Limit camera looking up/down

        // Rotate the camera based on mouse movement
        Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0);
        Vector3 desiredPosition = target.position + rotation * offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // Make the camera look at the spider
        transform.LookAt(target.position);
    }
}
