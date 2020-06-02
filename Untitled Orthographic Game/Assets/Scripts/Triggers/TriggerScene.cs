using System.Collections;
using UnityEngine;

public class TriggerScene : Trigger {

    public float delay = 0;

    private void OnTriggerEnter(Collider other) {
        ActivateTrigger();
    }

    public override void ActivateTrigger() {
        StartCoroutine(WaitToActivate(delay));
    }

    public override void DeactivateTrigger() {
        throw new System.NotImplementedException();
    }

    public IEnumerator WaitToActivate (float delay) {
        yield return new WaitForSeconds(delay);
        GameManager.instance.LoadNextScene();
        _collider.enabled = false;
    }
}
