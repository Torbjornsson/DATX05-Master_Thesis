using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubiksFaceFinder : MonoBehaviour
{
    public OVRGrabber myHand;
    public OVRGrabber otherHand;
    public Collider myCollider;
    [Space]
    [Range (0, 90)] public float faceAngleRange = 45;
    public float distanceMultiplier = 10;
    [Range (0,1)] public float previousMultiplier = 0.8f;

    private RotateRubiks rubiksScript;
    private List<Collider> rubiksFaceColliders;
    private Collider closest = null;

    // Start is called before the first frame update
    void Start()
    {
        if (!myHand) Debug.LogError("RubiksFaceFinder: OVR Grabber hand was not found!");
        if (!myCollider) Debug.LogError("RubiksFaceFinder: My Collider was not found!");

        var grabbables = GameObject.FindGameObjectsWithTag("PuzzleCube");
        var rubiksPresent = false;
        foreach (GameObject go in grabbables) {
            rubiksScript = go.GetComponent<RotateRubiks>();
            if (rubiksScript != null) {
                rubiksPresent = true;
                break;
            }
        }
        if (!rubiksPresent) gameObject.SetActive(false);

        rubiksFaceColliders = new List<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        // Disabling collider, making this script dormant, until hand grabs something
        if (otherHand.grabbedObject == null) {
            if (myCollider.enabled) myCollider.enabled = false;
            return;
        } else if (!myCollider.enabled) {
            myCollider.enabled = true;
        }
        // Debug.Log("Rubiks Face Finder update() ?");

        // Finding the closest small-cube
        float bestDistance = -1;
        var handDir = transform.TransformDirection(Vector3.forward);
        Collider previousClosest = closest;
        closest = null;
        if (previousClosest != null && !rubiksFaceColliders.Contains(previousClosest))
            rubiksFaceColliders.Add(previousClosest);
        if (rubiksFaceColliders.Count > 0)
            Debug.Log("-- Going through colliders... (size: "+rubiksFaceColliders.Count+")");

        foreach (Collider face in rubiksFaceColliders) {
            // Facing cut-off
            var faceOut = face.transform.TransformDirection(Vector3.back);
            var angle = Vector3.Angle(handDir, faceOut);
            // Debug.Log("Angle between hand and face ("+face.name+"): "+angle);
            if (angle < 180 - faceAngleRange) continue;

            // Distance cut-off
            Vector3 direction = handDir;
            Vector3 startingPoint = transform.position;
            
            Ray ray = new Ray(startingPoint, direction);
            float distanceToRay = Vector3.Cross(ray.direction, face.transform.position - ray.origin).magnitude;
            Debug.Log("Distance to ray: "+distanceToRay);

            var distance = ((face.transform.position - transform.position).magnitude + distanceToRay * distanceMultiplier) * distanceMultiplier;
            if (face.Equals(previousClosest)) distance *= previousMultiplier;
            Debug.Log("Candidate face: "+face.name+", angle: "+angle+", distance: "+distance);
            if (bestDistance > 0 && bestDistance <= distance) continue;

            bestDistance = distance;
            closest = face;
        }
        var hand = closest ? myHand.tag : null;

        if (closest) Debug.Log("-- Chosen face: "+closest.name+", with hand: "+hand);
        rubiksScript.SetSelectedCollider(closest, hand);

        // rubiksFaceColliders.Clear();
    }

    // private void OnTriggerStay(Collider other)
    // {
    //     if (!other.isTrigger || rubiksFaceColliders.Contains(other)) return;

    //     Debug.Log("Found collider! "+other.name+", tag: "+other.tag);
    //     // Debug.DrawRay(transform.position, other.transform.position, Color.red, 1);
    //     if (other.tag.Equals("RubiksBlocker")) return;
    //     Debug.DrawLine(transform.position, other.transform.position, Color.red);
    //     // if (!rubiksFaceColliders.Contains(other))
    //     rubiksFaceColliders.Add(other);
    // }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger || rubiksFaceColliders.Contains(other)) return;

        // Debug.Log("Found collider! "+other.name+", tag: "+other.tag);
        // Debug.DrawRay(transform.position, other.transform.position, Color.red, 1);
        if (other.tag.Equals("RubiksBlocker")) return;
        Debug.DrawLine(transform.position, other.transform.position, Color.red);
        rubiksFaceColliders.Add(other);
    }

    private void OnTriggerExit(Collider other)
    {
        // Debug.Log("Collider exits! "+other.name+", tag: "+other.tag);
        if (rubiksFaceColliders.Contains(other)) {
            rubiksFaceColliders.Remove(other);
            // Debug.Log("... Collider removed! "+other.name+", tag: "+other.tag);
        }
    }
}
