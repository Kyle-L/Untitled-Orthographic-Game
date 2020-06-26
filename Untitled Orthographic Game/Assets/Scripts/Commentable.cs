using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Commentable : InteractBase {

    [Tooltip("The name of the object.")]
    public string commentableUIActionString = "Comment on";
    public string commentableUIObjectString = "Unnamed";
    public string startNode;
    public bool addToDialogueStorage = false;

    public bool disableAfter = true;
    public bool switchTagAfter = true;
    public string switchTag = "Lookable";

    private void Start() {
        SetString(commentableUIActionString, commentableUIObjectString);

        this.tag = "Commentable";
    }

    public void Go() {
        // Indicates that the player has interacted with this object.
        DialogueVariableStorage.instance.SetValue(this.name, new Yarn.Value(true));

        if (disableAfter) {
            this.enabled = false;
        }

        DisableUI();

        if (switchTagAfter) {
            this.tag = switchTag;
        }
    }

}
