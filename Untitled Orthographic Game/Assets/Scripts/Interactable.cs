using UnityEngine;

public abstract class Interactable : MonoBehaviour {

    public Transform interactionPoint;
    public string interactionGoString;
    public string interactionStopString;

    public abstract void Go();

    public abstract void Stop();

}
