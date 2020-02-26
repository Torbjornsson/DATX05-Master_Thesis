using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachableTarget : MonoBehaviour
{
    public BoxCollider myMainCollider;

    private Vector3 unAttachedColliderCenter;
    private Vector3 unAttachedColliderSize;
    public Vector3 attachedColliderCenter;
    public Vector3 attachedColliderSize;

    // Start is called before the first frame update
    void Start()
    {
        unAttachedColliderSize = myMainCollider.size;
        unAttachedColliderCenter = myMainCollider.center;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AttachObject() {
        myMainCollider.size = attachedColliderSize;
        myMainCollider.center = attachedColliderCenter;
    }

    public void DetachObject() {
        myMainCollider.size = unAttachedColliderSize;
        myMainCollider.center = unAttachedColliderCenter;
    }
}
