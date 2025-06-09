using UnityEngine;

public class OrbitCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 2f, 0f);
    public float defaultDistance = 5f;
    public float zoomSpeed = 2f;
    public float minZoom = 2f;
    public float maxZoom = 8f;

    public float mouseSensitivity = 3f;
    public float rotationSmoothTime = 0.1f;
    public float cameraCollisionSmoothTime = 0.05f;

    public float minYAngle = -35f;
    public float maxYAngle = 70f;

    public float collisionRadius = 0.3f;
    public LayerMask collisionLayers;

    private float yaw;
    private float pitch;

    private float currentDistance;
    private float desiredDistance;
    private float distanceVelocity;

    private Vector3 currentRotation;
    private Vector3 rotationSmoothVelocity;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;

        desiredDistance = defaultDistance;
        currentDistance = defaultDistance;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        HandleInput();
        UpdateCameraPosition();
    }

    void HandleInput()
    {
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, minYAngle, maxYAngle);

        // Scroll zoom
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            desiredDistance -= scroll * zoomSpeed;
            desiredDistance = Mathf.Clamp(desiredDistance, minZoom, maxZoom);
        }
    }

    void UpdateCameraPosition()
    {
        Vector3 targetRotation = new Vector3(pitch, yaw);
        currentRotation = Vector3.SmoothDamp(currentRotation, targetRotation, ref rotationSmoothVelocity, rotationSmoothTime);
        transform.eulerAngles = currentRotation;

        Vector3 dir = Quaternion.Euler(currentRotation) * Vector3.back;
        Vector3 origin = target.position + offset;

        float targetDistance = desiredDistance;

        if (Physics.SphereCast(origin, collisionRadius, dir, out RaycastHit hit, desiredDistance, collisionLayers))
        {
            targetDistance = hit.distance - 0.1f;
        }

        currentDistance = Mathf.SmoothDamp(currentDistance, targetDistance, ref distanceVelocity, cameraCollisionSmoothTime);

        transform.position = origin + dir * currentDistance;
    }
}
