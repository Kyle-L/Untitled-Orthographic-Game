using System.Linq;
using UnityEngine;

public class CharacterSerializer : MonoBehaviour {
    public static CharacterSerializer instance;

    public Controller[] AllCharacters { get; private set; }
    public Transform[] Lookable { get; private set; }

    void Awake() {
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

        UpdateCharacters();
        UpdateLookable();
    }

    void UpdateCharacters() {
        AllCharacters = FindObjectsOfType(typeof(Controller)) as Controller[];
    }

    void UpdateLookable() {
        var list = GameObject.FindGameObjectsWithTag("Lookable").Select(go => go.transform);
        list = list.Union(GameObject.FindGameObjectsWithTag("Viewable").Select(go => go.transform));
        list = list.Union(GameObject.FindGameObjectsWithTag("Commentable").Select(go => go.transform));

        Lookable = list.ToArray();
    }
}
