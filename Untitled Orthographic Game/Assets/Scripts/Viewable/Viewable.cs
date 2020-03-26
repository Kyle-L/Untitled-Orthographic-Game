using UnityEngine;

public abstract class Viewable : MonoBehaviour {

    public Transform interactionPoint;

    [Header("UI")]
    public string viewUIActionString;
    public string viewUIObjectString;

    public Viewable leftView;
    public Viewable rightView;

    public abstract void Go();

    public abstract void Stop();

}
