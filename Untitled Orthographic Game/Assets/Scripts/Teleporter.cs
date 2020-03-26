using UnityEngine;

public class Teleporter : MonoBehaviour {

    private bool canUse = true;

    [Header("General")]
    private int currentPosition = 0;
    private int nextPosition = 1;
    public GameObject[] positions;

    [Header("Particles")]
    public ParticleSystem _particleSystem;

    [Header("Audio")]
    public AudioClip audioClip;
    public AudioSource _audioSource;

    private void Update() {
        /* On jump, the user teleports if they can use. */
        if (Input.GetButtonDown("Jump") && canUse) {

            // Calculates the difference and then applies that to the next position.
            Vector3 diff = positions[currentPosition].transform.InverseTransformPoint(PlayerControllerMain.instance.transform.position);
            Vector3 telePos = positions[nextPosition].transform.position + diff;

            // Puts the user on the closest navmesh point.
            PlayerControllerMain.instance.MovementController.SetPosition(telePos);

            // Plays the particles and audio if they exist.
            //_particleSystem?.Play();
            //_audioSource?.PlayOneShot(audioClip);

            // Increments to the next position and bounds.
            currentPosition++;
            nextPosition++;

            currentPosition %= positions.Length;
            nextPosition %= positions.Length;
            print("jump");
        }
    }

    public void SetControl(bool control) {
        canUse = control;
    }
}
