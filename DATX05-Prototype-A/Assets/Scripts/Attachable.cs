﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attachable : MonoBehaviour
{
    public BoxCollider myAttachZone;
    public BoxCollider targetAttachZone;
    public GameObject originalParent;
    public Rigidbody rb;

    private GameObject overlappingTarget = null;

    private GameObject attachedTo = null;
    private Vector3 attachedOffset;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // overlappingTarget = false;
        // Debug.Log("Resetting overlapping target...");
    }

    void FixedUpdate() {
        // if (attachedTo != null) {
        //     transform.position = attachedTo.transform.position + attachedOffset;
        //     transform.rotation = attachedTo.transform.rotation;
        //     Debug.Log("Attached to: "+attachedTo+", position: "+attachedTo.transform.position + attachedOffset);
        // }
    }

    void OnTriggerEnter(Collider other) {
        if (other.tag.Equals("TargetZone")) {
            overlappingTarget = other.gameObject;
            // Debug.Log("Overlapping target!");
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.tag.Equals("TargetZone")) {
            overlappingTarget = null;
            // Debug.Log("Overlapping target!");
        }
    }
    
    // private Collider[] results;
    // public Collider[] result = new Collider[1];
    public void CatchGrabEndEvent(OVRGrabbable grabbable) {
        // Debug.Log("Dropping grabbable! (overlapping? "+overlappingTarget+")");

        // Collider[] results = new Collider[1];
        // var pos = myAttachZone.transform.position + myAttachZone.gameObject.transform.position;
        // var size = myAttachZone.bounds.size / 2f;
        // Debug.Log("Pos: "+pos+", size: "+size);
        // var hits = Physics.OverlapBoxNonAlloc(pos, size, results, myAttachZone.transform.rotation);

        // if (hits > 0 && results[0].tag.Equals("TargetZone")) {
        if (overlappingTarget && attachedTo == null) {
            AttachToObject(overlappingTarget);
        }
    }

    public void AttachToObject(GameObject go) {
        Debug.Log("Attach to object! - "+go.name);

        transform.position = go.transform.position;
        transform.rotation = go.transform.rotation;

        rb.isKinematic = true;
        attachedTo = go;
        gameObject.transform.SetParent(go.transform);
        // attachedOffset = transform.position - go.transform.position;
    }

    public void CatchGrabStartEvent(OVRGrabbable grabbable) {
        if (attachedTo != null) {
            gameObject.transform.SetParent(originalParent.transform);
            attachedTo = null;
            attachedOffset = Vector3.zero;
        }
    }
}
