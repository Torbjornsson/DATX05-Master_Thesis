using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateRubiks_Test : MonoBehaviour
{
    public AudioClip[] shuffleSounds;
    public AudioSource soundSource;
    [Range(0, 1)] public float shufflePitchSpan = 0.05f;
    
    SmallCube[] smallCubes;
    GameObject frontFace, backFace;
    bool isRotationStarted = false;

    private Dictionary<string, GameObject> hands;

    private float shufflePitchBase;

    OVRGrabbable grabbable;
    // Start is called before the first frame update
    void Start()
    {
        if (shuffleSounds.Length <= 0)
            Debug.LogError(gameObject.name+": No shuffle sounds were found!");
        if (!soundSource)
            Debug.LogError(gameObject.name+": Audio Source was not found!");

        grabbable = GetComponent<OVRGrabbable>();
        frontFace = new GameObject();
        backFace = new GameObject();
        SetFace(frontFace);
        SetFace(backFace);
        smallCubes = GetComponentsInChildren<SmallCube>();

        shufflePitchBase = soundSource.pitch;

        hands = new Dictionary<string, GameObject>();
        FindAndSaveHand("LeftHand");
        FindAndSaveHand("RightHand");
    }

    private void FindAndSaveHand(string hand) {
        var obj = GameObject.FindGameObjectWithTag(hand);
        if (!obj)
            Debug.LogError(gameObject.name+": "+hand+" object was not found!");
        hands.Add(hand, obj);
    }

    // Update is called once per frame
    void Update()
    {
        if (!grabbable.isGrabbed) return;
        switch(grabbable.grabbedBy.tag)
        {
            case "RightHand":
                HandAction("LeftHand", OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger));
                break;

            case "LeftHand":
                HandAction("RightHand", OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger));
                break;

            default:
                if(isRotationStarted)
                    StopRotation();
                break;
        }
    }

    private void HandAction(string hand, float input) {
        // Pressing "TURN" (or trigger)
        if (input > 0.7f && !isRotationStarted)
        {
            int direction = 0;
            // Vector3 axis = PickAxis(hand, out direction);
            Vector3 axis = PickAxisAndRotate(hand, out direction);
            if (axis != Vector3.zero && direction != 0 && frontFace.transform.childCount == 4)
                RotateFace(axis, direction);
        }

        // Releasing "TURN" (or trigger)
        else if (input <= 0.5f && isRotationStarted)
            StopRotation();
        
        // Idle (just for showing hints)
        else {
            var collider = GetSmallCubeCollider(hand);
            // if (collider && collider.transform.childCount < 2) {
            if (collider && !collider.tag.Equals("RubiksBlocker")) {
                // Debug.Log("Found small cube collider: "+collider.gameObject.name);
                var handDir = GetHandOrientationComparedToSmallCube(hands[hand], collider.gameObject);

                var script = collider.gameObject.GetComponentInChildren<RubiksBoxScript>();
                if (script) script.ShowHint(true, handDir);
            }
        }
    }

    void StartRotation(Vector3 axis, int side)
    {
        // Debug.Log("Rotation Started");
        isRotationStarted = true;

        foreach (var smallCube in smallCubes)
        {
            int smallCubePos = -1;
            if (axis.x > 0) smallCubePos = smallCube.intPos.x;
            if (axis.y > 0) smallCubePos = smallCube.intPos.y;
            if (axis.z > 0) smallCubePos = smallCube.intPos.z;

            if (smallCubePos != -1)
                smallCube.transform.SetParent(smallCubePos == side ? frontFace.transform : backFace.transform);
        }
        // Debug.Log("Rotates cubes: " + frontFace.transform.childCount);
    }

    void StopRotation()
    {
        Vector3Int prevPos;
        // Quaternion rot = frontFace.transform.localRotation;
        // rot.eulerAngles = rot.eulerAngles/90f;
        // Vector3 euler = new Vector3();
        // euler.x = Mathf.Round(rot.eulerAngles.x);
        // euler.y = Mathf.Round(rot.eulerAngles.y);
        // euler.z = Mathf.Round(rot.eulerAngles.z);
        // euler *= 90f;
        // rot.eulerAngles = euler;

        // frontFace.transform.localRotation = rot;

        foreach(var smallCube in smallCubes)
        {
            smallCube.transform.SetParent(transform);
            prevPos = smallCube.intPos;
            smallCube.UpdatePosition();
            // Debug.Log("from position " + prevPos + " to position " + smallCube.intPos);
        }

        SetFace(frontFace);
        SetFace(backFace);
                
        isRotationStarted = false;
        // Debug.Log("Rotation Stopped");
    }

    private Collider GetSmallCubeCollider(string hand) {
        
        RaycastHit hit;
        GameObject activeHand = hands[hand];

        // int layerMask = 1 << 12;
        int layerMask = 1 << LayerMask.NameToLayer("SmollCube");
        var start = activeHand.transform.position;
        var up = activeHand.transform.TransformDirection(Vector3.up);
        start += up * 0.05f;
        var end = activeHand.transform.TransformDirection(Vector3.forward);

        if (Physics.Raycast(start, end, out hit, 1f, layerMask))
        {
            Debug.DrawRay(start, end, Color.red, 1);

            if (hit.collider) return hit.collider;
        }

        return null;
    }

    private Vector2Int GetHandOrientationComparedToSmallCube(GameObject hand, GameObject smallCube) {
        var handPos = hand.transform.position;
        var handDown = hand.transform.TransformDirection(Vector3.down);
        // Debug.DrawLine(handPos, handPos + handDown);

        var cubePos = smallCube.transform.position;
        var cubeOut = smallCube.transform.TransformDirection(Vector3.back);
        // Debug.DrawLine(cubePos, cubePos + cubeOut);

        var projected = Vector3.ProjectOnPlane(handDown, cubeOut);
        var projectedLocal = smallCube.transform.InverseTransformVector(projected);

        var localDir = Vector2Int.zero;
        if (Mathf.Abs(projectedLocal.x) > Mathf.Abs(projectedLocal.y)) {
            localDir.x = (int) Mathf.Sign(projectedLocal.x);
        } else {
            localDir.y = (int) Mathf.Sign(projectedLocal.y);
        }
        
        // Debug.DrawLine(cubePos, cubePos + projected, Color.blue, 1);
        // Debug.Log(smallCube.name+": projected: "+projected+", local space: "+projectedLocal+", local direction: "+localDir);

        return localDir;
    }

    private Vector3 PickAxisAndRotate(string hand, out int direction) {
        
        direction = 0;
        int side = 0;
        Vector3 axis = Vector3.zero;

        var collider = GetSmallCubeCollider(hand);

        if (!collider || collider.tag.Equals("RubiksBlocker")) return axis;
        var parent = collider.transform.parent;
        
        var handDir = GetHandOrientationComparedToSmallCube(hands[hand], collider.gameObject);

        direction = handDir.x != 0 ? handDir.x : handDir.y;

        var localAxis = handDir.x == 0 ? Vector3.right : Vector3.up;
        axis = parent.localRotation * localAxis;
        axis = new Vector3(Mathf.Round(axis.x), Mathf.Round(axis.y), Mathf.Round(axis.z));

        if (axis.x < 0 || axis.y > 0 || (axis.z < 0 && localAxis.x > 0) || (axis.z > 0 && localAxis.y > 0)) direction *= -1;
        // Debug.Log("Local axis: "+localAxis+", hand dir: "+handDir+", Axis: ("+axis.x+","+axis.y+","+axis.z+"), direction: "+direction);
        
        axis = new Vector3(Mathf.Abs(axis.x), Mathf.Abs(axis.y), Mathf.Abs(axis.z));
        // Debug.Log("FOUND AXIS: ("+axis.x+","+axis.y+","+axis.z+") vs RIGHT: "+Vector3.right);

        // axis = Vector3.right;
        // direction = 1;
        side = 0; // ----------------->>> TODO: Just fix the sides

        if(axis != Vector3.zero)
            StartRotation(axis, side);

        return axis;
    }

    private void RotateAroundAxis() {
        
    }

    private Vector3 PickAxis(string hand, out int direction) {

        direction = 0;
        int side = 0;
        Vector3 axis = Vector3.zero;

        var collider = GetSmallCubeCollider(hand);
        if (!collider) return axis;

        switch(collider.tag){
            case "FDL":
                axis = Vector3.right;
                direction = -1;
                side = 0;
                break;
            case "FDR":
                axis = Vector3.right;
                direction = 1;
                side = 1;
                break;
            case "FLD":
                axis = Vector3.up;
                direction = 1;
                side = 0;
                break;
            case "FLT":
                axis = Vector3.up;
                direction = 1;
                side = 1;
                break;
            case "FTL":
                axis = Vector3.right;
                direction = -1;
                side = 0;
                break;
            case "FTR":
                axis = Vector3.right;
                direction = 1;
                side = 1;
                break;
            case "FRD":
                axis = Vector3.up;
                direction = -1;
                side = 0;
                break;
            case "FRT":
                axis = Vector3.up;
                direction = -1;
                side = 1;
                break;
            case "DLF":
                axis = Vector3.forward;
                direction = -1;
                side = 0;
                break;
            case "DLB":
                axis = Vector3.forward;
                direction = -1;
                side = 1;
                break;
            case "DRF":
                axis = Vector3.forward;
                direction = 1;
                side = 0;
                break;
            case "DRB":
                axis = Vector3.forward;
                direction = 1;
                side = 1;
                break;
        }

        Debug.Log("Collider: " + collider.name);
        
        if(axis != Vector3.zero)
            StartRotation(axis, side);
        
        return axis;
    }

    void RotateFace(Vector3 axis, int direction)
    {
        frontFace.transform.localEulerAngles += axis * (direction * 90);
        PlayShuffleSound();
    }

    void SetFace(GameObject face)
    {
        face.transform.position = transform.position;
        face.transform.rotation = transform.rotation;
        face.transform.SetParent(transform);
        face.layer = LayerMask.NameToLayer("Ignore Raycast"); //ignore raycast
    }

    public void PlayShuffleSound() {
        soundSource.clip = shuffleSounds[Random.Range(0, shuffleSounds.Length)];
        soundSource.pitch = shufflePitchBase - shufflePitchSpan/2 + Random.Range(0, shufflePitchSpan);
        soundSource.Play();
    }

}
