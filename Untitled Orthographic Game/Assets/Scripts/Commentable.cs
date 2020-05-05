using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Commentable : MonoBehaviour {

    [Tooltip("The name of the object.")]
    public string commentableUIObjectString = "Unnamed";
    public string startNode;


    public bool disableAfter = true;
    public bool switchTagAfter = true;
    public string switchTag = "Lookable";

    private void Start() {
        this.tag = "Commentable";
    }

    public void Go() {
        if (disableAfter) {
            this.enabled = false;
        }

        if (switchTagAfter) {
            this.tag = switchTag;
        }
    }

}
