using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachableTarget : MonoBehaviour
{
    public BoxCollider myGrabCollider;
    public BoxCollider myExtraCollider;
    public GameObject attachTarget;
    public Attachable attachedObject {get; private set;}
    public Vector3 attachedColliderCenter;
    public Vector3 attachedColliderSize;

    [HideInInspector] public bool allowAttaching = true;


    private Vector3 unAttachedColliderCenter;
    private Vector3 unAttachedColliderSize;
    private Rigidbody attachedObjectRB;
    private bool isOccupied;

    // Start is called before the first frame update
    void Start()
    {
        if (!myGrabCollider)
            Debug.LogError(gameObject.name+": Grab collider was not found!");
        if (!myExtraCollider)
            Debug.LogError(gameObject.name+": Extra collider was not found!");
        if (!attachTarget)
            Debug.LogError(gameObject.name+": Attach target was not found!");

        isOccupied = false;
        unAttachedColliderSize = myGrabCollider.size;
        unAttachedColliderCenter = myGrabCollider.center;
    }

    // Update is called once per frame
    void Update()
    {
        if (attachedObject != null) {
            Debug.DrawLine(attachedObject.transform.position, attachTarget.transform.position, Color.red);
            Debug.Log("Previous position: "+attachedObject.transform.position+", new position: "+attachTarget.transform.position);
            attachedObject.transform.position = attachTarget.transform.position;
            attachedObject.transform.rotation = attachTarget.transform.rotation;
        }
    }

    public void AttachObject(Attachable attachable) {
        isOccupied = true;

        myGrabCollider.size = attachedColliderSize;
        myGrabCollider.center = attachedColliderCenter;
        
        attachedObject = attachable;
        attachedObjectRB = attachable.GetComponent<Rigidbody>();
        attachedObjectRB.useGravity = false;

        myExtraCollider.gameObject.SetActive(true);

        if (attachable.correctSolution) {
            GameMaster.instance.goalCriteriaSatisfied = true;
        }
    }

    public void DetachObject() {
        isOccupied = false;

        myGrabCollider.size = unAttachedColliderSize;
        myGrabCollider.center = unAttachedColliderCenter;

        attachedObjectRB.useGravity = true;
        attachedObjectRB = null;
        attachedObject = null;

        myExtraCollider.gameObject.SetActive(false);

        GameMaster.instance.goalCriteriaSatisfied = false;
    }

    public bool IsOccupied() {
        return isOccupied;
    }
}
