using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerMovementController : MonoBehaviour {

    // Components
    private CharacterController _characterController;
    private Animator _animator;
    private NavMeshAgent _agent;

    // Movement Controller Variables
    [Header("Movement Speed Settings")]
    public float moveSpeed = 5;
    public float sideSpeedMultiplier = 0.75f;
    public float animatorLerpSpeed = 5;

    // Jumping
    [Header("Jumping Settings")]
    public bool jumpEnabled = true;
    public float jumpSpeed = 2.5f;
    public float jumpHeightDetection = 0.5f;

    public Camera playerMainCamera;
    public Transform target;

    public bool AgentControlled { get; set; } = true;
    public bool Control { get; set; } = true;

    private void Start() {
        // Gets componenets.
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();

        _agent.updatePosition = false;
        _agent.updateRotation = false;
    }

    private void Update() {
        #region Movement
        float horizontal = 0;
        float vertical = 0;
        float verticalVelocity = 0;
        Vector3 direction = Vector3.zero;

        if (Control) {
            if (Input.GetButtonDown("Jump")) {
                AgentControlled = true;
                _agent.SetDestination(target.position);
            } else if (Input.GetAxis("Vertical") > 0 || Input.GetAxis("Horizontal") > 0) {
                AgentControlled = false;
            }

            if (!AgentControlled) {
                // Movement
                vertical = Input.GetAxis("Vertical");
                horizontal = Input.GetAxis("Horizontal");

                //Converts user input to a direction.
                direction = new Vector3(horizontal, 0f, vertical);

                //Makes direction relative to camera.
                direction = playerMainCamera.transform.TransformDirection(direction);
                direction.y = 0.0f;

                direction = Vector3.Normalize(direction);
            } else {
                direction = _agent.desiredVelocity.normalized;
            }
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

        _agent.velocity = _characterController.velocity;

        // Gets the previous SpeedX and Y.
        float speedY = _animator.GetFloat("SpeedY");
        float speedX = _animator.GetFloat("SpeedX");

        // Smooths them to the current values.
        speedY = Mathf.Lerp(speedY, transform.InverseTransformDirection(_characterController.velocity).z, animatorLerpSpeed * Time.deltaTime);
        speedX = Mathf.Lerp(speedX, transform.InverseTransformDirection(_characterController.velocity).x, animatorLerpSpeed * Time.deltaTime);

        // Sets the values.
        _animator.SetFloat("SpeedY", speedY);
        _animator.SetFloat("SpeedX", speedX);
        #endregion
    }

}
