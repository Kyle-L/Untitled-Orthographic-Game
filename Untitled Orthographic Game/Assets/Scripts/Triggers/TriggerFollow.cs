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
        if (triggerType == TriggerTypes.Collider) {
            _collider.enabled = false;
        }
    }

    public override void DeactivateTrigger() {
        followComponent.FollowPosition = false;
        followComponent.LookAt = false;
        //followComponent.target = null;
    }

}
