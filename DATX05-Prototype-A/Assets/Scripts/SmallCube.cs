using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallCube : MonoBehaviour
{
    public Vector3Int intPos;
    Vector3 floatPos;

    // Start is called before the first frame update
    void Start()
    {
        UpdatePosition();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void RotateFace(Vector3 parentPosition, Vector3 axiz)
    {
        transform.RotateAround(parentPosition, Vector3.forward, 10);
        UpdatePosition();
    }
    public void UpdatePosition()
    {
        floatPos = transform.localPosition + new Vector3(0.05f, 0.05f, 0.05f);
        floatPos *= 10;        

        intPos.x = Mathf.RoundToInt(floatPos.x);
        intPos.y = Mathf.RoundToInt(floatPos.y);
        intPos.z = Mathf.RoundToInt(floatPos.z);
    }
}
