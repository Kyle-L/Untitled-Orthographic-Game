using UnityEngine;

public class Teleporter : MonoBehaviour {

    [Header("General")]
    private bool nextFade = false;
    private bool nextPosSet = false;
    private Transform nextPosition;

    [Header("Audio")]
    public AudioClip audioClip;
    public AudioSource _audioSource;

    public void SetNextTeleport (Transform teleportPos, bool fade = true) {
        nextPosition = teleportPos;
        nextPosSet = true;
        nextFade = fade;
        if (nextFade) {
            UIFadeTransitionController.instance.FadeIn(1);
        }
    }

    public void Teleport () {
        if (!nextPosSet) {
            print("The next teleport position has not been set. Aborting teleport...");
            return;
        }

        if (nextFade) {
            UIFadeTransitionController.instance.FadeOut(0.5f);
            nextFade = false;
        }


        PlayerControllerMain.instance.MovementController.SetPosition(nextPosition);
        PlayerControllerMain.instance.SetState(Controller.States.UserControlled);
        nextPosSet = false;
    }

}
