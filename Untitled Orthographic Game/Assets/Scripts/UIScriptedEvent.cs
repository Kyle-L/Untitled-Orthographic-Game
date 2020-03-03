using UnityEngine;

public class UIScriptedEvent : ScriptedEvent {

    public UIMenu uiMenu;
    
    public override void Go() {
        UIMenuController.instance.SetMenu(uiMenu);
    }
}
