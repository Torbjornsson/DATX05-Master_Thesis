using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attachable : MonoBehaviour
{
    public Collider myCollider;
    public Rigidbody rb;

    private GameObject originalParent;
    private GameObject overlappingTarget = null;

    private GameObject attachedTo = null;

    private AttachableTarget target;

    // Start is called before the first frame update
    void Start()
    {
        if (!myCollider) Debug.LogError("Couldn't find my collider!");

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

    // public void FixedUpdate() {
    //     if (attachedTo != null) {
    //         transform.position = attachedTo.transform.position;
    //         transform.rotation = attachedTo.transform.rotation;
    //     }
    // }

    public void AttachToObject(GameObject go) {
        // Debug.Log("Attach to object! - "+go.name);

        target = go.GetComponentInParent<AttachableTarget>();
        if (target.IsOccupied()) return;

        target.AttachObject(this);

        transform.position = go.transform.position;
        transform.rotation = go.transform.rotation;

        myCollider.enabled = false;
        rb.isKinematic = true;
        // Destroy(rb);
        
        attachedTo = go;
        // gameObject.transform.SetParent(go.transform);
    }

    public void DetachFromObject()
    {
        target.DetachObject();

        myCollider.enabled = true;

        // rb = gameObject.AddComponent<Rigidbody>();
        // rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        
        attachedTo = null;
        // gameObject.transform.SetParent(originalParent.transform);
    }
}
