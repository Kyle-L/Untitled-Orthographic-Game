using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

/// <summary>
/// Controls the camera.
/// </summary>
[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour {
    [HideInInspector]
    public static CameraController instance;

    //[HideInInspector]
    public GameObject trackingObject;

    private Camera _camera;
    public Camera uiCamera;

    [Header("Camera Movement Settings")]
    public float cameraRotationSmoothSpeed = 2.5f;
    public float cameraPositionSmoothSpeed = 2.5f;

    [Header("Camera Position Settings")]
    public Vector3 trackingOffset;
    public float objectOffset;

    private Vector3 cameraTarget;

    public float startHeight = 15;
    private float curHeight = 15;
    public float cameraMinHeight = 0;
    public float cameraMaxHeight = 50;

    public float startSize = 5;
    public float cameraMinSize = 1;
    public float cameraMaxSize = 7;

    public float cameraRadius = 15;
    public float cameraStartAngle = 15;
    private float curAngle;

    [Header("Camera Speed")]
    public float cameraMoveSpeed = 1;
    public float cameraHeightSpeed = 1;
    public float cameraRotateSpeed = 1;
    public float cameraZoomSpeed = 2;

    //Controls
    private bool control = true;

    void Awake() {
        instance = this;

        _camera = GetComponent<Camera>();

        SetAngle(cameraStartAngle);
        SetSize(startSize);
        SetHeight(startHeight);
    }

    void Start() {
    }

    void Update() {
        cameraTarget = trackingObject.transform.position;
        if (control) {
            // Processes user input for rotation and height.
            if (CrossPlatformInputManager.GetButton("Fire2")) {
                // Get user input
                float axisX = Input.GetAxis("Axis X");
                float axisY = Input.GetAxis("Axis Y");

                // Apply the user input to the current angle and height.
                curAngle += axisX * cameraRotateSpeed * Time.deltaTime;
                curHeight -= axisY * cameraHeightSpeed * Time.deltaTime;

                // Clamps the camera's height.
                curHeight = Mathf.Clamp(curHeight, cameraMinHeight, cameraMaxHeight);
            }

            // Processes user input for movement.
            //float horizontal = Input.GetAxis("Horizontal");
            //float vertical = Input.GetAxis("Vertical");

            // Get camera forward and right vectors:
            Vector3 forward = _camera.transform.forward;
            Vector3 right = _camera.transform.right;

            // Project forward and right vectors on the horizontal plane (y = 0).
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            // Get the direction in the world space where the user wants to move.
            //Vector3 desiredMoveDirection = forward * vertical + right * horizontal;

            // Apply the movement to the camera's target look position.
            //cameraTarget -= desiredMoveDirection * cameraMoveSpeed * Time.deltaTime;

            // Processes user input for camera size.
            _camera.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * cameraZoomSpeed;
            _camera.orthographicSize = Mathf.Clamp(_camera.orthographicSize, cameraMinSize, cameraMaxSize);
        }

        //Creates the angle used to rotate the camera.
        Vector3 v = Quaternion.AngleAxis(curAngle, Vector3.up) * new Vector3(cameraRadius, curHeight, 0); //Center angle relative to player.

        //Sets the camera position.
        transform.position = v + trackingOffset + cameraTarget;

        //Rotates the camera to look at the playerController.
        transform.LookAt(trackingOffset + cameraTarget);

        if (uiCamera != null) {
            uiCamera.transform.position = _camera.transform.position;
            uiCamera.transform.rotation = _camera.transform.rotation;
            uiCamera.orthographicSize = _camera.orthographicSize;
        }
    }

    public void SetAngle(float aAngle) {
        curAngle = aAngle;
    }

    public void SetSize(float aSize) {
        _camera.orthographicSize = Mathf.Clamp(aSize, cameraMinSize, cameraMaxSize); ;
    }

    public void SetHeight(float aHeight) {
        curHeight = aHeight;
    }

    public void SetControl(bool canControl) {
        control = canControl;
    }
}
