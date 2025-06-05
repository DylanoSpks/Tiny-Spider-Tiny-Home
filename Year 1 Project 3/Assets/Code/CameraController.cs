using System;
using UnityEngine;

public class SpiderCameraController : MonoBehaviour
{
    public Transform target;
    public float distance = 3f;
    public float heightOffset = 0.3f;
    public float sensitivityX = 1f;
    public float sensitivityY = 1f;
    public float minY = -80f;
    public float maxY = 80f;
    public LayerMask collisionMask;

    private float _rotationX;
    private float _rotationY;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        _rotationX = angles.y;
        _rotationY = angles.x;

        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }

    //This update function should fix the cursor problem in a roundabout way

    private void Update()
    {
        if (PauseMenu.isPaused)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    
    void LateUpdate()
    {
        if (!PauseMenu.isPaused)
        {
            RotateCamera();
        }
    }

    private void RotateCamera()
    {
        if (!PauseMenu.isPaused)
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
}