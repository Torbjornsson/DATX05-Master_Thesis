using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBSlide : MonoBehaviour
{
    public GameObject[] slideContents;
    public SimpleHelvetica textScript;
    public int lineSpacing = 30;
    public bool dynamicPosition = true;

    public Vector3 startingPosition;
    public Vector3 startingRotation;
    public float transitionDepth = 0.3f;
    public float transitionRotation = 5;

    // Start is called before the first frame update
    void Start()
    {
        startingPosition = transform.localPosition;
        startingRotation = transform.localEulerAngles;

        textScript.LineSpacing = lineSpacing;
        
        if (dynamicPosition)
            UpdateTextPosition();
        
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetText(string text) {
        textScript.Text = text;
        textScript.GenerateText();
        if (dynamicPosition) UpdateTextPosition();
    }

    public void UpdateTextPosition() {
        var trans = textScript.gameObject.transform;
        var localPos = trans.localPosition;
        localPos.y = textScript.height / 2 - (textScript.LineSpacing / 2) * textScript.transform.localScale.y;
        trans.localPosition = localPos;
    }

    public void SetTransitionPosition(float progress) {
        var pos = startingPosition;
        pos.z += progress * transitionDepth;
        transform.localPosition = pos;

        var rot = startingRotation;
        rot.x -= progress * transitionRotation;
        rot.y += progress * transitionRotation * 0.1f;
        transform.localEulerAngles = rot;
    }
}
