using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class HandIKController : MonoBehaviour {

    // Constants
    private const float RAYCAST_MAX_DISTANCE = 5f;

    // Preferences
    [Range(0f, 1f)]
    public float positionWeightLeftHand = 1;
    [Range(0f, 1f)]
    public float rotationWeightLeftHand = 1;
    [Range(0f, 1f)]
    public float positionWeightRightHand = 1;
    [Range(0f, 1f)]
    public float rotationWeightRightHand = 1;

    public Vector3 leftOffset;
    public Vector3 rightOffset;
    public bool leftHand;
    public bool rightHand;


    public Transform leftObject;
    public Transform rightObject;

    // Components
    private Animator _animator;

    void Start() {
        _animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Called on the object with an animator with IK Pass enabled. Called before the final
    /// animation pass.
    /// </summary>
    void OnAnimatorIK() {
        if (leftHand) {
            PerformHandIK(AvatarIKGoal.LeftHand, leftObject, leftOffset);

        }
        if (rightHand) {
            PerformHandIK(AvatarIKGoal.RightHand, rightObject, rightOffset);
        }

        _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, positionWeightLeftHand);
        _animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, rotationWeightLeftHand);

        _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, positionWeightRightHand);
        _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, rotationWeightRightHand);
    }

    /// <summary>
    /// Performs IK on a foot.
    /// </summary>
    /// <param name="hand">The foot that the IK is performed on.</param>
    private void PerformHandIK(AvatarIKGoal hand, Transform heldObject, Vector3 offset) {
        if (heldObject != null) {
            // Sets the IK position to the hit point plus the offset.
            _animator.SetIKPosition(hand, heldObject.position + offset);

            /* If the rotation weight is greater than 0.
             * calculate the rotation the foot should be
             * on the surface below the foot. */
            if (rotationWeightLeftHand > 0f) {
                // Calculates the look rotation by projecting a vector onto the hit point normal below the foot.
                Quaternion rotation = heldObject.rotation;
                // Sets the IK rotation.
                _animator.SetIKRotation(hand, rotation);
            }

        }
    }


    public void LerpPositon(float weight) {
        StartCoroutine(LerpPositionCoroutine(weight));
    }

    private IEnumerator LerpPositionCoroutine(float weight) {
        while (Mathf.Abs(weight - positionWeightLeftHand) > 0.01f || Mathf.Abs(weight - positionWeightRightHand) > 0.01f) {
            positionWeightLeftHand = Mathf.Lerp(positionWeightLeftHand, weight, Time.deltaTime);
            positionWeightRightHand = Mathf.Lerp(positionWeightRightHand, weight, Time.deltaTime);
            yield return null;
        }
    }

    public void LerpRotation(float weight) {
        StartCoroutine(LerpRotationCoroutine(weight));
    }

    private IEnumerator LerpRotationCoroutine(float weight) {
        while (Mathf.Abs(weight - rotationWeightLeftHand) > 0.01f || Mathf.Abs(weight - rotationWeightRightHand) > 0.01f) {
            rotationWeightLeftHand = Mathf.Lerp(rotationWeightLeftHand, weight, Time.deltaTime);
            rotationWeightRightHand = Mathf.Lerp(rotationWeightRightHand, weight, Time.deltaTime);
            yield return null;
        }
    }
}
