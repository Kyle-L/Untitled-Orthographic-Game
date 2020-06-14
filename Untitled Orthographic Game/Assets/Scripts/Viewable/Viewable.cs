using UnityEngine;

public abstract class Viewable : MonoBehaviour {

    public Transform interactionPoint;

    [Header("UI")]
    public string viewUIActionString;
    public string viewUIObjectString;

    public Viewable leftView;
    public Viewable rightView;

    public virtual void Go() {
        // Indicates that the player has interacted with this object.
        DialogueVariableStorage.instance.SetValue(this.name, new Yarn.Value(true));
    }

    public abstract void Stop();

}
