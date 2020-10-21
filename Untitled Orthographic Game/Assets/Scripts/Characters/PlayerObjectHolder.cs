using System.Collections;
using UnityEngine;

[RequireComponent(typeof(HandIKController))]
public class PlayerObjectHolder : MonoBehaviour {

    [Header("Defaults")]
    public Pickupable startHold;
    private Pickupable currentHeld;

    public enum HoldPositions { LeftCenter, RightCenter, BothCenter, LeftLower, RightLower, BothLower };
    [Header("Hold Positions")]
    public Transform leftCenter;
    public Transform rightCenter;
    public Transform bothCenter;
    public Transform leftLower;
    public Transform rightLower;
    public Transform bothLower;

    // Components
    private HandIKController _handIKController;
    private Animator _animator;

    void Start() {
        _handIKController = this.GetComponent<HandIKController>();
        _animator = this.GetComponent<Animator>();

        if (startHold != null) {
            SetPickup(startHold, false);
        }
    }

    void Update() {
        //if (Input.GetButtonDown("Jump")) {
        //    Drop();
        //}
    }

    public void SetPickup(Pickupable pickUp, bool animation = true) {
        currentHeld = pickUp;

        if (animation) {
            _animator.SetTrigger("Pickup");

            _animator.SetFloat("PickupHeight", Mathf.Clamp((pickUp.transform.position.y - transform.position.y) / 2, 0, 1));
        } else {
            GrabObject();
            GrabArms();
        }


    }

    public void GrabArms() {

        if (currentHeld.holdPosition.ToString().Contains("Left") || currentHeld.holdPosition.ToString().Contains("Both")) {
            _handIKController.leftHand = true;
        }

        if (currentHeld.holdPosition.ToString().Contains("Right") || currentHeld.holdPosition.ToString().Contains("Both")) {
            _handIKController.rightHand = true;
        }

        _handIKController.leftObject = currentHeld.pickupLeftHandle;
        _handIKController.rightObject = currentHeld.pickupRightHandle;

        _handIKController.LerpPositon(1);
    }

    public void GrabObject() {
        currentHeld.Pickup(GetParentTransform(currentHeld.holdPosition));
        LerpPositon(currentHeld);
        _handIKController.LerpRotation(1);
        _handIKController.LerpPositon(1);
    }

    public void Drop() {
        _handIKController.LerpPositon(0);
        _handIKController.LerpRotation(0);
        currentHeld?.Drop();

        _handIKController.leftHand = false;
        _handIKController.rightHand = false;
        _handIKController.leftObject = null;
        _handIKController.rightObject = null;
    }

    private Transform GetParentTransform(HoldPositions pos) {
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

    private void LerpPositon(Pickupable pickUp) {
        StartCoroutine(LerpPositionCoroutine(pickUp));
    }

    private IEnumerator LerpPositionCoroutine(Pickupable pickUp) {
        while (Vector3.Distance(pickUp.pickupObject.localPosition, Vector3.zero) > 0.01f) {
            pickUp.pickupObject.localPosition = Vector3.Lerp(pickUp.pickupObject.localPosition, Vector3.zero, Time.deltaTime * 5);
            pickUp.pickupObject.localRotation = Quaternion.Euler(Vector3.Lerp(pickUp.pickupObject.localPosition, Vector3.zero, Time.deltaTime * 5));
            yield return null;
        }
    }

}
