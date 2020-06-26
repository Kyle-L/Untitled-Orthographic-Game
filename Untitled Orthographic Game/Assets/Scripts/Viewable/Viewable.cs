using UnityEngine;

public abstract class Viewable : InteractBase {

    public Transform interactionPoint;

    [Header("UI")]
    public string viewUIActionString;
    public string viewUIObjectString;

    public Viewable leftView;
    public Viewable rightView;

    private void Start() {
        SetString(viewUIActionString, viewUIObjectString);
    }

    public virtual void Go() {
        // Indicates that the player has interacted with this object.
        DialogueVariableStorage.instance.SetValue(this.name, new Yarn.Value(true));
    }

    public virtual void Stop() {
        EnableUI();
    }

}
