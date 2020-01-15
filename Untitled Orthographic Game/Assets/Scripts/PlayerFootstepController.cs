using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource[]))]
public class PlayerFootstepController : MonoBehaviour {

    private const int MAX_NUM_OF_TAGS = 100;

    [SerializeField]
    private float footStepDistance = 5.0f;
    [SerializeField]
    private float minVolumeScale = 0.75f;
    [SerializeField]
    private float maxVolumeScale = 1.25f;
    [SerializeField]
    private float minPitchScale = 0.75f;
    [SerializeField]
    private float maxPitchScale = 1.25f;

    [Range(0, MAX_NUM_OF_TAGS)]
    [Tooltip("The maximum number of possible tags/audioclips.")]
    [SerializeField]
    private int materialNums = 5;

    [Tooltip("The tags that corrispond to the audioclips. \nThe tag must be in the same index as its corrisponding AudioClip." +
        "\nAlso ensure that the way the tag is spelled out in the tags array is exactly the same as the tags the menu.")]
    [SerializeField]
    private string[] tags = new string[1];

    [Tooltip("The AudioClip that corrispond to the tags. \nThe AudioClip must be in the same index as its corrisponding tag." +
        "\nAlso ensure that the way the tag is spelled out in the tags array is exactly the same as the tags the menu.")]
    [SerializeField]
    private AudioClip[] audioClips = new AudioClip[1];

    private AudioSource[] sources;
    private int audioPoolIndex = 0;

    private Dictionary<string, List<AudioClip>> audioDictionary;

    private void Awake() {
        audioDictionary = new Dictionary<string, List<AudioClip>>();
        sources = GetComponents<AudioSource>();

        /*Converts the information in the two arrays into a Dictionary of List<AudioClip> with their key matching their corresponding
        tag*/
        for (int i = 0; i < audioClips.Length; i++) {
            //If the key doesn't exist create one a new entry.
            if (!audioDictionary.ContainsKey(tags[i])) {
                audioDictionary.Add(tags[i], new List<AudioClip>());
            }
            //Add the AudioClip to the List<AudioClip> of the corresponding tag.
            audioDictionary[tags[i]].Add(audioClips[i]);
        }
    }

    /// <summary>
    /// This function is called when the script is loaded or a value is changed in the inspector (Called in the editor only).
    /// </summary>
    private void OnValidate() {
        //Ensures that the tags and audioClips arrays are the same size in the editor.
        if (tags.Length != materialNums) {
            Array.Resize(ref tags, materialNums);
        }

        if (audioClips.Length != materialNums) {
            Array.Resize(ref audioClips, materialNums);
        }
    }

    /// <summary>
    /// The event the the left foot plays when it steps on the ground.
    /// </summary>
    public void PlayLeftStep() {
        PlayFootStep();
    }

    /// <summary>
    /// The event the the right foot plays when it steps on the ground.
    /// </summary>
    public void PlayRightStep() {
        PlayFootStep();
    }

    /// <summary>
    /// Plays the footstep sound.
    /// </summary>
    public void PlayFootStep() {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, -Vector3.up, out hit, footStepDistance)) {
            sources[audioPoolIndex].pitch = UnityEngine.Random.Range(minPitchScale, maxPitchScale);
            sources[audioPoolIndex].volume = UnityEngine.Random.Range(minVolumeScale, maxVolumeScale);
            try {
                sources[audioPoolIndex].clip = audioDictionary[hit.collider.tag][UnityEngine.Random.Range(0, audioDictionary[hit.collider.tag].Count - 1)];
            } catch (Exception) {
                Debug.Log("That tag doesn't exist in the dictionary, please ensure that you've included the tag in both the script array and Unity's Tags and Layers menu.");
            }
            sources[audioPoolIndex].Play();

            audioPoolIndex++;
            audioPoolIndex %= sources.Length;
        }
    }

}
