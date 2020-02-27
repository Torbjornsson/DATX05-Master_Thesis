using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachableTarget : MonoBehaviour
{
    public BoxCollider myMainCollider;
    public GameObject attachTarget;
    public Vector3 attachedColliderCenter;
    public Vector3 attachedColliderSize;


    private Vector3 unAttachedColliderCenter;
    private Vector3 unAttachedColliderSize;
    private bool isOccupied;
    private Attachable attachedObject;
    private GameObject attachedObjectDummy;
    // private Rigidbody attachedObjectRB;
    // private Rigidbody targetRB;
    // private Rigidbody myRB;

    // Start is called before the first frame update
    void Start()
    {
        isOccupied = false;
        unAttachedColliderSize = myMainCollider.size;
        unAttachedColliderCenter = myMainCollider.center;
        // targetRB = attachTarget.GetComponent<Rigidbody>();
        // myRB = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // if (attachedObjectRB != null) {
        //     // attachedObjectRB.position = myRB.position + attachTarget.transform.localPosition;
        //     attachedObjectRB.position = attachTarget.transform.position;
        //     attachedObjectRB.rotation = attachTarget.transform.rotation;
        // }
        if (attachedObject != null) {
            // attachedObjectRB.position = myRB.position + attachTarget.transform.localPosition;
            attachedObject.transform.position = attachTarget.transform.position;
            attachedObject.transform.rotation = attachTarget.transform.rotation;
        }
    }

    // public void FixedUpdate() {
    //     if (attachedObjectRB != null) {
    //         attachedObjectRB.position = myRB.position + attachTarget.transform.localPosition;
    //         attachedObjectRB.rotation = attachTarget.transform.rotation;
    //     }
    // }

    public void AttachObject(Attachable attachable) {
        isOccupied = true;
        myMainCollider.size = attachedColliderSize;
        myMainCollider.center = attachedColliderCenter;
        attachedObject = attachable;
        // attachedObjectRB = attachable.gameObject.GetComponent<Rigidbody>();
    }

    public void DetachObject() {
        isOccupied = false;
        myMainCollider.size = unAttachedColliderSize;
        myMainCollider.center = unAttachedColliderCenter;
        attachedObject = null;
        // attachedObjectRB = null;
    }

    public bool IsOccupied() {
        return isOccupied;
    }
}
