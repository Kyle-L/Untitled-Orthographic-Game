using UnityEngine;
using Yarn.Unity;

public class PlayerMovementController : MovementController {

    public Camera playerMainCamera;

    public bool Control { get; set; } = true;

    public void Move(Vector2 dir) {
        if (Control) {
            if (dir.sqrMagnitude > 0) {
                if (!DialogueRunner.instance.isDialogueRunning && PlayerControllerMain.instance.isAlive) {
                    agentControlled = false;
                    PlayerControllerMain.instance.SetState(Controller.States.UserControlled);
                }
            }

            if (!agentControlled && IsAnimating()) {
                // Movement
                float vertical = dir.y;
                float horizontal = dir.x;

                //Converts user input to a direction.
                direction = new Vector3(horizontal, 0f, vertical);

                //Makes direction relative to camera.
                direction = playerMainCamera.transform.TransformDirection(direction);
                direction.y = 0.0f;

                direction = Vector3.Normalize(direction);
            }
        }
    }

}
