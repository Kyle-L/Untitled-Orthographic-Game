using System.Collections.Generic;
using UnityEngine;

/*
A helper component that enables blending from Mecanim animation to ragdolling and back. 

To use, do the following:

Add "GetUpFromBelly" and "GetUpFromBack" bool inputs to the Animator controller
and corresponding transitions from any state to the get up animations. When the ragdoll mode
is turned on, Mecanim stops where it was and it needs to transition to the get up state immediately
when it is resumed. Therefore, make sure that the blend times of the transitions to the get up animations are set to zero.

TODO:

Make matching the ragdolled and animated root rotation and position more elegant. Now the transition works only if the ragdoll has stopped, as
the blending code will first wait for mecanim to start playing the get up animation to get the animated hip position and rotation. 
Unfortunately Mecanim doesn't (presently) allow one to force an immediate transition in the same frame. 
Perhaps there could be an editor script that precomputes the needed information.

*/

[RequireComponent(typeof(Animator))]
public class RagdollHelper : MonoBehaviour {
    //public property that can be set to toggle between ragdolled and animated character
    public bool ragdolled {
        get {
            return state != RagdollState.animated;
        }
        set {
            if (value == true) {
                if (state == RagdollState.animated) {
                    //Transition from animated to ragdolled
                    SetKinematic(false); //allow the ragdoll RigidBodies to react to the environment
                    _animator.enabled = false; //disable animation
                    state = RagdollState.ragdolled;
                }
            } else {
                if (state == RagdollState.ragdolled) {
                    //Transition from ragdolled to animated through the blendToAnim state
                    SetKinematic(true); //disable gravity etc.
                    ragdollingEndTime = Time.time; //store the state change time
                    _animator.enabled = true; //enable animation
                    state = RagdollState.blendToAnim;

                    //Store the ragdolled position for blending
                    foreach (BodyPart b in bodyParts) {
                        b.storedRotation = b.transform.rotation;
                        b.storedPosition = b.transform.position;
                    }

                    //Remember some key positions
                    ragdolledFeetPosition = 0.5f * (_animator.GetBoneTransform(HumanBodyBones.LeftToes).position + _animator.GetBoneTransform(HumanBodyBones.RightToes).position);
                    ragdolledHeadPosition = _animator.GetBoneTransform(HumanBodyBones.Head).position;
                    ragdolledHipPosition = _animator.GetBoneTransform(HumanBodyBones.Hips).position;

                    //Initiate the get up animation
                    if (_animator.GetBoneTransform(HumanBodyBones.Hips).forward.y > 0) //hip hips forward vector pointing upwards, initiate the get up from back animation
                    {
                        _animator.SetTrigger("GetUpFromBack");
                    } else {
                        _animator.SetTrigger("GetUpFromBelly");
                    }
                } //if (state==RagdollState.ragdolled)
            }   //if value==false	
        } //set
    }

    //Possible states of the ragdoll
    enum RagdollState {
        animated,    //Mecanim is fully in control
        ragdolled,   //Mecanim turned off, physics controls the ragdoll
        blendToAnim  //Mecanim in control, but LateUpdate() is used to partially blend in the last ragdolled pose
    }

    //The current state
    RagdollState state = RagdollState.animated;

    //How long do we blend when transitioning from ragdolled to animated
    public float ragdollToMecanimBlendTime = 0.5f;
    float mecanimToGetUpTransitionTime = 0.05f;

    //A helper variable to store the time when we transitioned from ragdolled to blendToAnim state
    float ragdollingEndTime = -100;

    //Declare a class that will hold useful information for each body part
    public class BodyPart {
        public Transform transform;
        public Vector3 storedPosition;
        public Quaternion storedRotation;
    }
    //Additional vectores for storing the pose the ragdoll ended up in.
    private Vector3 ragdolledHipPosition, ragdolledHeadPosition, ragdolledFeetPosition;

    //Declare a list of body parts, initialized in Start()
    private List<BodyPart> bodyParts = new List<BodyPart>();

    //Declare an Animator member variable, initialized in Start to point to this gameobject's Animator component.
    private Animator _animator;

    // Declare a Transform variables that points to the root of the character.
    [Tooltip("The root transform of the character. Typically has the animator attached.")]
    public Transform root;

