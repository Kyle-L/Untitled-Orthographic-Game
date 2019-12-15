using UnityEngine;

public class PlayerMovementController2 : MovementController {

    [SerializeField]
    private Camera playerMainCamera;

    public bool Control { get; set; } = true;
    [SerializeField]
    private LayerMask layerMask;

    private RaycastHit _hit;

    private new void Start() {
        base.Start();
    }

    private new void Update() {
        base.Update();

        if (Control) {
            if (Input.GetButton("Fire1")) {
                if (Physics.Raycast(playerMainCamera.ScreenPointToRay(Input.mousePosition), out _hit, 100, layerMask)) {
                    Go(_hit.point);
                }
            }
        }
    }
}
