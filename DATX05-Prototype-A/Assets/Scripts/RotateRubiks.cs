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
            Debug.Log("Grabbed by: " + grabbable.grabbedBy.tag);
            switch(grabbable.grabbedBy.tag)
            {
                case "RightHand":
                    if (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) > 0.7f && !isRotationStarted)
                    {
                        RotateFront();
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
                    Debug.Log(smallCube.gameObject.name);
                    smallCube.transform.SetParent(frontFace.transform);
                }
            }
            else if (axis.y > 0)
            {
                if (smallCube.intPos.y == side)
                {
                    Debug.Log(smallCube.gameObject.name);
                    smallCube.transform.SetParent(frontFace.transform);
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
    void RotateFront()
    {
        StartRotation(Vector3.forward, 0);
        frontFace.transform.localEulerAngles = new Vector3(0, 0, 30);
    }

    void SetFace(GameObject face)
    {
        face.transform.position = transform.position;
        face.transform.rotation = transform.rotation;
        face.transform.SetParent(transform);
    }

}
