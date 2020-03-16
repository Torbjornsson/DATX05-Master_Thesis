using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RotateRubiks : MonoBehaviour
{
    public enum Symbol {
        None, Symbol1, Symbol2, Symbol3
    }

    public static Symbol GetSymbol(string tag) {
        //Debug.Log("Tag: " + tag);
        switch(tag) {
            case "RubiksSymbol1": return Symbol.Symbol1;
            case "RubiksSymbol2": return Symbol.Symbol2;
            case "RubiksSymbol3": return Symbol.Symbol3;
            default: break;
        }
        return Symbol.None;
    }


    public AudioClip[] shuffleSounds;
    public AudioSource soundSource;
    [Range(0, 1)] public float shufflePitchSpan = 0.05f;
    [Space]
    public Symbol solutionTopLeft;
    public Symbol solutionTopRight;
    public Symbol solutionBottomLeft;
    public Symbol solutionBottomRight;
    [Space]
    public UnityEvent checkSolutionEvent;
    public RubiksCubeFace[] rubiksCubeFaces;
    public float rotationAngleSnap = 10;
    
    public float speed = 10f;
    SmallCube[] smallCubes;
    GameObject frontFace, backFace, to;
    bool isRotationStarted = false;
    // Vector3 rotationStartingAngle;
    bool rotatedEnough = false;

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
        if (rubiksCubeFaces.Length != 6)
            Debug.LogError(gameObject.name+": The wrong number of RukibsCubeFace found!");
        foreach (RubiksCubeFace f in rubiksCubeFaces) {
            if (!f)
                Debug.LogError(gameObject.name+": One of the given RukibsCubeFace was not found!");
        }

        grabbable = GetComponent<OVRGrabbable>();
        frontFace = new GameObject();
        backFace = new GameObject();
        to = new GameObject();
        SetFace(frontFace);
        SetFace(backFace);
        smallCubes = GetComponentsInChildren<SmallCube>();

        shufflePitchBase = soundSource.pitch;

        hands = new Dictionary<string, GameObject>();
        FindAndSaveHand("LeftHand");
        FindAndSaveHand("RightHand");

        if (checkSolutionEvent == null)
            checkSolutionEvent = new UnityEvent();
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
                if (isRotationStarted)
                    StopRotation();
                break;
        }

        if(isRotationStarted) {
            SmoothRotation();
            
            var angles = frontFace.transform.localRotation.eulerAngles;
            if (!rotatedEnough && angles.magnitude > 90 - rotationAngleSnap && angles.magnitude < 270 + rotationAngleSnap)
                rotatedEnough = true;
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
        // rotationStartingAngle = frontFace.transform.localRotation.eulerAngles;
        rotatedEnough = false;

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
        if (!rotatedEnough) return;

        Vector3Int prevPos;

        Quaternion rot = frontFace.transform.localRotation;
        rot.eulerAngles = rot.eulerAngles/90f;
        Vector3 euler = new Vector3();
        euler.x = Mathf.Round(rot.eulerAngles.x);
        euler.y = Mathf.Round(rot.eulerAngles.y);
        euler.z = Mathf.Round(rot.eulerAngles.z);
        euler *= 90f;

        // if ((rot.eulerAngles - euler).magnitude > rotationAngleSnap)
        //     return;
        // Debug.Log("SNAP! previous angle: "+rotationStartingAngle+", current angle: "+rot.eulerAngles+", new angle: "+euler);

        rot.eulerAngles = euler;

        frontFace.transform.localRotation = rot;

        foreach(var smallCube in smallCubes)
        {
            smallCube.transform.SetParent(transform);
            prevPos = smallCube.intPos;
            smallCube.UpdatePosition();
        }

        SetFace(frontFace);
        SetFace(backFace);
                
        isRotationStarted = false;

        Invoke("CheckSolution", 0.01f);
    }

    private Collider GetSmallCubeCollider(string hand) {
        
        RaycastHit hit;
        GameObject activeHand = hands[hand];

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

        var cubePos = smallCube.transform.position;
        var cubeOut = smallCube.transform.TransformDirection(Vector3.back);

        var projected = Vector3.ProjectOnPlane(handDown, cubeOut);
        var projectedLocal = smallCube.transform.InverseTransformVector(projected);

        var localDir = Vector2Int.zero;
        if (Mathf.Abs(projectedLocal.x) > Mathf.Abs(projectedLocal.y)) {
            localDir.x = (int) Mathf.Sign(projectedLocal.x);
        } else {
            localDir.y = (int) Mathf.Sign(projectedLocal.y);
        }
        
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

        var localAxis = handDir.x == 0 ? Vector3.right : Vector3.up;
        axis = parent.localRotation * localAxis;
        axis = new Vector3(Mathf.Round(axis.x), Mathf.Round(axis.y), Mathf.Round(axis.z));

        direction = handDir.x != 0 ? handDir.x : handDir.y;
        if (axis.x < 0 || axis.y > 0 || (axis.z < 0 && localAxis.x > 0) || (axis.z > 0 && localAxis.y > 0)) direction *= -1;

        if (handDir.y != 0)
            side = collider.transform.localPosition.x < 0 ? 0 : 1;
        else
            side = collider.transform.localPosition.y < 0 ? 0 : 1;
        if (axis.x < 0 || axis.y < 0 || axis.z < 0) side = (side == 0) ? 1 : 0;

        axis = new Vector3(Mathf.Abs(axis.x), Mathf.Abs(axis.y), Mathf.Abs(axis.z));

        if(axis != Vector3.zero)
            StartRotation(axis, side);

        return axis;
    }

    void RotateFace(Vector3 axis, int direction)
    {
        Vector3 euler = frontFace.transform.localEulerAngles + axis * (direction * 90);

        SetRotation(euler);
        
        PlayShuffleSound();
    }

    void SetRotation(Vector3 localEulerAngles)
    {
        to.transform.localPosition = frontFace.transform.localPosition;
        to.transform.localRotation = frontFace.transform.localRotation;
        to.transform.localEulerAngles = localEulerAngles;
    }

    void SmoothRotation()
    {
        if (Quaternion.Angle(frontFace.transform.localRotation, to.transform.localRotation) <= 0.01f)
            StopRotation();

        else 
        {
            frontFace.transform.localRotation = Quaternion.Lerp(frontFace.transform.localRotation, to.transform.localRotation, Time.deltaTime * speed);
        }        
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

    public void CheckSolution() {
        checkSolutionEvent.Invoke();

        bool solutionFound = false;
        foreach (RubiksCubeFace face in rubiksCubeFaces) {
            solutionFound |= face.solutionFound;
            if (solutionFound) break;
            //     Debug.Log("Solution found!!\n"+face.FacesToString());
        }

        // Debug.Log("Solution found? "+solutionFound);

        GameMaster.instance.goalCriteriaSatisfied = solutionFound;
    }
}
