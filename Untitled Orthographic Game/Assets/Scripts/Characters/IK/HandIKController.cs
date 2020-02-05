using UnityEngine;

[RequireComponent(typeof(Animator))]
public class HandIKController : MonoBehaviour {

    // Constants
    private const float RAYCAST_MAX_DISTANCE = 5f;

    // Preferences
    public bool IkActive { get; set; } = true;

    [Range(0f, 1f)]
    public float positionWeightMultiplierLeftHand = 1;
    [Range(0f, 1f)]
    public float rotationWeightLeftHand = 1;
    [Range(0f, 1f)]
    public float positionWeightMultiplierRightHand = 1;
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
        // If IK is enabled, perform IK on both feet.
        if (IkActive) {
            if (leftHand) {
                PerformFootIK(AvatarIKGoal.LeftHand, rotationWeightLeftHand, positionWeightMultiplierLeftHand, leftObject, leftOffset);
            }
            if (rightHand) {
                PerformFootIK(AvatarIKGoal.RightHand, rotationWeightRightHand, positionWeightMultiplierRightHand, rightObject, rightOffset);
            }

            // If IK is disabled, set the weight to 0.
        } else {
            _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0f);
            _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0f);
            _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0f);
            _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0f);
        }
    }

    /// <summary>
    /// Performs IK on a foot.
    /// </summary>
    /// <param name="hand">The foot that the IK is performed on.</param>
    private void PerformFootIK(AvatarIKGoal hand, float rotationWeight, float positionWeight, Transform heldObject, Vector3 offset) {
        // Gets the initial position.
        Vector3 FootPos = _animator.GetIKPosition(hand);

        if (heldObject != null) {
            // Sets IK weights of the foot based on the script's variables.
            _animator.SetIKPositionWeight(hand, positionWeight);
            _animator.SetIKRotationWeight(hand, rotationWeight);

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

            // If the raycast doesn't hit anything, set the position and rotation weight to 0.
        } else {
            _animator.SetIKPositionWeight(hand, 0f);
            _animator.SetIKRotationWeight(hand, 0f);
        }
    }
}
