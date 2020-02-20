using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandMatChangeFix : MonoBehaviour
{

    public Material mat;

    private GameObject handLeft;
    private GameObject handRight;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (handRight == null)
            handRight = GameObject.Find("hand_right_renderPart_0");
        if (handLeft == null)
            handLeft = GameObject.Find("hand_left_renderPart_0");

        if (handRight != null && handLeft != null)
        {
            handLeft.GetComponent<Renderer>().material = mat;
            handRight.GetComponent<Renderer>().material = mat;
            Destroy(GetComponent<HandMatChangeFix>());
        }
    }
}
