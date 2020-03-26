using UnityEngine;

[RequireComponent(typeof(HandIKController))]
public class PlayerObjectHolder : MonoBehaviour {

    private HandIKController _handIKController;

    public enum HoldPositions { LeftCenter, RightCenter, BothCenter, LeftLower, RightLower, BothLower };

    public Transform leftCenter;
    public Transform rightCenter;
    public Transform bothCenter;
    public Transform leftLower;
    public Transform rightLower;
    public Transform bothLower;

    void Start() {
        _handIKController = this.GetComponent<HandIKController>();
    }

    void Update() {

    }

    public void Pickup (Pickupable pickUp) {
        pickUp.pickupObject.parent = GetParentTransform(pickUp.holdPosition);
        pickUp.pickupObject.localPosition = Vector3.zero;
        pickUp.pickupObject.localRotation = Quaternion.Euler(Vector3.zero);

        _handIKController.IkActive = true;

        if (pickUp.holdPosition.ToString().Contains("Left") || pickUp.holdPosition.ToString().Contains("Both")) {
            _handIKController.leftHand = true;
        }

        if (pickUp.holdPosition.ToString().Contains("Right") || pickUp.holdPosition.ToString().Contains("Both")) {
            _handIKController.rightHand = true;
        }

        _handIKController.leftObject = pickUp.pickupLeftHandle;
        _handIKController.rightObject = pickUp.pickupRightHandle;
    }

    private Transform GetParentTransform (HoldPositions pos) {
        switch (pos) {
            case HoldPositions.LeftCenter:
                return leftCenter;
            case HoldPositions.RightCenter:
                return rightCenter;
            case HoldPositions.BothCenter:
                return bothCenter;
            case HoldPositions.LeftLower:
                return leftLower;
            case HoldPositions.RightLower:
                return rightLower;
            case HoldPositions.BothLower:
                return bothLower;
            default:
                return bothCenter;
        }
    }

}
