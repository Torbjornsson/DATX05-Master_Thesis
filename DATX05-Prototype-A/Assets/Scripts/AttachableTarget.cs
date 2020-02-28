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
            attachedObject.transform.position = attachTarget.transform.position;
            attachedObject.transform.rotation = attachTarget.transform.rotation;
        }
    }

    public void AttachObject(Attachable attachable) {
        isOccupied = true;

        myGrabCollider.size = attachedColliderSize;
        myGrabCollider.center = attachedColliderCenter;
        
        attachedObject = attachable;

        myExtraCollider.gameObject.SetActive(true);

        if (attachable.correctSolution) {
            GameMaster.instance.goalCriteriaSatisfied = true;
        }
    }

    public void DetachObject() {
        isOccupied = false;

        myGrabCollider.size = unAttachedColliderSize;
        myGrabCollider.center = unAttachedColliderCenter;

        attachedObject = null;

        myExtraCollider.gameObject.SetActive(false);

        GameMaster.instance.goalCriteriaSatisfied = false;
    }

    public bool IsOccupied() {
        return isOccupied;
    }
}
