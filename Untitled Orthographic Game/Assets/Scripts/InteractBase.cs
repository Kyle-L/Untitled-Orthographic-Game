using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))]
public abstract class InteractBase : MonoBehaviour
{

    [Header("Interact Base")]
    public GameObject prefab;
    public Transform prefabPosition;
    [Range(1, 50)]
    public float sizeMultiplier = 1;
    private string actionStr;
    private string objStr;
    private const float DEFAULT_OFFSET = 0.125f;

    GameObject ui;
    Animator _animator;
    Text _text;

    protected void Awake () {
        float colliderTop = GetComponent<Collider>().bounds.max.y;

        Vector3 position = (prefabPosition != null) ? prefabPosition.position : transform.position + new Vector3(0, DEFAULT_OFFSET + (colliderTop - transform.position.y), 0);
        Quaternion rotation = (prefabPosition != null) ? prefabPosition.rotation : transform.rotation;

        ui = Instantiate((prefab == null) ? PrefabController.instance.interactBaseUIPrefab : prefab, position, rotation);

        if (prefabPosition != null) {
            float x = ui.transform.localScale.x * sizeMultiplier;
            float y = ui.transform.localScale.y * sizeMultiplier;
            float z = ui.transform.localScale.z * sizeMultiplier;

            ui.transform.localScale = new Vector3(x, y, z);

            ui.transform.SetParent(prefabPosition);
        } else {
            ui.transform.SetParent(this.transform);
        }

        _animator = ui.GetComponentInChildren<Animator>();
        _text = ui.GetComponentInChildren<Text>();
        SetString(actionStr, objStr);
    }

    public void UnhideUI() {
        _animator.SetBool("Hide", false);
    }

    public void FocusUI() {
        _animator.SetBool("Interacting", true);
    }

    public void DefocusUI() {
        _animator.SetBool("Interacting", false);
    }

    public void HideUI() {
        _animator.SetBool("Hide", true);
    }

    public void EnableUI () {
        _animator.SetBool("Disable", false);
    }

    public void DisableUI () {
        _animator.SetBool("Disable", true);
    }

    public void SetString (string action, string obj) {
        actionStr = action;
        objStr = obj;
        if (_text != null) _text.text = action + " " + obj;
    }
}
