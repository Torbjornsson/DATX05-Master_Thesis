using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateRubiks : MonoBehaviour
{
    public AudioClip[] shuffleSounds;
    public AudioSource soundSource;
    [Range(0, 1)] public float shufflePitchSpan = 0.05f;
    
    SmallCube[] smallCubes;
    GameObject frontFace, backFace;
    bool isRotationStarted = false;

    private float shufflePitchBase;

    OVRGrabbable grabbable;
    // Start is called before the first frame update
    void Start()
    {
        if (shuffleSounds.Length <= 0)
            Debug.LogError("Rotate Rubiks: No shuffle sounds were found!");
        if (!soundSource)
            Debug.LogError("Rotate Rubiks: Audio Source was not found!");

        grabbable = GetComponent<OVRGrabbable>();
        frontFace = new GameObject();
        backFace = new GameObject();
        SetFace(frontFace);
        SetFace(backFace);
        smallCubes = GetComponentsInChildren<SmallCube>();

        shufflePitchBase = soundSource.pitch;
    }



    // Update is called once per frame
    void Update()
    {
        if (grabbable.isGrabbed)
        {
            switch(grabbable.grabbedBy.tag)
            {
                case "RightHand":
                    if (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) > 0.7f && !isRotationStarted)
                    {
                        int direction = 0;
                        Vector3 axis = PickAxis("LeftHand", out direction);
                        if (axis != Vector3.zero && direction != 0 && frontFace.transform.childCount == 4)
                            RotateFace(axis, direction);
                    }

                    if (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) <= 0.5f && isRotationStarted)
                        StopRotation();
                    break;

                case "LeftHand":
                    if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > 0.7f && !isRotationStarted)
                    {
                        int direction = 0;
                        Vector3 axis = PickAxis("RightHand", out direction);
                        if (axis != Vector3.zero && direction != 0 && frontFace.transform.childCount == 4)
                            RotateFace(axis, direction);
                    }

                    if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) <= 0.5f && isRotationStarted)
                        StopRotation();
                    break;

                default:
                    if(isRotationStarted)
                        StopRotation();
                    break;
            }
        }
    }

    void StartRotation(Vector3 axis, int side)
    {
        Debug.Log("Rotation Started");
        isRotationStarted = true;

        foreach (var smallCube in smallCubes)
        {
            if (axis.x > 0)
            {
                smallCube.transform.SetParent(smallCube.intPos.x == side ? frontFace.transform : backFace.transform);
            }

            else if (axis.y > 0)
            {
                smallCube.transform.SetParent(smallCube.intPos.y == side ? frontFace.transform : backFace.transform);
            }

            else if (axis.z > 0)
            {
                smallCube.transform.SetParent(smallCube.intPos.z == side ? frontFace.transform : backFace.transform);
            }
        }
        Debug.Log("Rotates cubes: " + frontFace.transform.childCount);
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
            Debug.Log("from position " + prevPos + " to position " + smallCube.intPos);
        }

        SetFace(frontFace);
        SetFace(backFace);
                
        isRotationStarted = false;
        Debug.Log("Rotation Stopped");
    }
    Vector3 PickAxis(string hand, out int direction)
    {
        direction = 0;
        int side = 0;
        int layerMask = 1 << 12;
        Vector3 axis = Vector3.zero;
        RaycastHit hit;
        GameObject activeHand = GameObject.FindGameObjectWithTag(hand);
        
        if(Physics.Raycast(activeHand.transform.position, activeHand.transform.TransformDirection(Vector3.forward), out hit, 1f, layerMask))
        {
            Debug.DrawRay(activeHand.transform.position, activeHand.transform.TransformDirection(Vector3.forward), Color.red, 1);
            Debug.Log(hit);
            if (hit.collider)
            {
                
                switch(hit.collider.tag){
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

                Debug.Log("Collider: " + hit.collider.name);
                if(axis != Vector3.zero)
                    StartRotation(axis, side);
            }
        }   
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
