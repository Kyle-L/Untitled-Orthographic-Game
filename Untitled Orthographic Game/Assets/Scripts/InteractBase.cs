using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))]
public abstract class InteractBase : MonoBehaviour
{

    [Header("Interact Base")]
    public Transform prefabPosition;

    private const float DEFAULT_OFFSET = 0.125f;

    Animator _animator;
    Text _text;

    protected void Start() {
        //if (prefabPosition == null) {
        //    print(this.name + " has an interaction base but does not have an assigned prefab position. Please assign one.");
        //}

        // Gets the top of the collider.
        float colliderTop = GetComponent<Collider>().bounds.max.y;

        Vector3 position = (prefabPosition != null) ? prefabPosition.position : transform.position + new Vector3(0, DEFAULT_OFFSET + (colliderTop - transform.position.y), 0);
        Quaternion rotation = (prefabPosition != null) ? prefabPosition.rotation : transform.rotation;

        GameObject go = Instantiate(PrefabController.instance.interactBaseUIPrefab, position, rotation);

        if (prefabPosition != null) {
            float x = go.transform.localScale.x * (1f / prefabPosition.localScale.x);
            float y = go.transform.localScale.y * (1f / prefabPosition.localScale.y);
            float z = go.transform.localScale.z * (1f / prefabPosition.localScale.z);

            go.transform.localScale = new Vector3(x, y, z);

            go.transform.parent = prefabPosition;
        }


        _animator = go.GetComponentInChildren<Animator>();
        _text = go.GetComponentInChildren<Text>();
        DisableUI();
        DefocusUI();
    }

    public void EnableUI() {
        _animator.SetBool("Hide", false);
    }

    public void FocusUI() {
        _animator.SetBool("Interacting", true);
    }

    public void DefocusUI() {
        _animator.SetBool("Interacting", false);
    }

    public void DisableUI() {
        _animator.SetBool("Hide", true);
    }

    public void SetString (string action, string obj) {
        _text.text = action + " " + obj;
    }
}
