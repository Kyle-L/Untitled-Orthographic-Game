using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MovementController : MonoBehaviour {

    [Header("Components")]
    [SerializeField]
    protected NavMeshAgent _navMeshAgent;
    protected Animator _animator;

    //States
    protected bool isWalking = false;

    //Default
    protected float speed;

    protected void Start() {
        // Get the animator.
        _animator = GetComponent<Animator>();
        if (_animator == null) {
            _animator = GetComponentInChildren<Animator>();
        }

        _navMeshAgent.updateRotation = true;
    }

    protected void Update() {
        // Updates the animation speed.
        if (_animator != null) {
            _animator.SetFloat("SpeedX", transform.InverseTransformDirection(_navMeshAgent.velocity).x);
            _animator.SetFloat("SpeedY", transform.InverseTransformDirection(_navMeshAgent.velocity).z);
        }
    }

    public void Go (Vector3 destination) {
        _navMeshAgent.SetDestination(destination);
    }

    public void Stop() {
        _navMeshAgent.isStopped = true;
    }

    /// <summary>
    /// Makes the npc face a target position.
    /// </summary>
    /// <param name="target"></param>
    public void FaceFace(Vector3 target) {
        // Stops the npc from being able to move.
        _navMeshAgent.isStopped = true;

        // Calculates the direction by finding the normalized difference.
        Vector3 direction = (target - transform.position).normalized;

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

    Coroutine lookCoroutine;
    /// <summary>
    /// The coroutine the rotates the npc based on the angle.
    /// </summary>
    /// <param name="angle">The angle the npc is rotating towards</param>
    /// <returns></returns>
    private IEnumerator LookAtCoroutine(Quaternion angle) {
        while (Quaternion.Angle(transform.rotation, angle) > 1) {
            transform.rotation = Quaternion.Slerp(transform.rotation, angle, _navMeshAgent.angularSpeed * Time.deltaTime);
            yield return null;
        }
    }

}
