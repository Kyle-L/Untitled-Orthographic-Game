using UnityEngine;

public class UIFadeTransitionController : MonoBehaviour {
    public static UIFadeTransitionController instance;

    public Animator _animator;
    public string fadeInTrigger;
    public string fadeOutTrigger;

    private void Awake() {
        #region Enforces Singleton Pattern.
        //Check if instance already exists
        if (instance == null) {
            //if not, set instance to this	
            instance = this;
        }
        //If instance already exists and it's not this:
        else if (instance != this) {
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a MyNetworkManager.

            Destroy(gameObject);
        }
        #endregion
        _animator.speed = 0.25f;
    }

    public bool GetTransition() {
        return _animator.GetBool("Transition");
    }

    public void FadeIn() {
        _animator.SetBool("Transition", true);
        _animator.SetTrigger(fadeInTrigger);
    }

    public void FadeOut() {
        _animator.SetBool("Transition", true);
        _animator.SetTrigger(fadeOutTrigger);
    }

    public void Done() {
        _animator.SetBool("Transition", false);
    }
}
