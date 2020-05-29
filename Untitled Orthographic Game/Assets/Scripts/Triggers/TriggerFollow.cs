using UnityEngine;

public class TriggerFollow : Trigger {

    [Header("Follow")]
    public Follow followComponent;
    public Transform followTarget;
    public bool enableFollow = true;

    public override void ActivateTrigger() {
        followComponent.enabled = enableFollow;
        followComponent.target = followTarget;
    }

    public override void DeactivateTrigger() {
        followComponent.enabled = !enableFollow;
        followComponent.target = null;
    }

    private void OnTriggerEnter(Collider other) {
        ActivateTrigger();
    }
}
