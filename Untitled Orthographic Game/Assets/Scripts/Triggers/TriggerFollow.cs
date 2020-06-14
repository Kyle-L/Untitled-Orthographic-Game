using UnityEngine;

public class TriggerFollow : Trigger {

    [Header("Follow")]
    public Follow followComponent;
    public Transform followTarget;
    public bool enableFollow = true;
    public bool enableLookAt = true;

    public override void ActivateTrigger() {
        followComponent.FollowPosition = enableFollow;
        followComponent.LookAt = enableLookAt;
        followComponent.target = followTarget;
    }

    public override void DeactivateTrigger() {
        followComponent.FollowPosition = false;
        followComponent.LookAt = false;
        //followComponent.target = null;
    }

    private void OnTriggerEnter(Collider other) {
        ActivateTrigger();
    }
}
