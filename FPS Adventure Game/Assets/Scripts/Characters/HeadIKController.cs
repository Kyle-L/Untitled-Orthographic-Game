using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class HeadIKController : MonoBehaviour {

    // Constants.
    private const float RAYCAST_MAX_DISTANCE = 5f;

    [Header("Look Weight")]
    [Range(0, 1)]
    public float lookAtMaxWeight = 1;
    [Range(0, 1)]
    public float lookAtMinWeight = 0;
    private float lookAtWeight = 1;

    [Header("Misc.")]
    public float distanceToLook = -1;
    public float lookSpeed = 3;
    public float weightSpeed = 1;

    // Components.
    private Animator _animator;

    // Frequently used variables.
    private Vector3 lookPos;
    private RaycastHit _hit;
    private Coroutine lookCoroutine;

    private void Start() {
        _animator = GetComponent<Animator>();
    }

    private void OnAnimatorIK() {
        if (_animator) {
            _animator.SetLookAtPosition(lookPos);
            _animator.SetLookAtWeight(lookAtWeight);
        }
    }

    private void Update() {
        // Creates a ray from the head of the character to the look at position.
        Ray _ray = new Ray(transform.position, lookPos - transform.position);

        /* Calculates whether the look at object is in front or behind.
         * This is found by taking the inverse transform point and looking
         * at the axis.*/
        float front = transform.InverseTransformPoint(lookPos).z;

        /* In order for to look at something, the object needs to be visible 
         * and in front of the character. */ 
        if (Physics.Raycast(_ray, RAYCAST_MAX_DISTANCE) && front > distanceToLook) {
            /* If the object is in front, lerp the weight to the max.
             * The lerp is there so the character doesn't abruptly look.*/
            lookAtWeight = Mathf.Lerp(lookAtWeight, lookAtMaxWeight, weightSpeed * Time.deltaTime);
        } else {
            /* If the object is in front, lerp the weight to the max.
             * The lerp is there so the character doesn't abruptly look.*/
            lookAtWeight = Mathf.Lerp(lookAtWeight, lookAtMinWeight, weightSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// Makes the player look at a particular position.
    /// </summary>
    /// <param name="pos"></param>
    public void LookAt(Vector3 pos) {
        /* Calculates whether the look at object is in front or behind.
         * This is found by taking the inverse transform point and looking
         * at the axis.*/
        float front = transform.InverseTransformPoint(lookPos).z;

        /* If the position is behind the player, set it to be the current place the
         * character is looking. */
        if (front < distanceToLook) {
            lookPos = transform.forward;
        }

        // Stop the existing coroutine.
        if (lookCoroutine != null) {
            StopCoroutine(lookCoroutine);
        }

        // Start the coroutine.
        lookCoroutine = StartCoroutine(Look(pos));
    }

    /// <summary>
    /// The coroutine that ensures the character slowly turns their head to look at the position
    /// rather than abruptly looking at the position.
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    private IEnumerator Look (Vector3 pos) {
        while (Vector3.Distance(lookPos, pos) > 0.01f) {
            lookPos = Vector3.Lerp(lookPos, pos, lookSpeed * Time.deltaTime);
            yield return null;
        }
    }

}