using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialMaster : MonoBehaviour
{
    public enum Hand {
        Right, Left, None
    }

    public TutorialStateEvent triggerSlideTransitionEvent;
    public TutorialStateEvent setTutorialState;

    [System.Serializable]
    public class TutorialStateEvent : UnityEvent<int> { }

    public int tutorialState {get; private set;}

    private ITutorial tutorial;
    private OVRGrabber leftHand, rightHand;

    private int maxStates = 0;

    private bool firstGrab = false;
    private Hand handGrabbing = Hand.None;
    private bool objectResetted;
    private bool tileAttached;
    private bool tileDetached;
    private bool rotatedRubiks;
    private bool resetRubiks;
    private bool cubeScanned;


    // Start is called before the first frame update
    void Start()
    {
        tutorial = GameObject.FindObjectOfType<ITutorial>();
        if (!tutorial)
            Debug.LogError("Tutorial Master: Could not find an instance of ITutorial!");

        if (triggerSlideTransitionEvent == null)
            triggerSlideTransitionEvent = new TutorialStateEvent();
        if (setTutorialState == null)
            setTutorialState = new TutorialStateEvent();

        tutorialState = 0;

        leftHand = GameObject.Find("DistanceGrabHandLeft").GetComponent<OVRGrabber>();
        rightHand = GameObject.Find("DistanceGrabHandRight").GetComponent<OVRGrabber>();
        if (!leftHand || !rightHand)
            Debug.LogError("Tutorial Master: Could not find both OVRGrabber hands!");
    }

    // Update is called once per frame
    void Update()
    {
        // First grab event
        if (!firstGrab) {
            if (leftHand.grabbedObject || rightHand.grabbedObject) {
                firstGrab = true;
                tutorial.OnFirstGrab();
            }
        }

        // Switching hand event
        if (handGrabbing == Hand.Right && leftHand.grabbedObject || handGrabbing == Hand.Left && rightHand.grabbedObject) {
            tutorial.OnSwitchHands();
        }
        // Keeping check of previous grabbed hand
        if (leftHand.grabbedObject) handGrabbing = Hand.Left;
        else if (rightHand.grabbedObject) handGrabbing = Hand.Right;
        else handGrabbing = Hand.None;

        // Resetting object event
        if (objectResetted) tutorial.OnObjectReset();
        objectResetted = false;

        // Attaching tile to cube
        if (tileAttached) tutorial.OnTileAttach();
        tileAttached = false;

        // Detaching tile from cube
        if (tileDetached) tutorial.OnTileDetach();
        tileDetached = false;

        // Rotating rubiks
        if (rotatedRubiks) tutorial.OnRotateRubiks();
        rotatedRubiks = false;

        // Resetting rubiks
        if (resetRubiks) tutorial.OnResetRubiks();
        resetRubiks = false;

        // Scanning cube
        if (cubeScanned) tutorial.OnCubeScanned();
        cubeScanned = false;
    }

    public void TriggerNextSlide() {
        triggerSlideTransitionEvent.Invoke(tutorialState + 1);
    }

    public void IncrementTutorialState() {
        tutorialState++;
    }

    public void ArrivedAtSlide(int state) {
        tutorialState = state;
    }

    public void SeTutorialState(int state) {
        tutorialState = state;
        setTutorialState.Invoke(state);
    }

    public void SetMaxStates(int max) {
        if (max <= 0)
            Debug.LogError("TutorialMaster.SetMaxStats(): Max States needs to be at least 1!");
        maxStates = max;
    }

    public void ObjectResetted() {
        objectResetted = true;
    }

    public void TileAttached() {
        tileAttached = true;
    }

    public void TileDetached() {
        tileDetached = true;
    }

    public void RotatedRubiks() {
        rotatedRubiks = true;
    }

    public void ResetRubiks() {
        resetRubiks = true;
    }

    public void ScanningCube() {
        cubeScanned = true;
    }
}
