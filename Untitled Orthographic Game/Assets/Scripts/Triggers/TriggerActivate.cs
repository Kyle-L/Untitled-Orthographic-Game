using UnityEngine;

public class TriggerActivate : Trigger {

    public GameObject[] objects;
    public bool[] activation;

    public override void ActivateTrigger() {
        if (objects.Length != activation.Length) {
            print("objects and activation must have the same length!");
        }

        for (int i = 0; i < objects.Length; i++) {
            objects[i].SetActive(activation[i]);
        }

        if (triggerType == TriggerTypes.Collider) {
            _collider.enabled = false;
        }
    }

    public override void DeactivateTrigger() {
        throw new System.NotImplementedException();
    }

}
