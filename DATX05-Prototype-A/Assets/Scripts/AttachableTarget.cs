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

    // Start is called before the first frame update
    void Start()
    {
        isOccupied = false;
        unAttachedColliderSize = myMainCollider.size;
        unAttachedColliderCenter = myMainCollider.center;
    }

    // Update is called once per frame
    void Update()
    {
        if (attachedObject != null) {
            attachedObject.transform.position = attachTarget.transform.position;
            attachedObject.transform.rotation = attachTarget.transform.rotation;
        }
    }

    public void AttachObject(Attachable attachable) {
        isOccupied = true;
        myMainCollider.size = attachedColliderSize;
        myMainCollider.center = attachedColliderCenter;
        attachedObject = attachable;
    }

    public void DetachObject() {
        isOccupied = false;
        myMainCollider.size = unAttachedColliderSize;
        myMainCollider.center = unAttachedColliderCenter;
        attachedObject = null;
    }

    public bool IsOccupied() {
        return isOccupied;
    }
}
