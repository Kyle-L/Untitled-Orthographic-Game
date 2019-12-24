using UnityEngine;

public class Controller : MonoBehaviour {

    // move and rotate the Marker gameobject in the prefab to
    // adjust the clipping plane location and angle

    public GameObject prefab;
    Transform marker;
    GameObject instance;
    Clipper clipper;

    void Start() {
        clipper = new Clipper();
        instance = (GameObject)Object.Instantiate(prefab);

        // Add a second instance of the prefab to prove the
        // clipping materials are not affecting the shared materials.
        Object.Instantiate(prefab, new Vector3(0f, 3f, 0f), Quaternion.identity);
    }

    void Update() {
        marker = instance.transform.Find("Marker");


        // Clipping offset from centerpoint along the normal.
        // Prefab objects have a thickness of 1 in this demo
        // so if the marker is at the rear edge (nearest the
        // camera, z=-0.5), an offset of 0.9f is 90% visible.
        float sectionOffset = 0.5f;

        // Apply the clipping materials
        clipper.Clip(true, prefab, instance);

        // The normal can be defined two ways; this demo uses the second method.
        // 1. set the Euler angles in a Vector3 and convert that to a Quaternion,
        //    then multiply the resulting rotation by a Vector3 unit direction
        //    which represents the normal at (0,0,0) degrees of rotation, or...
        // 2. leave the Euler angles at (0,0,0) which is the same thing as
        //    Quaternion.identity and use the transform of a gameobject that has
        //    been rotated to point along the desired normal.
        Vector4 sectionPlane = Quaternion.identity * marker.forward;

        // Centerpoint of clipping plane (called SectionPoint)
        Vector4 sectionPoint = new Vector4(marker.position.x, marker.position.y, marker.position.z, 1f);

        // Apply the changes
        clipper.SetClippingPlane(sectionPoint, sectionPlane, sectionOffset);

    }
}
