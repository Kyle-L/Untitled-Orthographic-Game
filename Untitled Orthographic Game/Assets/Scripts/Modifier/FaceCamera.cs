using UnityEngine;

public class FaceCamera : MonoBehaviour {

    void Update() {
        transform.LookAt(Camera.main.transform, Camera.main.transform.up);
    }

}
