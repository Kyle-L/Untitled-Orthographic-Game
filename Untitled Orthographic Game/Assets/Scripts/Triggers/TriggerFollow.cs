using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerFollow : MonoBehaviour
{

    public Follow follow;

    private void OnTriggerEnter(Collider other) {
        follow.enabled = true;
    }
}
