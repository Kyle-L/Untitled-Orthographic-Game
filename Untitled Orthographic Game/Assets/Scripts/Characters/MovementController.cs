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
    [HideInInspector]
    public NavMeshAgent _navMeshAgent;
    protected CharacterController _characterController;
    protected Animator _animator;
    public FootIKController FootIKController { get; private set; }
    public HeadIKController HeadIKController { get; private set; }
    public HandIKController HandIKController { get; private set; }
    protected RagdollHelper _ragdollHelper;

    [Header("Speed properties")]
    [SerializeField]
    public float moveSpeed = 5;
    public float alignSpeed = 3;
    public float rotateSpeed = 3;
    public float animatorLerpSpeed = 5;
    public float fallLerpSpeed = 3;
    public float fallVelocityActivation = 6;

    public bool agentControlled { get; set; } = true;
    public bool isPosed { get; private set; } = false;
    private bool canLookPosed;
    private bool canRotatePosed;
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
        FootIKController = this.GetComponent<FootIKController>();
        HeadIKController = this.GetComponent<HeadIKController>();
        HandIKController = this.GetComponent<HandIKController>();
        _ragdollHelper = this.GetComponent<RagdollHelper>();

        _navMeshAgent.updatePosition = false;
        _navMeshAgent.updateRotation = false;
    }

    protected void Update() {
        if (!_characterController.enabled) {
            return;
        }

        if (agentControlled) {
            direction = _navMeshAgent.desiredVelocity.normalized;
        }

        direction *= moveSpeed;

        if (!_characterController.isGrounded) {
            verticalVelocity = Mathf.Lerp(verticalVelocity, Physics.gravity.y, Time.deltaTime * fallLerpSpeed);
        } else {
            verticalVelocity = 0;
        }

        //Normalize direction.
        if (direction != Vector3.zero) {
            transform.forward = Vector3.Slerp(transform.forward, direction, 3 * Time.deltaTime);
        }

        direction.y = verticalVelocity;

        // Uses all prior information to move the player.
        _characterController.Move(direction * Time.deltaTime);

        _navMeshAgent.velocity = _characterController.velocity;
        _navMeshAgent.nextPosition = transform.position;


        // Gets the previous SpeedX and Y.
        float speedY = _animator.GetFloat("SpeedY");
        float speedX = _animator.GetFloat("SpeedX");
        float speedZ = _animator.GetFloat("SpeedZ");

        // Smooths them to the current values.
        speedY = Mathf.Lerp(speedY, transform.InverseTransformDirection(_characterController.velocity).z, animatorLerpSpeed * Time.deltaTime);
        speedX = Mathf.Lerp(speedX, transform.InverseTransformDirection(_characterController.velocity).x, animatorLerpSpeed * Time.deltaTime);
        speedZ = Mathf.Lerp(speedZ, (Math.Abs(_characterController.velocity.y) < fallVelocityActivation) ?  0 : transform.InverseTransformDirection(_characterController.velocity).y, animatorLerpSpeed * Time.deltaTime);

        // Sets the values.
        _animator.SetFloat("SpeedY", speedY);
        _animator.SetFloat("SpeedX", speedX);
        _animator.SetFloat("SpeedZ", speedZ);
    }

    /// <summary>
    /// Makes the npc face a target position.
    /// </summary>
    /// <param name="target"></param>
    public void Face(Transform target) {
        if (!canRotatePosed && isPosed) {
            print(this.name + " cannot face anything currently as it is posed.");
            return;
        }

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
        while (Vector3.Distance(transform.rotation.eulerAngles, angle.eulerAngles) > 1) {
            transform.rotation = Quaternion.Slerp(transform.rotation, angle, rotateSpeed * Time.deltaTime);
            yield return null;
        }
    }

    public void AlignRotation(Transform transform) {
        if (!canRotatePosed && isPosed) {
            print(this.name + " cannot be rotated currently as it is posed.");
            return;
        }

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

    public void SetPosition(Transform pos, bool resetAnimation = true) {
        SetPosition(pos.position, pos.rotation, resetAnimation);
    }

    public void SetPosition(Vector3 pos, Quaternion rot, bool resetAnimation = true) {
        // If a coroutine is running, stop it.
        if (transformMoveCoroutine != null) {
            StopCoroutine(transformMoveCoroutine);
        }

        _navMeshAgent.Warp(pos);
        transform.position = pos;
        transform.rotation = rot;
        if (resetAnimation) {
            _animator.SetTrigger("Idle");
        }
    }

    public void SetPosition(Vector3 pos, bool resetAnimation = true) {
        // If a coroutine is running, stop it.
        if (transformMoveCoroutine != null) {
            StopCoroutine(transformMoveCoroutine);
        }

        _navMeshAgent.Warp(pos);
        transform.position = pos;
        if (resetAnimation) {
            _animator.SetTrigger("Idle");
        }
    }

    public void Pose(int poseLayer, string poseAnimation, bool canLook, bool canRotate, bool disable) {
        if (disable) {
            _characterController.enabled = false;
            _navMeshAgent.enabled = false;
        }

        isPosed = true;
        canRotatePosed = canRotate;
        canLookPosed = canLook;

        _animator.Play(poseAnimation, poseLayer);
    }

    public void Unpose() {
        isPosed = false;
        _characterController.enabled = true;
        _navMeshAgent.enabled = true;
        _animator.SetTrigger("Idle");
    }


    /// <summary>
    /// Makes the character look at specific transform.
    /// </summary>
    /// <param name="pos"></param>
    public void LookAt(Transform pos) {
        if (!canLookPosed && isPosed) {
            print(this.name + " cannot be look currently as it is posed.");
            return;
        }

        HeadIKController.LookAt(pos);
    }

    /// <summary>
    /// Makes the character look at a random transform.
    /// </summary>
    /// <param name="trans"></param>
    public void LookAtRandom(Transform[] trans) {
        if (!canLookPosed && isPosed) {
            print(this.name + " cannot be look currently as it is posed.");
            return;
        }

        HeadIKController.LookAt(HeadIKController.GetClosestInFrontTransform(trans));
    }

    public void SetDestination(Transform transform) {
        SetDestination(transform.position);
    }

    public void SetDestination(Vector3 position) {
        if (!_characterController.enabled || !_navMeshAgent.enabled) {
            print(this.name + " can't move at the moment since either the CharacterContoller or NavmeshAgent is disabled");
            return;
        }

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

    public void TriggerAnimation(string trigger) {
        _animator.SetTrigger(trigger);
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
