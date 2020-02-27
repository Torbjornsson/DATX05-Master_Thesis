using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attachable : MonoBehaviour
{
    public Collider myGrabCollider;
    public BoxCollider mySolidCollider;
    public Rigidbody rb;

    private GameObject originalParent;
    private GameObject overlappingTarget = null;

    private GameObject attachedTo = null;

    private AttachableTarget target;

    // Start is called before the first frame update
    void Start()
    {
        if (!myGrabCollider) Debug.LogError("Couldn't find my grab collider!");
        if (!mySolidCollider) Debug.LogError("Couldn't find my solid collider!");

        originalParent = transform.parent.gameObject;
        if (!originalParent) Debug.LogError("Couldn't find original parent!");
    }

    void OnTriggerEnter(Collider other) {
        if (other.tag.Equals("TargetZone")) {
            overlappingTarget = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.tag.Equals("TargetZone")) {
            overlappingTarget = null;
        }
    }

    public void CatchGrabStartEvent(OVRGrabbable grabbable) {
        if (attachedTo != null) {
            DetachFromObject();
        }
    }
    
    public void CatchGrabEndEvent(OVRGrabbable grabbable) {
        if (overlappingTarget && attachedTo == null) {
            AttachToObject(overlappingTarget);
        }
    }

    public void AttachToObject(GameObject go) {
        target = go.GetComponentInParent<AttachableTarget>();
        if (target.IsOccupied()) return;

        target.AttachObject(this);

        transform.position = go.transform.position;
        transform.rotation = go.transform.rotation;

        myGrabCollider.enabled = false;
        mySolidCollider.enabled = false;
        attachedTo = go;
    }

    public void DetachFromObject()
    {
        target.DetachObject();

        myGrabCollider.enabled = true;
        mySolidCollider.enabled = true;
        attachedTo = null;
    }
}
