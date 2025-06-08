using UnityEngine;

public class SpiderFaceCamera : MonoBehaviour
{
    [Tooltip("Drag in the camera that follows your spider")]
    public Transform cameraTransform;

    [Tooltip("Degrees per second the spider can turn to match camera")]
    public float turnSpeed = 720f;

    void Update()
    {
        if (cameraTransform == null) return;

        // 1) Get the camera's forward, but keep it horizontal
        Vector3 flatForward = cameraTransform.forward;
        flatForward.y = 0f;
        if (flatForward.sqrMagnitude < 0.001f) return;

        // 2) Build the target world‐space rotation
        Quaternion targetRot = Quaternion.LookRotation(flatForward, Vector3.up);

        // 3) Smoothly rotate toward that direction
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation, 
            targetRot, 
            turnSpeed * Time.deltaTime
        );
    }
}