using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollDetect : MonoBehaviour
{

    private NPCMovementController movement;

    private void Start() {
        movement = GetComponentInParent<NPCMovementController>();
    }

    private void OnCollisionEnter(Collision collision) {
        movement.RagDoll();
    }
}
