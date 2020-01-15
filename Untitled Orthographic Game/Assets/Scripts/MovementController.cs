using System;
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
    protected NavMeshAgent _navMeshAgent;
    protected CharacterController _characterController;
    protected Animator _animator;
    protected FootIKController _footIKController;
    protected HeadIKController _headIKController;
    protected HandIKController _handIKController;
    protected RagdollHelper _ragdollHelper;

    [Header("Speed properties")]
    [SerializeField]
    public float moveSpeed = 5;
    public float rotateSpeed = 1;
    public float animatorLerpSpeed = 5;

    public bool agentControlled { get; set; } = true;
    public bool isWalking { get; private set; } = false;

    public delegate void EventHandler(object sender, EventArgs args);
    public event EventHandler ReachedDestination = delegate { };

    protected float verticalVelocity;
    protected Vector3 direction;

    protected void Start() {
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
        verticalVelocity = 0;

        if (agentControlled) {
            direction = _navMeshAgent.desiredVelocity.normalized;
        }

        direction *= moveSpeed;

        if (!_characterController.isGrounded) {
            verticalVelocity += Physics.gravity.y * Time.deltaTime;
        }

        //Normalize direction.
        transform.forward = Vector3.Slerp(transform.forward, direction, 3 * Time.deltaTime);

        direction.y = verticalVelocity;

        // Uses all prior information to move the player.
        _characterController.Move(direction * Time.deltaTime);

        _navMeshAgent.velocity = _characterController.velocity;

        // Gets the previous SpeedX and Y.
        float speedY = _animator.GetFloat("SpeedY");
        float speedX = _animator.GetFloat("SpeedX");

        // Smooths them to the current values.
        speedY = Mathf.Lerp(speedY, transform.InverseTransformDirection(_characterController.velocity).z, animatorLerpSpeed * Time.deltaTime);
        speedX = Mathf.Lerp(speedX, transform.InverseTransformDirection(_characterController.velocity).x, animatorLerpSpeed * Time.deltaTime);

        // Sets the values.
        _animator.SetFloat("SpeedY", speedY);
        _animator.SetFloat("SpeedX", speedX);

        // If the npc is walking, check to see if it should be heading to next destination.
        if (isWalking && !_navMeshAgent.pathPending) {
            if (_navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance) {
                if (!_navMeshAgent.hasPath || _characterController.velocity.sqrMagnitude == 0f) {
                    ReachedDestination(this, new EventArgs());
                    isWalking = false;
                }
            }
        }
    }

    public void Stop() {
        _navMeshAgent.isStopped = true;
    }

    public void SetLocation(Transform loc) {
        SetLocation(loc.position);
    }

    public void SetLocation(Vector3 loc) {
        _navMeshAgent.isStopped = false;
        isWalking = true;
        _navMeshAgent.SetDestination(loc);
    }

    /// <summary>
    /// Makes the npc face a target position.
    /// </summary>
    /// <param name="target"></param>
    public void Face(Transform target) {
        _headIKController?.LookAt(target);

        // Stops the npc from being able to move.
        Stop();

        // Calculates the direction by finding the normalized difference.
        Vector3 direction = (target.position - transform.position).normalized;

        // Converts the direction to a quaternion.
        Quaternion lookRotation = Quaternion.LookRotation(direction);

        /* Sets the x and y rotations to 0 so that the npc only turns on
           the y axis.*/
        lookRotation.x = lookRotation.z = 0;

        // If a coroutine is running, stop it.
        if (lookCoroutine != null) {
            StopCoroutine(lookCoroutine);
        }

        // Start the look look at coroutine.
        lookCoroutine = StartCoroutine(LookAtCoroutine(lookRotation));
    }

    public void StopFace() {
        if (lookCoroutine != null) {
            StopCoroutine(lookCoroutine);
        }
    }

    Coroutine lookCoroutine;

    /// <summary>
    /// The coroutine the rotates the npc based on the angle.
    /// </summary>
    /// <param name="angle">The angle the npc is rotating towards</param>
    /// <returns></returns>
    private IEnumerator LookAtCoroutine(Quaternion angle) {
        while (Quaternion.Angle(transform.rotation, angle) > 1) {
            transform.rotation = Quaternion.Slerp(transform.rotation, angle, rotateSpeed * Time.deltaTime);
            yield return null;
        }
    }

    public void LookAt(Transform pos) {
        _headIKController.LookAt(pos);
    }

    public void LookAtRandom(Transform[] trans) {
        _headIKController.LookAt(_headIKController.GetClosestInFrontTransform(trans));
    }

    public void StopLookAt() {
        //_headIKController.stop
    }

    public void RagDoll() {
        _ragdollHelper.ragdolled = true;
    }

    public void StopRagdoll() {
        _ragdollHelper.ragdolled = false;
    }
}
