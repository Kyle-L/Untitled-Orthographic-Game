using UnityEngine;
using System.Collections;

public class CameraOrbit : MonoBehaviour {

    public float orbitDegPerSec = 30f;

    void Update()
    {
        float orbit = Mathf.Lerp(0, orbitDegPerSec, Time.deltaTime);
        transform.RotateAround(Vector3.zero, Vector3.up, orbit);
        transform.LookAt(Vector3.zero);
    }

}
