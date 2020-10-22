using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Yarn.Unity;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputController : MonoBehaviour {

    private PlayerInput _playerInput;
    [SerializeField] private MouseStateController mouseStateController = null;

    [SerializeField] private CameraController controlledCamera = null;
    [SerializeField] private PlayerControllerMain[] controlledPlayer;
    [SerializeField] private UIDialogue uiDialogue = null;

    private Vector2 move;
    private Vector2 look;
    private bool fire1;
    private bool fire2;
    private bool zoom;

    private void Awake() {
        _playerInput = this.GetComponent<PlayerInput>();

        _playerInput.controlsChangedEvent.AddListener(OnDeviceChange);
    }

    public void Update() {
        if (controlledPlayer.Length == 0) {
            return;
        }

        foreach (PlayerControllerMain player in controlledPlayer) {
            player.PlayerMovementController.Move(move);
        }

        if (controlledCamera == null) {
            return;
        }

        // Gamepad specific controls.
        if (_playerInput.currentControlScheme == "Gamepad") {
            if (zoom) {
                controlledCamera.Zoom(look.y);
            } else {
                controlledCamera.Look(look);
            }

            // Keyboard specific controls.
        } else {
            if (zoom) {
                controlledCamera.Zoom(look.y);
            } else if (fire2) {
                controlledCamera.Look(look);
            }
        }
    }

    public void OnDeviceChange (PlayerInput input) {
        if (_playerInput.currentControlScheme == "Gamepad") {
            UIMenuController.instance.UseEventSystem = true;
        } else {
            UIMenuController.instance.UseEventSystem = false;
        }
    }

    public void OnMove(InputAction.CallbackContext context) {
        if (controlledPlayer.Length == 0) {
            return;
        }

        move = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context) {
        if (controlledPlayer.Length == 0) {
            return;
        }

        look = context.ReadValue<Vector2>();
    }

    public void OnFire1(InputAction.CallbackContext context) {
        if (!context.performed) {
            return;
        }

        if (DialogueRunner.instance.isDialogueRunning) {
            uiDialogue.DialogueContinue();
        } else {
            foreach (PlayerControllerMain player in controlledPlayer) {
                player.PlayerInteractionController.Interact();
            }
        }
    }

    public void OnFire2(InputAction.CallbackContext context) {
        if (controlledPlayer.Length == 0) {
            return;
        }

        fire2 = context.performed;
    }

    public void OnZoom(InputAction.CallbackContext context) {
        if (controlledCamera == null) {
            return;
        }

        zoom = context.performed;
    }

    public void OnPause(InputAction.CallbackContext context) {
        if (!context.performed) {
            return;
        }

        if (UIMenuController.instance.GetCurrentMenu() == null) {
            UIMenuController.instance.PauseMenu();
        } else {
            UIMenuController.instance.SetMenu(UIMenuController.MainMenus.NoMenu);
        }
    }

    public void OnCancel (InputAction.CallbackContext context) {
        if (!context.performed) {
            return;
        }

        if (UIMenuController.instance.GetMenuState()) {
            UIMenuController.instance.Back();
        }
    }
}
