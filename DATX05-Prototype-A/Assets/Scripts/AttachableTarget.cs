using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachableTarget : MonoBehaviour
{
    public BoxCollider myGrabCollider;
    public GameObject attachTarget;
    public Attachable attachedObject {get; private set;}
    public Vector3 attachedColliderCenter;
    public Vector3 attachedColliderSize;
    public bool canOnlyAttachWhenGrabbed = true;
    [Space]
    public AudioClip[] attachSounds;
    public AudioClip[] detachSounds;
    public AudioSource soundSource;
    [Range(0, 1)] public float attachBasePitch;
    [Range(0, 1)] public float detachBasePitch;
    [Range(0, 1)] public float pitchSpan = 0.1f;

    [HideInInspector] public bool allowAttaching = true;


    private Vector3 unAttachedColliderCenter;
    private Vector3 unAttachedColliderSize;
    private Rigidbody attachedObjectRB;
    private OVRGrabbable_EventExtension grabbable;
    private bool isOccupied;

    // Start is called before the first frame update
    void Start()
    {
        grabbable = GetComponent<OVRGrabbable_EventExtension>();

        if (!myGrabCollider)
            Debug.LogError(gameObject.name+": Grab collider was not found!");
        // if (!myExtraCollider)
        //     Debug.LogError(gameObject.name+": Extra collider was not found!");
        if (!attachTarget)
            Debug.LogError(gameObject.name+": Attach target was not found!");
        if (!grabbable)
            Debug.LogError(gameObject.name+": OVRGrabbable_EventExtension was not found!");

        isOccupied = false;
        unAttachedColliderSize = myGrabCollider.size;
        unAttachedColliderCenter = myGrabCollider.center;

        if (attachSounds.Length <= 0 || detachSounds.Length <= 0)
            Debug.LogError(gameObject.name+": No attach or detach sounds were found!");
        if (!soundSource)
            Debug.LogError(gameObject.name+": Audio Source was not found!");
    }

    // Update is called once per frame
    void Update()
    {
        if (attachedObject != null) {
            // Debug.DrawLine(attachedObject.transform.position, attachTarget.transform.position, Color.red);
            // Debug.Log("Previous position: "+attachedObject.transform.position+", new position: "+attachTarget.transform.position);
            attachedObject.transform.position = attachTarget.transform.position;
            attachedObject.transform.rotation = attachTarget.transform.rotation;
        }
    }

    public void AttachObject(Attachable attachable) {
        isOccupied = true;

        myGrabCollider.size = attachedColliderSize;
        myGrabCollider.center = attachedColliderCenter;

        attachTarget.SetActive(false);
        
        attachedObject = attachable;
        attachedObjectRB = attachable.GetComponent<Rigidbody>();
        attachedObjectRB.useGravity = false;

        GameMaster.instance.goalCriteriaSatisfied = IsCorrectSolution(attachable);

        PlayAttachSound();
    }

    public void DetachObject() {
        isOccupied = false;

        myGrabCollider.size = unAttachedColliderSize;
        myGrabCollider.center = unAttachedColliderCenter;

        attachedObjectRB.useGravity = true;
        attachedObjectRB = null;
        attachedObject = null;

        attachTarget.SetActive(true);

        GameMaster.instance.goalCriteriaSatisfied = false;

        PlayDetachSound();
    }

    public bool IsOccupied() {
        return isOccupied;
    }

    public bool CanBeAttachedTo() {
        return !isOccupied && allowAttaching && (!canOnlyAttachWhenGrabbed || grabbable.isGrabbed);
    }

    public void PlayAttachSound() {
        PlaySound(attachSounds, attachBasePitch);
    }
    public void PlayDetachSound() {
        PlaySound(detachSounds, detachBasePitch);
    }

    public void PlaySound(AudioClip[] sounds, float basePitch) {
        soundSource.clip = sounds[Random.Range(0, sounds.Length)];
        soundSource.pitch = basePitch - pitchSpan/2 + Random.Range(0, pitchSpan);
        soundSource.Play();
    }

    private bool IsCorrectSolution(Attachable attachable)
    {
        bool correct = false;
        if (attachable.correctSolution)
        {
            //Debug.Log("cubes rotation " + transform.localEulerAngles + " tiles rotation " + attachable.transform.localEulerAngles);
            correct = Mathf.Abs(transform.localEulerAngles.y - attachable.transform.localEulerAngles.y) < 10;
        }

        return correct;
    }
}
