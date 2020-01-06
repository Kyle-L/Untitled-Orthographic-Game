using UnityEngine;
using UnityEngine.Serialization;
/// <summary>
/// Controls all of the dialogue options for the npc that Yarn
/// Spinner and the Dialogue Runner will use.
/// </summary>
public class NPCDialogueController : MonoBehaviour {

    [Tooltip("The name of the character.")]
    [SerializeField]
    private string characterName;
    public string CharacterName {
        get {
            return characterName;
        }
    }

    [Tooltip("The node from which the dialogue tree begins.")]
    [FormerlySerializedAs("startNode")]
    [SerializeField]
    private string talkToNode;
    public string TalkToNode {
        get {
            return talkToNode;
        }
    }

    [Tooltip("The script that the dialogue is in." +
        "Adds the script to the dialogue runner if it" +
        "is not yet added.")]
    [Header("Optional")]
    [SerializeField]
    private TextAsset scriptToLoad;

    void Start() {
        // Adds the script to the dialogue runner if it is not null.
        if (scriptToLoad != null) {
            FindObjectOfType<Yarn.Unity.DialogueRunner>().AddScript(scriptToLoad);
        }

    }

}

