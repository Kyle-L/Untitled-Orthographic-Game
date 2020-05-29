using UnityEngine;

public class Follow : MonoBehaviour {
    public Transform target;

    [Header("Offset")]
    public Vector3 offsetPosition;
    public Space offsetPositionSpace = Space.Self;

    [Header("Follow")]
    public FollowMotions followMotion = FollowMotions.Lerp;
    public float followSpeed = 5;
    public enum FollowMotions { Lerp, Slerp }

    [Header("Rotation")]
    public bool lookAt = true;

    private void Update() {
        Refresh();
    }

    private void Refresh() {
        if (target == null) {
            Debug.LogWarning("Missing target ref !", this);
            return;
        }

        // Computes the target position in either self space or world space.
        Vector3 targetPos = (offsetPositionSpace == Space.Self) ? target.TransformPoint(offsetPosition) : target.position + offsetPosition;

        // Lerps or Slerps the current position to the target position.
        switch (followMotion) {
            case FollowMotions.Lerp:
                transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);
                break;
            case FollowMotions.Slerp:
                transform.position = Vector3.Slerp(transform.position, targetPos, followSpeed * Time.deltaTime);
                break;
        }

        // Computes the rotation.
        if (lookAt) {
            transform.LookAt(target);
        }
    }
}