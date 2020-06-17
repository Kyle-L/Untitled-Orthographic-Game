using UnityEngine;

public class CameraRelativeScaler : MonoBehaviour {
    public Camera cam;
    private Vector3 initialScale;
    [Range(1.000001f, 25)]
    public float fallOff = 1.00001f;

    // set the initial scale, and setup reference camera
    void Start() {
        // if no specific camera, grab the default camera
        if (cam == null) {
            cam = Camera.main;
        }

        // record initial scale, use this as a basis
        initialScale = transform.localScale / cam.orthographicSize;
    }

    // scale object relative to distance from camera plane
    void Update() {
        if (!gameObject.activeSelf) {
            return;
        }
        transform.localScale = initialScale * (Mathf.Log(cam.orthographicSize + 1, fallOff) + 1);
    }
}