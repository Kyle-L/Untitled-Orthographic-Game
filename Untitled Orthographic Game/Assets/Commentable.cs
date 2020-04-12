using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Commentable : MonoBehaviour {

    public string name;
    public string startNode;
    public TextAsset scriptToLoad;
    public bool disableAfter = true;
    public bool switchTagAfter = true;
    public string switchTag = "Lookable";

    private void Start() {
        if (scriptToLoad != null) {
            FindObjectOfType<Yarn.Unity.DialogueRunner>().AddScript(scriptToLoad);
        }
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
