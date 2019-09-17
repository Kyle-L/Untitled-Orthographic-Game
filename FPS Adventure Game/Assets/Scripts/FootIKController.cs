using UnityEngine;

[RequireComponent(typeof(Animator))]
public class FootIKController : MonoBehaviour {

    // Constants
    private const float RAYCAST_MAX_DISTANCE = 5f;

    // Preferences
    [SerializeField]
    private bool IkActive = true;

    private float positionWeight;

    [Range(0f, 1f)]
    [SerializeField]
    private float positionWeightMultiplier = 1;
    public float PositionWeight {
        get {
            return positionWeightMultiplier;
        }
        set {
            positionWeightMultiplier = value;
        }
    }

    [Range(0f, 1f)]
    [SerializeField]
    private float rotationWeight = 1;
    public float RotationWeight {
        get {
            return rotationWeight;
        }
        set {
            rotationWeight = value;
        }
    }
    [SerializeField]
    private Vector3 offset;
    [SerializeField]
    private LayerMask RayMask;

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
        positionWeight = _animator.GetFloat("Weight") * positionWeightMultiplier;

        // If IK is enabled, perform IK on both feet.
        if (IkActive) {
            PerformFootIK(AvatarIKGoal.RightFoot);
            PerformFootIK(AvatarIKGoal.LeftFoot);
            // If IK is disabled, set the weight to 0.
        } else {
            _animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 0f);
            _animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 0f);
            _animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 0f);
            _animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 0f);
        }
    }

    /// <summary>
    /// Performs IK on a foot.
    /// </summary>
    /// <param name="foot">The foot that the IK is performed on.</param>
    private void PerformFootIK (AvatarIKGoal foot) { 
        // Gets the initial position.
        Vector3 FootPos = _animator.GetIKPosition(foot);

        // Raycasts down from the foot.
        RaycastHit hit;
        if (Physics.Raycast(FootPos + Vector3.up, Vector3.down, out hit, RAYCAST_MAX_DISTANCE, RayMask)) {
            // Sets IK weights of the foot based on the script's variables.
            _animator.SetIKPositionWeight(foot, positionWeight);
            _animator.SetIKRotationWeight(foot, rotationWeight);

            // Sets the IK position to the hit point plus the offset.
            _animator.SetIKPosition(foot, hit.point + offset); 

            /* If the rotation weight is greater than 0.
             * calculate the rotation the foot should be
             * on the surface below the foot. */
            if (rotationWeight > 0f) {
                // Calculates the look rotation by projecting a vector onto the hit point normal below the foot.
                Quaternion footRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, hit.normal), hit.normal);
                // Sets the IK rotation.
                _animator.SetIKRotation(foot, footRotation);
            }
        // If the raycast doesn't hit anything, set the position and rotation weight to 0.
        } else {
            _animator.SetIKPositionWeight(foot, 0f);
            _animator.SetIKRotationWeight(foot, 0f);
        }
    }
}
