using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerScene : MonoBehaviour
{

    private Collider col;

    private void Start() {
        col = this.GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other) {
        GameManager.instance.LoadNextScene();
        col.enabled = false;
    }

}
