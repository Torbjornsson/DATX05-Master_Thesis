﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attachable : MonoBehaviour
{
    public Collider myCollider;
    // public BoxCollider myAttachZone;
    // public BoxCollider targetAttachZone;
    public Rigidbody rb;

    private GameObject originalParent;
    private GameObject overlappingTarget = null;

    private GameObject attachedTo = null;
    private Vector3 attachedOffset;

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

    public void AttachToObject(GameObject go) {
        Debug.Log("Attach to object! - "+go.name);

        transform.position = go.transform.position;
        transform.rotation = go.transform.rotation;

        // rb.isKinematic = true;
        myCollider.enabled = false;
            
        attachedTo = go;
        gameObject.transform.SetParent(go.transform);
    }

    public void DetachFromObject()
    {
        gameObject.transform.SetParent(originalParent.transform);
        attachedTo = null;
        attachedOffset = Vector3.zero;
        myCollider.enabled = true;
    }
}
