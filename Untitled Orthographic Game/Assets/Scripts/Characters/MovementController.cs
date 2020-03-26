using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(FootIKController))]
[RequireComponent(typeof(HeadIKController))]
[RequireComponent(typeof(HandIKController))]
[RequireComponent(typeof(RagdollHelper))]
public abstract class MovementController : MonoBehaviour {

    [Header("Components")]
    [HideInInspector]
    public NavMeshAgent _navMeshAgent;
    protected CharacterController _characterController;
    protected Animator _animator;
    protected FootIKController _footIKController;
    protected HeadIKController _headIKController;
    protected HandIKController _handIKController;
    protected RagdollHelper _ragdollHelper;

    [Header("Speed properties")]
    [SerializeField]
    public float moveSpeed = 5;
    public float alignSpeed = 3;
    public float rotateSpeed = 3;
    public float animatorLerpSpeed = 5;

    public bool agentControlled { get; set; } = true;
    public bool isWalking { get; private set; } = false;

    protected float verticalVelocity;
    protected Vector3 direction;

    public enum AnimationTriggers {
        SitDown,
        Exit
    }

    protected Coroutine angleCoroutine;
    protected Coroutine transformMoveCoroutine;
    protected Coroutine agentMoveCoroutine;

    protected void Awake() {
        _navMeshAgent = this.GetComponent<NavMeshAgent>();
        _characterController = this.GetComponent<CharacterController>();
        _animator = this.GetComponent<Animator>();
        _footIKController = this.GetComponent<FootIKController>();
        _headIKController = this.GetComponent<HeadIKController>();
        _handIKController = this.GetComponent<HandIKController>();
        _ragdollHelper = this.GetComponent<RagdollHelper>();

        _navMeshAgent.updatePosition = false;
        _navMeshAgent.updateRotation = false;
    }

    protected void Update() {
        if (!_characterController.enabled) {
            return;
        }

        verticalVelocity = 0;

        if (agentControlled) {
            direction = _navMeshAgent.desiredVelocity.normalized;
        }

        direction *= moveSpeed;

        if (!_characterController.isGrounded) {
            verticalVelocity += Physics.gravity.y/* * Time.deltaTime*/;
        }


        //Normalize direction.
        transform.forward = Vector3.Slerp(transform.forward, direction, 3 * Time.deltaTime);

        direction.y = verticalVelocity;

        // Uses all prior information to move the player.
        _characterController.Move(direction * Time.deltaTime);

        _navMeshAgent.velocity = _characterController.velocity;
        _navMeshAgent.nextPosition = transform.position;

        // Gets the previous SpeedX and Y.
        float speedY = _animator.GetFloat("SpeedY");
        float speedX = _animator.GetFloat("SpeedX");

        // Smooths them to the current values.
        speedY = Mathf.Lerp(speedY, transform.InverseTransformDirection(_characterController.velocity).z, animatorLerpSpeed * Time.deltaTime);
        speedX = Mathf.Lerp(speedX, transform.InverseTransformDirection(_characterController.velocity).x, animatorLerpSpeed * Time.deltaTime);

        // Sets the values.
        _animator.SetFloat("SpeedY", speedY);
        _animator.SetFloat("SpeedX", speedX);
    }

    /// <summary>
    /// Makes the npc face a target position.
    /// </summary>
    /// <param name="target"></param>
    public void Face(Transform target) {
        // Calculates the direction by finding the normalized difference.
        Vector3 direction = (target.position - transform.position).normalized;

        // Converts the direction to a quaternion.
        Quaternion lookRotation = Quaternion.LookRotation(direction);

        /* Sets the x and y rotations to 0 so that the npc only turns on
           the y axis.*/
        lookRotation.x = lookRotation.z = 0;

        // If a coroutine is running, stop it.
        if (angleCoroutine != null) {
            StopCoroutine(angleCoroutine);
        }

        // Start the look look at coroutine.
        angleCoroutine = StartCoroutine(SetAngleSlerp(lookRotation));
    }

    public void StopFace() {
        if (angleCoroutine != null) {
            StopCoroutine(angleCoroutine);
        }
    }

    /// <summary>
    /// The coroutine the rotates the npc based on the angle.
    /// </summary>
    /// <param name="angle">The angle the npc is rotating towards</param>
    /// <returns></returns>
    private IEnumerator SetAngleSlerp(Quaternion angle) {
        while (Quaternion.Angle(transform.rotation, angle) > 1) {
            transform.rotation = Quaternion.Slerp(transform.rotation, angle, rotateSpeed * Time.deltaTime);
            yield return null;
        }
    }

    public void AlignRotation(Transform transform) {
        // If a coroutine is running, stop it.
        if (angleCoroutine != null) {
            StopCoroutine(angleCoroutine);
        }

        // Start the look look at coroutine.
        angleCoroutine = StartCoroutine(SetAngleSlerp(transform.rotation));
    }

    public void AlignPosition(Transform transform) {
        // If a coroutine is running, stop it.
        if (transformMoveCoroutine != null) {
            StopCoroutine(transformMoveCoroutine);
        }

        // Start the look look at coroutine.
        transformMoveCoroutine = StartCoroutine(SetPositionSlerp(transform.position));
    }

    /// <summary>
    /// The coroutine the transforms the npc based on the position.
    /// </summary>
    /// <param name="pos">The position the npc is transforming towards</param>
    /// <returns></returns>
    private IEnumerator SetPositionSlerp(Vector3 pos) {
        while (Vector3.Distance(transform.position, pos) > 0.05f) {
            transform.position = Vector3.Slerp(transform.position, pos, alignSpeed * Time.deltaTime);
            yield return null;
        }
    }

    public void SetPosition(Vector3 pos) {
        _navMeshAgent.Warp(pos);
        transform.position = pos;
    }

    /// <summary>
    /// Makes the character look at specific transform.
    /// </summary>
    /// <param name="pos"></param>
    public void LookAt(Transform pos) {
        _headIKController.LookAt(pos);
    }

    /// <summary>
    /// Makes the character look at a random transform.
    /// </summary>
    /// <param name="trans"></param>
    public void LookAtRandom(Transform[] trans) {
        _headIKController.LookAt(_headIKController.GetClosestInFrontTransform(trans));
    }

    public void SetDestination(Transform transform) {
        SetDestination(transform.position);
    }

    public void SetDestination(Vector3 position) {
        if (agentMoveCoroutine != null) {
            StopCoroutine(agentMoveCoroutine);
        }
        agentMoveCoroutine = StartCoroutine(WaitToSet(position));
    }

    private IEnumerator WaitToSet(Vector3 position) {
        _navMeshAgent.destination = position;
        _navMeshAgent.isStopped = true;
        while (!IsAnimating()) {
            yield return null;
        }
        _navMeshAgent.isStopped = false;
        agentControlled = true;
    }

    public void TriggerAnimation(AnimationTriggers trigger) {
        _animator.SetTrigger(trigger.ToString());
    }

    public void SetCharacterControllerState(bool state) {
        //_navMeshAgent.enabled = state;
        _characterController.enabled = state;
    }

    public void RagDoll() {
        _ragdollHelper.ragdolled = true;
    }

    public void StopRagdoll() {
        _ragdollHelper.ragdolled = false;
    }

    /// <summary>
    /// Returns whether the player is in any animation state other than Idle and Movement.
    /// </summary>
    /// <returns></returns>
    public bool IsAnimating() {
        return (_animator.GetCurrentAnimatorStateInfo(0).IsName("Movement") || _animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"));
    }
}
