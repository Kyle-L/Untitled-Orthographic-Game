using UnityEngine;

/// <summary>
/// An abstract class that represents a triggerable action.
/// </summary>
public abstract class Trigger : MonoBehaviour {
    [Header("Trigger Type")]
    [Tooltip("The type of trigger this is which dictates can it will be triggerd. " +
             "Collider triggers when the player enters/exits triggerable collider attached to this object." +
             "Method triggers when the method ActivateTrigger/Deactivate is called.")]
    public TriggerTypes triggerType = TriggerTypes.Collider;
    public enum TriggerTypes { Collider, Method };

    private protected Collider _collider;

    /// <summary>
    /// Activates the trigger.
    /// </summary>
    public abstract void ActivateTrigger();

    /// <summary>
    /// Deactivates the trigger.
    /// </summary>
    public abstract void DeactivateTrigger();

    public virtual void Start() {
        if (triggerType == TriggerTypes.Collider) {
            _collider = this.GetComponent<Collider>();
        }
    }

    protected void OnTriggerEnter(Collider other) {
        // Ignore triggers.
        if (other.isTrigger) {
            return;
        }
        ActivateTrigger();
    }

    protected void OnTriggerExit(Collider other) {
        DeactivateTrigger();
    }

}
