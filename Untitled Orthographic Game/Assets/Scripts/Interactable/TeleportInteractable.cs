using UnityEngine;

public class TeleportInteractable : Interactable {

    public Transform teleportTransform;
    public float cameraAngle;
    public float cameraSize;
    public float cameraHeight;

    public override void Go(Controller controller) {
        controller.Teleporter.SetNextTeleport(teleportTransform);

    }

    public override void Stop(Controller controller) {
        CameraController.instance.SetSize(cameraSize);
        CameraController.instance.SetAngle(cameraAngle);
        CameraController.instance.SetHeight(cameraHeight);
    }

}
