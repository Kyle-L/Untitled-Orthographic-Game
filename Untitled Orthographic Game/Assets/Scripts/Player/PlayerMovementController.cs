using UnityEngine;

public class PlayerMovementController : MovementController {


    public Camera playerMainCamera;
    public Transform target;

    public bool Control { get; set; } = true;

    public AnimatedInteractable interactableTest;

    private new void Update() {
        direction = Vector3.zero;

        #region Movement
        if (Control) {
            if (Input.GetButtonDown("Jump")) {
                agentControlled = true;
                PlayerControllerMain.instance.ModifyBlackBoard(Controller.BlackBoardVars.InteractingObject, interactableTest);
                PlayerControllerMain.instance.ModifyBlackBoard(Controller.BlackBoardVars.State, Controller.States.Interacting);
                PlayerControllerMain.instance.currentState = Controller.States.Interacting;
            } else if (Mathf.Abs(Input.GetAxis("Vertical")) > 0 || Mathf.Abs(Input.GetAxis("Horizontal")) > 0) {
                agentControlled = false;
                PlayerControllerMain.instance.ModifyBlackBoard(Controller.BlackBoardVars.State, Controller.States.UserControlled);
                PlayerControllerMain.instance.currentState = Controller.States.UserControlled;
            }

            if (!agentControlled && _animator.GetCurrentAnimatorStateInfo(0).IsName("Movement") || _animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")) {
                // Movement
                float vertical = Input.GetAxis("Vertical");
                float horizontal = Input.GetAxis("Horizontal");

                //Converts user input to a direction.
                direction = new Vector3(horizontal, 0f, vertical);

                //Makes direction relative to camera.
                direction = playerMainCamera.transform.TransformDirection(direction);
                direction.y = 0.0f;

                direction = Vector3.Normalize(direction);
            }

        }

        base.Update();

        #endregion
    }

}
