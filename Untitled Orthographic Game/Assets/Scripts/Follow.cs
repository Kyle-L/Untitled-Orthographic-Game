using UnityEngine;

public class Follow : MonoBehaviour {
    public Transform target;

    [Header("Offset")]
    public Vector3 offsetPosition;

    public float followSpeed = 5;

    public Space offsetPositionSpace = Space.Self;

    public bool lookAt = true;

    private void Update() {
        Refresh();
    }

    private void Refresh() {
        if (target == null) {
            Debug.LogWarning("Missing target ref !", this);
            return;
        }

        // compute position
        if (offsetPositionSpace == Space.Self) {
            transform.position = Vector3.Lerp(transform.position, target.TransformPoint(offsetPosition), followSpeed * Time.deltaTime);
        } else {
            transform.position = Vector3.Lerp(transform.position, target.position + offsetPosition, followSpeed * Time.deltaTime);
        }

        // compute rotation
        if (lookAt) {
            transform.LookAt(target);
        } else {
            transform.rotation = target.rotation;
        }
    }
}