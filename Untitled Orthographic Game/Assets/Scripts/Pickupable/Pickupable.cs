using UnityEngine;

public class Pickupable : MonoBehaviour {

    [Header("UI")]
    [Tooltip("The action that is displayed for the user when picking the object up.")]
    public string pickUpUIActionString;
    [Tooltip("The name of the object.")]
    public string pickUpUIObjectString;

    [Header("Transforms")]
    [Tooltip("The main transform of the object being picked up. If not assigned, the transform of this script will be the default.")]
    public Transform pickupObject;
    [Tooltip("A sub-transform of the object that will be where the player grabs the object with their left hand. If not assigned, the transform of this script will be the default.")]
    public Transform pickupLeftHandle;
    [Tooltip("A sub-transform of the object that will be where the player grabs the object with their right hand. If not assigned, the transform of this script will be the default.")]
    public Transform pickupRightHandle;

    [Tooltip("Where the player will hold the object relative to their own body.")]
    public PlayerObjectHolder.HoldPositions holdPosition;

    private void Awake() {
        if (pickupObject == null) {
            pickupObject = this.transform;
        }

        if (pickupLeftHandle == null) {
            pickupLeftHandle = this.transform;
        }

        if (pickupRightHandle == null) {
            pickupRightHandle = this.transform;
        }
    }

}
