﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBSlide : MonoBehaviour
{
    public GameObject[] slideContents;
    public SimpleHelvetica textScript;
    public int lineSpacing = 30;
    public bool dynamicPosition = true;

    public float transitionDepth = 0.3f;
    public float transitionRotation = 5;
    
    private Vector3[] startingPosition;
    private Vector3[] startingRotation;
    private float[] transitionDepths;
    private float[] transitionRotations;

    public bool initiated {get; private set;}

    // Start is called before the first frame update
    void Start()
    {
        Initiate();
    }

    public void Initiate() {
        if (initiated) return;

        gameObject.SetActive(true);

        // Updating text position
        textScript.LineSpacing = lineSpacing;
        if (dynamicPosition)
            UpdateTextPosition();

        // Saving start positions and transition values
        startingPosition = new Vector3[slideContents.Length];
        startingRotation = new Vector3[slideContents.Length];
        transitionDepths = new float[slideContents.Length];
        transitionRotations = new float[slideContents.Length];

        for (int i = 0; i < slideContents.Length; i++) {
            startingPosition[i] = slideContents[i].transform.localPosition;
            startingRotation[i] = slideContents[i].transform.localEulerAngles;
            transitionDepths[i] = transitionDepth;
            transitionRotations[i] = transitionRotation;
            var col = slideContents[i].GetComponent<Collider>();
            if (col)
            {
                // Debug.Log("Bounds size: "+col.bounds.size);
                var newDepth = col.bounds.size.x;
                if (newDepth > transitionDepth)
                {
                    transitionRotations[i] = (newDepth / transitionDepth) * transitionRotation;
                    // Debug.Log("Bounds size was larger! new rotation: "+transitionRotations[i]);
                    transitionDepths[i] = newDepth;
                }
            }
        }
        
        // Deactivating
        gameObject.SetActive(false);

        initiated = true;
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

    public float UpdateTextPosition() {
        var trans = textScript.gameObject.transform;
        var localPos = trans.localPosition;
        var textYpos = textScript.height / 2 - (textScript.LineSpacing / 2) * textScript.transform.localScale.y;
        localPos.y = textYpos;
        trans.localPosition = localPos;
        return textYpos;
    }

    public void SetTransitionPosition(float progress) {
        for (int i = 0; i < slideContents.Length; i++) {
            var pos = startingPosition[i];
            pos.z += progress * transitionDepths[i];
            slideContents[i].transform.localPosition = pos;

            var rot = startingRotation[i];
            rot.x -= progress * transitionRotations[i];
            rot.y += progress * transitionRotations[i] * 0.1f;
            slideContents[i].transform.localEulerAngles = rot;
        }
    }
}
