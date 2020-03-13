using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attachable : MonoBehaviour
{
    // private static Dictionary<Symbol, Mesh> symbolMeshes = new Dictionary<Symbol, Mesh>();

    public enum Symbol {
        Circle, Square, Triangle
    }

    public Collider myGrabCollider;
    public BoxCollider mySolidCollider;
    // public Rigidbody rb;
    public bool correctSolution = false;
    [Space]
    [Range(0, 8)] public int tileDesign = 0;
    public MeshRenderer meshRenderer;
    public Material[] tileMaterials;
    [Space]
    public Symbol symbol = Symbol.Circle;
    public MeshFilter meshFilter;
    public Mesh tileCircle;
    public Mesh tileSquare;
    public Mesh tileTriangle;

    private GameObject originalParent;
    private GameObject overlappingTarget = null;

    private GameObject attachedTo = null;

    private AttachableTarget target;

    // Start is called before the first frame update
    void Start()
    {
        if (!myGrabCollider)
            Debug.LogError(gameObject.name+": Couldn't find my grab collider!");
        if (!mySolidCollider)
            Debug.LogError(gameObject.name+": Couldn't find my solid collider!");

        originalParent = transform.parent.gameObject;
        if (!originalParent)
            Debug.LogError(gameObject.name+": Couldn't find original parent!");

        // Setting the right symbol on the back
        switch(symbol) {
            case Symbol.Circle:
                meshFilter.mesh = tileCircle;
                break;
            case Symbol.Square:
                meshFilter.mesh = tileSquare;
                break;
            case Symbol.Triangle:
                meshFilter.mesh = tileTriangle;
                break;
        }

        if (tileDesign < 0 || tileDesign >= tileMaterials.Length)
            Debug.LogError(gameObject.name+": Design number not valid!");

        // Setting the right material (design pattern)
        var materials = meshRenderer.materials;
        materials[1] = tileMaterials[tileDesign];
        meshRenderer.materials = materials;
    }

    void OnTriggerStay(Collider other) {
        if (overlappingTarget == null && other.tag.Equals("TargetZone")) {
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
        if (!target.CanBeAttachedTo()) return;

        target.AttachObject(this);

        transform.position = go.transform.position;
        transform.rotation = go.transform.rotation;

        // myGrabCollider.enabled = false;
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
