using UnityEngine;

public class TriggerScene : Trigger {

    private void OnTriggerEnter(Collider other) {
        ActivateTrigger();
    }

    public override void ActivateTrigger() {
        GameManager.instance.LoadNextScene();
        _collider.enabled = false;
    }

    public override void DeactivateTrigger() {
        throw new System.NotImplementedException();
    }
}
