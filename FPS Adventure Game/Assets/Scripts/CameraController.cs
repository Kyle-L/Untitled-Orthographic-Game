using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;

/// <summary>
/// Controls the camera.
/// </summary>
public class CameraController : MonoBehaviour {
    [HideInInspector]
    public static CameraController instance;

    //[HideInInspector]
    public GameObject trackingObject;

    private Camera camera;

    [Header("Camera Movement Settings")]
    public float cameraRotationSmoothSpeed = 2.5f;
    public float cameraPositionSmoothSpeed = 2.5f;

    [Header("Camera Position Settings")]
    public Vector3 cameraOffset;
    public float cameraHeight = 15;
    public float cameraMinHeight = 0;
    public float cameraMaxHeight = 50;
    private float curAngle = 15;
    public float cameraRadius = 15;


    [Header("Ray Cast Settings")]
    public int rayCastAngle = 120;
    public int rayCastNum = 10;
    private RaycastHit[][] lastRayCastHits;
    public RaycastHit[][] rayCastHits;

    LayerMask mask = 1 << 10;

    //Controls
    private bool Control { get; set; } = true;

    void Awake() {
        instance = this;

        camera = GetComponent<Camera>();
    }

    void Start() {
        rayCastHits = new RaycastHit[rayCastNum][];
    }

    void Update() {
        if (Control) {
            if (CrossPlatformInputManager.GetButton("Fire1")) {
                curAngle += Input.GetAxis("Axis X") * 2;
                cameraHeight -= Input.GetAxis("Axis Y");
                cameraHeight = Mathf.Clamp(cameraHeight, cameraMinHeight, cameraMaxHeight);
            }
            if (CrossPlatformInputManager.GetButton("Fire2")) {

            }

            camera.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * 2;
            camera.orthographicSize = Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, 1, 7);
        }

        //Creates all angles used for calculating the raycast 'pie'.
        List<Vector3> vList = new List<Vector3>();
        for (int i = -rayCastNum / 2; i < rayCastNum / 2; i++) {
            vList.Add(Quaternion.AngleAxis(curAngle + i * (rayCastAngle / rayCastNum), Vector3.up) * new Vector3(cameraRadius, cameraHeight, 0));
        }

        for (int i = 0; i < vList.Count; i++) {
            //Performs Linecasts.
            Debug.DrawRay(trackingObject.transform.position, new Vector3(vList[i].x, 0, vList[i].z), Color.yellow);

            Ray ray = new Ray(trackingObject.transform.position, new Vector3(transform.position.x + vList[i].x, trackingObject.transform.position.y, transform.position.z + vList[i].z) - trackingObject.transform.position);
            rayCastHits[i] = Physics.RaycastAll(ray,100, mask);
        }

        //Enables all objects previously raycasted.
        if (lastRayCastHits != null) {
            for (int i = 0; i < lastRayCastHits.Length; i++) {
                for (int i2 = 0; i2 < lastRayCastHits[i].Length; i2++) {
                    lastRayCastHits[i][i2].collider.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                }
            }
        }

        //Copies rayCastHits to lastRayCastHits.
        lastRayCastHits = new RaycastHit[rayCastHits.Length][];
        for (int i = 0; i < lastRayCastHits.Length; i++) {
            lastRayCastHits[i] = (RaycastHit[])rayCastHits[i].Clone();
        }

        //Disables all objects raycasted.
        for (int i = 0; i < rayCastHits.Length; i++) {
            for (int i2 = 0; i2 < rayCastHits[i].Length; i2++) {
                lastRayCastHits[i][i2].collider.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            }
        }

        //Creates the angle used to rotate the camera.
        Vector3 v = Quaternion.AngleAxis(curAngle, Vector3.up) * new Vector3(cameraRadius, cameraHeight, 0); //Center angle relative to player.

        transform.position = trackingObject.transform.position + v + cameraOffset; //Sets the camera position.
        transform.LookAt(trackingObject.transform.position + cameraOffset); //Rotates the camera to look at the playerController.

    }

    public void SetAngle(float aAngle) {
        curAngle = aAngle;
    }
}
