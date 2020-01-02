using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementController : MonoBehaviour {

    // Components
    private CharacterController _characterController;
    [SerializeField]
    private Animator _animator;

    // Camera rotation
    [Header("Camera Rotation Settings")]
    [SerializeField]
    private float upDownRange = 80f;
    private float xRotation;
    private float yRotation;
    private float currentXRotation;
    private float currentYRotation;
    private float xRotationV;
    private float yRotationV;
    private float rotationSmoothTime = 0.075f;
    [SerializeField]
    private float currentLookSensitivity { get; set; } = 10;

    // Movement Controller Variables
    [Header("Movement Speed Settings")]
    [SerializeField]
    private float moveSpeed = 5;
    [SerializeField]
    private float sideSpeedMultiplier = 0.75f;
    [SerializeField]
    private float animatorLerpSpeed = 5;

    // Jumping
    [Header("Jumping Settings")]
    [SerializeField]
    private bool jumpEnabled = true;
    private float verticalVelocity = 0f;
    [SerializeField]
    private float jumpSpeed = 2.5f;
    [SerializeField]
    private float jumpHeightDetection = 0.5f;

    [SerializeField]
    private Camera playerMainCamera;

    public bool Control { get; set; } = true;

    private void Start() {
        // Gets componenets.
        _characterController = GetComponent<CharacterController>();
        //_animator = GetComponent<Animator>();

        // Default look direction.
        SetRotation(transform.localEulerAngles.y, playerMainCamera.transform.localEulerAngles.x);
    }

    private void Update() {
        #region Movement
        float forwardSpeed = 0;
        float sideSpeed = 0;


        if (Control) {
            // Rotation
            xRotation -= Input.GetAxis("Axis Y") * currentLookSensitivity;
            yRotation += Input.GetAxis("Axis X") * currentLookSensitivity;

            // Movement
            forwardSpeed = Input.GetAxis("Vertical");
            sideSpeed = Input.GetAxis("Horizontal");
        }

        // Clamps the xRotation so the player can't look directly up or directly down.
        xRotation = Mathf.Clamp(xRotation, -upDownRange, upDownRange);

        // Smooths the camera's X and Y rotations based on the velocity and smoothTime.
        currentXRotation = Mathf.SmoothDamp(currentXRotation, xRotation, ref xRotationV, rotationSmoothTime);
        currentYRotation = Mathf.SmoothDamp(currentYRotation, yRotation, ref yRotationV, rotationSmoothTime);

        // Rotates the gameObject to face the camera's Y rotation based on the player's input.
        transform.rotation = Quaternion.Euler(0, currentYRotation, 0);

        // Rotates the camera's X rotation based on the player's input.
        playerMainCamera.transform.localRotation = Quaternion.Euler(currentXRotation, 0, 0);

        if (jumpEnabled && Input.GetButtonDown("Jump") && Control && _characterController.isGrounded) {
            // Prevents the player from jumping while an object is above their head.
            if (!Physics.Raycast(transform.position + new Vector3(0, _characterController.height, 0), Vector3.up, out RaycastHit hit, jumpHeightDetection)) {
                verticalVelocity = jumpSpeed;
            }
            _animator.SetTrigger("Jump");
            // Handles gravity.
        } else if (!_characterController.isGrounded) {
            verticalVelocity += Physics.gravity.y * Time.deltaTime;
        }

        // Creates the vector that will be used to tell the CharacterController where to move.
        Vector3 speed = new Vector3(sideSpeed * moveSpeed * sideSpeedMultiplier, verticalVelocity, forwardSpeed * moveSpeed);

        /* Modifies the speed variable to make it relative to the gameObjects rotation. 
        (Aka telling it to move forward will make it move forward) */
        speed = transform.rotation * speed;

        // Uses all prior information to move the player.
        _characterController.Move(speed * Time.deltaTime);

        // Gets the previous SpeedX and Y.
        float speedY = _animator.GetFloat("SpeedY");
        float speedX = _animator.GetFloat("SpeedX");

        // Smooths them to the current values.
        speedY = Mathf.Lerp(speedY, forwardSpeed * _characterController.velocity.magnitude, animatorLerpSpeed * Time.deltaTime);
        speedX = Mathf.Lerp(speedX, sideSpeed * _characterController.velocity.magnitude, animatorLerpSpeed * Time.deltaTime);

        // Sets the values.
        _animator.SetFloat("SpeedY", speedY);
        _animator.SetFloat("SpeedX", speedX);
        #endregion
    }

    public void SetRotation(float yRot, float xRot) {
        currentYRotation = yRot;
        yRotation = yRot;

        currentXRotation = xRot;
        xRotation = xRot;
    }

    /// <summary>
    /// Makes the player look at a particular position.
    /// </summary>
    /// <param name="pos"></param>
    public void LookAt(Transform pos, float sens = 1, float time = 0) {
        // Stop the existing coroutine.
        if (lookCoroutine != null) {
            StopCoroutine(lookCoroutine);
        }

        // Start the coroutine.
        lookCoroutine = StartCoroutine(Look(pos, sens, time));
    }

    public void StopLookAt () {
        if (lookCoroutine != null) {
            StopCoroutine(lookCoroutine);
        }
    }

    private Coroutine lookCoroutine;

    /// <summary>
    /// The coroutine that ensures the character slowly turns their head to look at the position
    /// rather than abruptly looking at the position.
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    private IEnumerator Look(Transform pos, float sens = 1, float time = 0) {
        Vector3 direction = pos.position - playerMainCamera.transform.position;
        Quaternion toRotationWorld = Quaternion.LookRotation(direction);
        Quaternion toRotationLocal = Quaternion.Inverse(playerMainCamera.transform.rotation) * toRotationWorld;

        while (Mathf.Abs(currentYRotation - toRotationWorld.eulerAngles.y) > sens || Mathf.Abs(currentXRotation - toRotationWorld.eulerAngles.x) > sens || time > 0) {
            direction = pos.position - playerMainCamera.transform.position;
            
            
            toRotationWorld = Quaternion.LookRotation(direction);
            toRotationLocal = Quaternion.Inverse(playerMainCamera.transform.rotation) * toRotationWorld;

            currentXRotation = xRotation = Mathf.LerpAngle(currentXRotation, toRotationLocal.eulerAngles.x, Time.deltaTime);
            currentYRotation = yRotation = Mathf.LerpAngle(currentYRotation, toRotationWorld.eulerAngles.y, Time.deltaTime);
            
            time -= Time.deltaTime;
            
            yield return null;
        }
    }

}
