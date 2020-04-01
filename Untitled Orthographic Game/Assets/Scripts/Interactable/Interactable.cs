using UnityEngine;

public abstract class Interactable : MonoBehaviour {

    [Header("UI")]
    public string interactionUIActionString;
    public string interactionUIObjectString;

    [Header("Transforms")]
    public Transform interactionPoint;
    public Transform leftHandPoint;
    public Transform righthandPoint;

    [Header("Animation")]
    public string interactionGoString;
    public string interactionStopString;



    public abstract void Go(Controller controller);

    public abstract void Stop(Controller controller);

}
