using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Linq;

public class PlayerMovementController : MovementController {


    public Camera playerMainCamera;
    public Transform target;

    private List<Transform> allParticipants;

    public bool Control { get; set; } = true;

    private new void Start () {
        base.Start();

        //allParticipants = new List<GameObject>(GameObject.FindGameObjectsWithTag("Lookable"));
        allParticipants = new List<Transform>();
        allParticipants.AddRange(GameObject.FindGameObjectsWithTag("Lookable").Select(go => go.transform));
        allParticipants.Remove(_headIKController.head.transform);
    }

    private new void Update() {
        direction = Vector3.zero;

        LookAtRandom(allParticipants.ToArray());

        #region Movement
        if (Control) {
            if (Input.GetButtonDown("Jump")) {
                agentControlled = true;
                SetLocation(target);
            }  else if (Mathf.Abs(Input.GetAxis("Vertical")) > 0 || Mathf.Abs(Input.GetAxis("Horizontal")) > 0) {
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
