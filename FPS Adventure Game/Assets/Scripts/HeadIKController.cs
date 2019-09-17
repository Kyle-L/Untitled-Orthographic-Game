using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class HeadIKController : MonoBehaviour {

    // Constants
    private const float RAYCAST_MAX_DISTANCE = 5f;

    [SerializeField]
    [Range(0, 1)]
    private float lookAtWeight = 1;

    private Animator _animator;
    [SerializeField]
    private Transform lookAtObj;

    void Start() {
        _animator = GetComponent<Animator>();
    }

    void OnAnimatorIK() {
        if (_animator) {
            if (lookAtObj != null) {
                _animator.SetLookAtWeight(lookAtWeight);
                _animator.SetLookAtPosition(lookAtObj.position);
            }
        }

    }

}
