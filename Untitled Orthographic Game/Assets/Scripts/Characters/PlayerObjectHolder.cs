using System.Collections.Generic;
using UnityEngine;

public class PlayerObjectHolder : MonoBehaviour {

    public Transform objectHolder;
    public List<GameObject> heldItems;
    public HandIKController ik;

    void Start() {

    }

    void Update() {
        if (Input.GetButtonDown("Jump")) {
            foreach (GameObject gameObject in heldItems) {
                gameObject.SetActive(!gameObject.activeSelf);
                ik.IkActive = !ik.IkActive;
            }
        }
    }
}
