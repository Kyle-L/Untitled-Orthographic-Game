using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerAnimation : MonoBehaviour
{

    public Animator animator;
    public string trigger;
    bool disableTrigger = true;

    private Collider col;

    private void Start() {
        col = this.GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other) {
        animator.SetTrigger(trigger);

        if (disableTrigger) {
            col.enabled = false;
        }
    }

}
