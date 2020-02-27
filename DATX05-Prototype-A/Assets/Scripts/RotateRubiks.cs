using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateRubiks : MonoBehaviour
{
    SmallCube[] smallCubes;
    GameObject frontFace, backFace;
    bool isRotationStarted = false;

    OVRGrabbable grabbable;
    // Start is called before the first frame update
    void Start()
    {
        grabbable = GetComponent<OVRGrabbable>();
        frontFace = new GameObject();
        backFace = new GameObject();
        SetFace(frontFace);
        SetFace(backFace);
        smallCubes = GetComponentsInChildren<SmallCube>();
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
                        int direction = 1;
                        Vector3 axis = PickAxis("LeftHand", out direction);
                        RotateFace(axis, direction);
                        //RotateFront();
                    }

                    if (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) <= 0.5f && isRotationStarted)
                        StopRotation(frontFace);
                    break;

                case "LeftHand":
                    if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > 0.7f && !isRotationStarted)
                    {
                        RotateFront();
                    }

                    if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) <= 0.5f && isRotationStarted)
                        StopRotation(frontFace);
                    break;

                default:
                    if(isRotationStarted)
                        StopRotation(frontFace);
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
                if (smallCube.intPos.x == side)
                {
                    smallCube.transform.SetParent(smallCube.intPos.x == side ? frontFace.transform : backFace.transform);
                    Debug.Log(smallCube.transform.parent.name);
                }
            }
            else if (axis.y > 0)
            {
                if (smallCube.intPos.y == side)
                {
                    smallCube.transform.SetParent(smallCube.intPos.y == side ? frontFace.transform : backFace.transform);
                    Debug.Log(smallCube.transform.parent.name);
                }
            }
            else if (axis.z > 0)
            {
                smallCube.transform.SetParent(smallCube.intPos.z == side ? frontFace.transform : backFace.transform);
                Debug.Log(smallCube.transform.parent.name);
            }
        }
    }

    void StopRotation(GameObject f)
    {
        Quaternion rot = f.transform.localRotation;
        rot.eulerAngles = rot.eulerAngles/90f;
        Vector3 euler = new Vector3();
        euler.x = Mathf.Round(rot.eulerAngles.x);
        euler.y = Mathf.Round(rot.eulerAngles.y);
        euler.z = Mathf.Round(rot.eulerAngles.z);
        euler *= 90f;
        rot.eulerAngles = euler;

        f.transform.localRotation = rot;

        foreach(var smallCube in smallCubes)
        {
            smallCube.transform.SetParent(transform);
        }
        isRotationStarted = false;
        Debug.Log("Rotation Stopped");
    }
    Vector3 PickAxis(string hand, out int direction)
    {
        direction = 0;
        int side = 0;
        int layerMask = 1 << 12;
        Vector3 axis = new Vector3();
        RaycastHit hit;
        GameObject activeHand = GameObject.FindGameObjectWithTag(hand);
        
        if(Physics.Raycast(activeHand.transform.position, activeHand.transform.TransformDirection(Vector3.forward), out hit, 1f, layerMask))
        {
            GameObject hitCube = hit.collider.gameObject;
            SmallCube sCube = hitCube.GetComponent<SmallCube>();
            Debug.Log(hitCube.name);
            if (hit.collider == sCube.colliderFA)
            {
                switch(hitCube.name){
                    case "Cube 0x0x0":
                        axis = Vector3.right;
                        direction = -1;
                        side = 0;
                        break;
                    case "Cube 0x1x0":
                        axis = Vector3.right;
                        direction = 1;
                        side = 0;
                        break;
                    case "Cube 1x0x0":
                        axis = Vector3.up;
                        direction = -1;
                        side = 0;
                        break;
                    case "Cube 1x1x0":
                        axis = Vector3.right;
                        direction = 1;
                        side = 1;
                        break;
                    case "Cube 0x0x1":
                        axis = Vector3.right;
                        direction = -1;
                        side = 0;
                        break;
                }

                StartRotation(axis, side);
            }
        }   
        return axis;
    }

    void RotateFront()
    {
        StartRotation(Vector3.forward, 0);
        frontFace.transform.localEulerAngles = new Vector3(0, 0, 30);
    }

    void RotateFace(Vector3 axis, int direction)
    {
        frontFace.transform.localEulerAngles = axis * (direction * 90);
    }
    void SetFace(GameObject face)
    {
        face.transform.position = transform.position;
        face.transform.rotation = transform.rotation;
        face.transform.SetParent(transform);
        face.layer = LayerMask.NameToLayer("Ignore Raycast"); //ignore raycast
    }

}
