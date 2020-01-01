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

    [Header("Transforms")]
    public Transform head;

    // Components.
    private Animator _animator;

    // Frequently used variables.
    private GameObject lookAtObj;
    private RaycastHit _hit;

    private void Start() {
        _animator = GetComponent<Animator>();

        lookAtObj = new GameObject("\"" + this.name + "\" look at object");
        lookAtObj.transform.position = transform.TransformDirection(head.forward);
    }

    private void OnAnimatorIK() {
        if (_animator) {
            if (lookAtObj != null) {
                _animator.SetLookAtWeight(lookAtWeight);
                _animator.SetLookAtPosition(lookAtObj.transform.position);
            }
        }
    }

    private void Update() {
        // Creates a ray from the head of the character to the look at position.
        Ray _ray = new Ray(head.position, lookAtObj.transform.position - head.position);

        /* Calculates whether the look at object is in front or behind.
         * This is found by taking the inverse transform point and looking
         * at the axis.*/
        float front = transform.InverseTransformPoint(lookAtObj.transform.position).z;

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

    public void LookAt(Vector3 pos) {
        if (coroutine != null) {
            StopCoroutine(coroutine);
        }
        coroutine = StartCoroutine(Look(pos));
    }

    private Coroutine coroutine;

    private IEnumerator Look (Vector3 pos) {
        while (Vector3.Distance(lookAtObj.transform.position, pos) > 0.01f) {
            lookAtObj.transform.position = Vector3.Lerp(lookAtObj.transform.position, pos, lookSpeed * Time.deltaTime);
            yield return null;
        }
    }

}