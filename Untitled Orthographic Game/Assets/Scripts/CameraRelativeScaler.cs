using UnityEngine;

public class CameraRelativeScaler : MonoBehaviour {
    public Camera cam;
    private Vector3 initialScale;
    public float referenceSize;

    // set the initial scale, and setup reference camera
    void Start() {
        // if no specific camera, grab the default camera
        if (cam == null) {
            cam = Camera.main;
        }

        // record initial scale, use this as a basis
        initialScale = transform.localScale / referenceSize;
    }

    // scale object relative to distance from camera plane
    void Update() {
        transform.localScale = cam.orthographicSize * initialScale;
    }
}