using UnityEngine;

public class Face : MonoBehaviour {

    public Transform lookTransform;

    void Update() {
        transform.LookAt(lookTransform, lookTransform.up);
    }

}
