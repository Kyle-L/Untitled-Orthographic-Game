using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
/// Controls all of the dialogue options for the npc that Yarn
/// Spinner and the Dialogue Runner will use.
/// </summary>
public class NPCDialogueController : MonoBehaviour {

    [Tooltip("The name of the character.")]
    public string characterName;

    public Text characterText;

    [Tooltip("The node from which the dialogue tree begins.")]
    [FormerlySerializedAs("startNode")]
    public string talkToNode;

    [Tooltip("The script that the dialogue is in." +
        "Adds the script to the dialogue runner if it" +
        "is not yet added.")]
    [Header("Optional")]
    public TextAsset scriptToLoad;

    void Start() {
        // Adds the script to the dialogue runner if it is not null.
        if (scriptToLoad != null) {
            FindObjectOfType<Yarn.Unity.DialogueRunner>().AddScript(scriptToLoad);
        }

    }

}

