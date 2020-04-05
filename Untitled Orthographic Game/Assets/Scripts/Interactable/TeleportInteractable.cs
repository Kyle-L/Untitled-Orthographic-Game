using UnityEngine;

public class TeleportInteractable : Interactable {

    public Transform teleportTransform;

    public override void Go(Controller controller) {
        controller.Teleporter.SetNextTeleport(teleportTransform);
    }

    public override void Stop(Controller controller) {

    }

}
