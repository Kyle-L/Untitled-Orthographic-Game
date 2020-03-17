using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class HeadIKController : MonoBehaviour {

    // Constants.
    private const float RAYCAST_MAX_DISTANCE = 5f;
    private const float MIN_LOOK_THRESHOLD = 0.01f;

    [Header("Transforms")]
    public Transform head;

    [Header("Look Weight")]
    [Range(0, 1)]
    public float lookAtMaxWeight = 1;
    [Range(0, 1)]
    public float lookAtMinWeight = 0;
    private float lookAtWeight = 1;

    [Header("Misc.")]
    public float distanceToLook = -1;
    public float lookSpeed = 3;
    public float weightSpeedStop = 1;
    public float weightSpeedStart = 1;

    // Components.
    private Animator _animator;

    // Frequently used variables.
    private Transform currentLookTrans;
    private Vector3 lookPos;
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
        // Creates a ray from the head of the character to the look at transform.
        Ray _ray = new Ray(head.position, lookPos);

        /* Calculates whether the look at object is in front or behind.
         * This is found by taking the inverse transform point and looking
         * at the axis.*/
        float front = transform.InverseTransformPoint(lookPos).z;

        /* In order for to look at something, the object needs to be visible 
         * and in front of the character. */
        if (/*Physics.Raycast(_ray, RAYCAST_MAX_DISTANCE) &&*/ front > distanceToLook && currentLookTrans != null) {
            /* If the object is in front, lerp the weight to the max.
             * The lerp is there so the character doesn't abruptly look.*/
            lookAtWeight = Mathf.Lerp(lookAtWeight, lookAtMaxWeight, weightSpeedStart * Time.deltaTime);
        } else {
            /* If the object is in front, lerp the weight to the max.
             * The lerp is there so the character doesn't abruptly look.*/
            lookAtWeight = Mathf.Lerp(lookAtWeight, lookAtMinWeight, weightSpeedStop * Time.deltaTime);
        }

        Debug.DrawLine(head.position, lookPos);
    }

    /// <summary>
    /// Makes the player look at a particular transform.
    /// </summary>
    /// <param name="trans"></param>
    public void LookAt(Transform trans) {
        if (trans == null) {
            return;
        }

        if (trans == currentLookTrans) {
            if (Vector3.Distance(lookPos, trans.position) <= MIN_LOOK_THRESHOLD) {
                return;
            }
        }

        currentLookTrans = trans;

        /* If the transform is behind the player, set it to be the current place the
        * character is looking. */
        lookPos = (lookPos - head.position).normalized + head.position;

        // Stop the existing coroutine.
        if (lookCoroutine != null) {
            StopCoroutine(lookCoroutine);
        }

        // Start the coroutine.
        lookCoroutine = StartCoroutine(Look(currentLookTrans));
    }

    /// <summary>
    /// The coroutine that ensures the character slowly turns their head to look at the transform
    /// rather than abruptly looking at the transform.
    /// </summary>
    /// <param name="trans"></param>
    /// <returns></returns>
    private IEnumerator Look(Transform trans) {
        while (Vector3.Distance(lookPos, trans.position) > MIN_LOOK_THRESHOLD) {
            lookPos = Vector3.Lerp(lookPos, trans.position, lookSpeed * Time.deltaTime);
            yield return null;
        }
    }

    public Transform GetClosestInFrontTransform(ICollection<Transform> transforms) {
        // Order all transforms by distance, least to greatest.
        IOrderedEnumerable<Transform> nClosest = transforms.OrderBy(t => (t.position - transform.position).sqrMagnitude);

        // Then order all transforms by whether they are in front of the root transform or behind.
        nClosest = nClosest.ThenByDescending(t => (transform.InverseTransformPoint(t.position).z));

        if (nClosest.First().Equals(head)) {
            if (nClosest.Count() >= 2) {
                return nClosest.Skip(1).First();
            } else {
                return null;
            }
        }

        // Return the the first element which should be the closest and in front of the root transform.
        return nClosest.First();
    }

}