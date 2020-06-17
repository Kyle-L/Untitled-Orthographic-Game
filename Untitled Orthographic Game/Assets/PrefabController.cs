using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This contains prefabs that are frequently generated in-script.
/// </summary>
public class PrefabController : MonoBehaviour
{
    public static PrefabController instance;

    public GameObject interactBaseUIPrefab;

    private void Awake() {
        #region Enforces Singleton Pattern.
        //Check if instance already exists
        if (instance == null) {
            //if not, set instance to this	
            instance = this;
        }
        //If instance already exists and it's not this:
        else if (instance != this) {
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a MyNetworkManager.
            Destroy(gameObject);
        }
        #endregion
    }
}
