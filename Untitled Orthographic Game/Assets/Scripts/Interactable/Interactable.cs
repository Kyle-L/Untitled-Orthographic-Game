using UnityEngine;

public abstract class Interactable : MonoBehaviour {

    [Header("UI")]
    [Tooltip("A string which represents the action taken while interacting.")]
    public string interactionUIActionString;
    [Tooltip("A string which represents the object being interacted with.")]
    public string interactionUIObjectString;

    [Header("Transforms")]
    [Tooltip("Where the character should align with at the start of the interaction.")]
    public Transform interactionPoint;
    [Tooltip("Where the character's left hand should align with at the start of the interaction.")]
    public Transform leftHandPoint;
    [Tooltip("Where the character's right hand should align with at the start of the interaction.")]
    public Transform righthandPoint;

    [Header("Animation")]
    [Tooltip("A string which represents the animation of an interaction on Go (Can represent trigger, animation name, etc.).")]
    public string animationGoString;
    [Tooltip("A string which represents the animation of an interaction on Stop (Can represent trigger, animation name, etc.).")]
    public string animationStopString;

    [Header("Triggers")]
    [Tooltip("A trigger that is called on Go.")]
    public Trigger goTrigger;
    [Tooltip("A trigger that is called on Stop.")]
    public Trigger stopTrigger;

    public virtual void Go(Controller controller) {
        goTrigger?.ActivateTrigger();
    }

    public virtual void Stop(Controller controller) {
        stopTrigger?.ActivateTrigger();
    }

}
