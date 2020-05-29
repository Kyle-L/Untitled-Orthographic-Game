using UnityEngine;

public class TriggerAnimation : Trigger {

    public Animator animator;
    public string triggerIn;
    public string triggerOut;
    public bool disableOnTrigger = false;

    private void OnTriggerEnter(Collider other) {
        ActivateTrigger();
    }

    private void OnTriggerExit(Collider other) {
        DeactivateTrigger();
    }

    public override void ActivateTrigger() {
        animator.SetTrigger(triggerIn);
        if (triggerType == TriggerTypes.Collider) {
            _collider.enabled = _collider.enabled && !disableOnTrigger;
        }
    }

    public override void DeactivateTrigger() {
        animator.SetTrigger(triggerOut);
        if (triggerType == TriggerTypes.Collider) {
            _collider.enabled = _collider.enabled && !disableOnTrigger;
        }
    }
}
