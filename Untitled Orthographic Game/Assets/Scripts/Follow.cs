using System.Collections;
using UnityEngine;

public class Follow : MonoBehaviour {
    public Transform target;

    [Header("Offset")]
    public Vector3 offsetPositionFollow;
    public Space offsetPositionFollowSpace = Space.Self;

    [Header("Offset")]
    public Vector3 offsetPositionLookAt;

    [Header("Follow")]
    [SerializeField]
    private bool followPosition = true;
    public bool FollowPosition {
        get { return followPosition; }
        set {
            followPosition = value;
            StartCoroutine(smoothFollowSpeed(followMotion, (followPosition) ? targetFollowSpeed : 0));
        }
    }
    public FollowMotions followMotion = FollowMotions.Instant;
    public float targetFollowSpeed = 5;
    public float followSpeed = 0;

    public enum FollowMotions { Instant, Lerp, Slerp }

    [Header("Rotation")]
    [SerializeField]
    private bool lookAt = true;
    public bool LookAt {
        get { return lookAt; }
        set { 
            lookAt = value;
            StartCoroutine(smoothRotationSpeed(followMotion, (lookAt) ? targetRotationSpeed : 0));
        }
    }
    public FollowMotions rotationMotion = FollowMotions.Instant;
    public float targetRotationSpeed = 2;
    private float rotationSpeed = 0;

    public float smoothSpeed = 5;

    private void Update() {
        Refresh();
    }

    private void Refresh() {
        if (target == null) {
            return;
        }

        // Computes the target position in either self space or world space.
        Vector3 targetPos = (offsetPositionFollowSpace == Space.Self) ? target.TransformPoint(offsetPositionFollow) : target.position + offsetPositionFollow;
        Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position);

        // Lerps or Slerps the current position to the target position.
        if (true) {
            switch (followMotion) {
                case FollowMotions.Instant:
                    if (FollowPosition) transform.position = targetPos;
                    break;
                case FollowMotions.Lerp:
                    transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);
                    break;
                case FollowMotions.Slerp:
                    transform.position = Vector3.Slerp(transform.position, targetPos, followSpeed * Time.deltaTime);
                    break;
            }
        }

        // Lerps or Slerps the current rotation to the target rotation.
        if (true) {
            switch (rotationMotion) {
                case FollowMotions.Instant:
                    if (LookAt) transform.LookAt(target);
                    break;
                case FollowMotions.Lerp:
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                    break;
                case FollowMotions.Slerp:
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                    break;
            }
        }
    }

    private IEnumerator smoothFollowSpeed (FollowMotions motion, float target) {
        while (Mathf.Abs(followSpeed - target) > 0.001f) {
            switch (motion) {
                case FollowMotions.Instant:
                    followSpeed = target;
                    break;
                case FollowMotions.Lerp:
                    followSpeed = Mathf.Lerp(followSpeed, target, smoothSpeed * Time.deltaTime);
                    break;
                case FollowMotions.Slerp:
                    followSpeed = Mathf.SmoothStep(followSpeed, target, smoothSpeed * Time.deltaTime);
                    break;
            }
            yield return null;
        }
    }

    private IEnumerator smoothRotationSpeed(FollowMotions motion, float target) {
        while (Mathf.Abs(followSpeed - target) > 0.001f) {
            switch (motion) {
                case FollowMotions.Instant:
                    rotationSpeed = target;
                    break;
                case FollowMotions.Lerp:
                    rotationSpeed = Mathf.Lerp(rotationSpeed, target, smoothSpeed * Time.deltaTime);
                    break;
                case FollowMotions.Slerp:
                    rotationSpeed = Mathf.SmoothStep(rotationSpeed, target, smoothSpeed * Time.deltaTime);
                    break;
            }
            yield return null;
        }
    }
}