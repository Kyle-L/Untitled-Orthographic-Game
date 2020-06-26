using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
/// Controls all of the dialogue options for the npc that Yarn
/// Spinner and the Dialogue Runner will use.
/// </summary>
public class NPCDialogueController : InteractBase {

    [Header("UI")]
    [Tooltip("A string which represents the action taken while interacting.")]
    public string interactionUIActionString = "Talk to";
    [Tooltip("A string which represents the object being interacted with.")]
    public string interactionUIObjectString = "NPC";

    [Tooltip("The text box of the character.")]
    public Text characterText;

    [Tooltip("The node from which the dialogue tree begins.")]
    [FormerlySerializedAs("startNode")]
    public string talkToNode;

    private void Start() {
        SetString(interactionUIActionString, interactionUIObjectString);

        characterText.gameObject.SetActive(false);
    }

}

