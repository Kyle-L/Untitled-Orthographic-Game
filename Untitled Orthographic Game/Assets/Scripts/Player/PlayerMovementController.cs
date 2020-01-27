using UnityEngine;

public class PlayerMovementController : MovementController {


    public Camera playerMainCamera;
    public Transform target;

    public bool Control { get; set; } = true;

    private new void Update() {
        direction = Vector3.zero;

        #region Movement
        if (Control) {
            if (Input.GetButtonDown("Jump")) {
                agentControlled = true;
                _navMeshAgent.SetDestination(target.position);
            } else if (Mathf.Abs(Input.GetAxis("Vertical")) > 0 || Mathf.Abs(Input.GetAxis("Horizontal")) > 0) {
                agentControlled = false;
            }

            if (!agentControlled) {
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