    //A helper function to set the isKinematc property of all RigidBodies in the children of the 
    //game object that this script is attached to
    private void SetKinematic(bool newValue) {
        //Get an array of components that are of type Rigidbody
        Component[] components = GetComponentsInChildren(typeof(Rigidbody));

        //For each of the components in the array, treat the component as a Rigidbody and set its isKinematic property
        foreach (Component c in components) {
            (c as Rigidbody).isKinematic = newValue;
        }
    }

    // Initialization, first frame of game
    private void Start() {
        //Set all RigidBodies to kinematic so that they can be controlled with Mecanim
        //and there will be no glitches when transitioning to a ragdoll
        SetKinematic(true);

        //Find all the transforms in the character, assuming that this script is attached to the root
        Component[] components = GetComponentsInChildren(typeof(Transform));

        //For each of the transforms, create a BodyPart instance and store the transform 
        foreach (Component c in components) {
            BodyPart bodyPart = new BodyPart {
                transform = c as Transform
            };
            bodyParts.Add(bodyPart);
        }

        _animator = GetComponent<Animator>();
    }

    private void LateUpdate() {
        //Blending from ragdoll back to animated
        if (state == RagdollState.blendToAnim) {
            if (Time.time <= ragdollingEndTime + mecanimToGetUpTransitionTime) {
                //If we are waiting for Mecanim to start playing the get up animations, update the root of the mecanim
                //character to the best match with the ragdoll
                Vector3 animatedToRagdolled = ragdolledHipPosition - _animator.GetBoneTransform(HumanBodyBones.Hips).position;
                Vector3 newRootPosition = root.position + animatedToRagdolled;

                //Now cast a ray from the computed position downwards and find the highest hit that does not belong to the character 
                RaycastHit[] hits = Physics.RaycastAll(new Ray(newRootPosition, Vector3.down));
                newRootPosition.y = 0;
                foreach (RaycastHit hit in hits) {
                    if (!hit.transform.IsChildOf(transform)) {
                        newRootPosition.y = Mathf.Max(newRootPosition.y, hit.point.y);
                    }
                }
                root.position = newRootPosition;

                //Get body orientation in ground plane for both the ragdolled pose and the animated get up pose
                Vector3 ragdolledDirection = ragdolledHeadPosition - ragdolledFeetPosition;
                ragdolledDirection.y = 0;

                Vector3 meanFeetPosition = 0.5f * (_animator.GetBoneTransform(HumanBodyBones.LeftFoot).position + _animator.GetBoneTransform(HumanBodyBones.RightFoot).position);
                Vector3 animatedDirection = _animator.GetBoneTransform(HumanBodyBones.Head).position - meanFeetPosition;
                animatedDirection.y = 0;

                //Try to match the rotations. Note that we can only rotate around Y axis, as the animated characted must stay upright,
                //hence setting the y components of the vectors to zero. 
                transform.rotation *= Quaternion.FromToRotation(animatedDirection.normalized, ragdolledDirection.normalized);
            }
            //compute the ragdoll blend amount in the range 0...1
            float ragdollBlendAmount = 1.0f - (Time.time - ragdollingEndTime - mecanimToGetUpTransitionTime) / ragdollToMecanimBlendTime;
            ragdollBlendAmount = Mathf.Clamp01(ragdollBlendAmount);

            //In LateUpdate(), Mecanim has already updated the body pose according to the animations. 
            //To enable smooth transitioning from a ragdoll to animation, we lerp the position of the hips 
            //and slerp all the rotations towards the ones stored when ending the ragdolling
            foreach (BodyPart b in bodyParts) {
                if (b.transform != transform) { //this if is to prevent us from modifying the root of the character, only the actual body parts
                                                //position is only interpolated for the hips
                    if (b.transform == _animator.GetBoneTransform(HumanBodyBones.Hips)) {
                        b.transform.position = Vector3.Lerp(b.transform.position, b.storedPosition, ragdollBlendAmount);
                    }
                    //rotation is interpolated for all body parts
                    b.transform.rotation = Quaternion.Slerp(b.transform.rotation, b.storedRotation, ragdollBlendAmount);
                }
            }

            //if the ragdoll blend amount has decreased to zero, move to animated state
            if (ragdollBlendAmount == 0) {
                state = RagdollState.animated;
                return;
            }
        }
    }

}
