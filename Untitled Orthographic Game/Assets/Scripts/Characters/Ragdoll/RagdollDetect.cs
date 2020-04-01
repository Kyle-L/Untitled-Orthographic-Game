using UnityEngine;

public class RagdollDetect : MonoBehaviour {

    private Controller movement;

    private void Start() {
        movement = GetComponentInParent<Controller>();
    }

    private void OnCollisionEnter(Collision collision) {
        //movement.Die();
    }
}
