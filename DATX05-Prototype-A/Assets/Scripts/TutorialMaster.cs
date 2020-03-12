using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialMaster : MonoBehaviour
{
    // [Range(0, 10)] public int maxStates = 0;
    public TutorialStateEvent triggerSlideTransitionEvent;
    public TutorialStateEvent setTutorialState;
    // public UnityEvent firstGrabEvent;

    [System.Serializable]
    public class TutorialStateEvent : UnityEvent<int> { }

    public int tutorialState {get; private set;}

    private int maxStates = 0;

    private bool firstGrab = false;


    // Start is called before the first frame update
    void Start()
    {
        // if (maxStates <= 0)
        //     Debug.LogWarning("TutorialMaster: WARNING - Max States is not set to a positive integer!");

        if (triggerSlideTransitionEvent == null)
            triggerSlideTransitionEvent = new TutorialStateEvent();
        if (setTutorialState == null)
            setTutorialState = new TutorialStateEvent();
        // if (firstGrabEvent == null)
        //     firstGrabEvent = new UnityEvent();

        tutorialState = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!firstGrab) {
            var leftHand = GameObject.Find("DistanceGrabHandLeft").GetComponent<OVRGrabber>();
            var rightHand = GameObject.Find("DistanceGrabHandRight").GetComponent<OVRGrabber>();

            if (leftHand && leftHand.grabbedObject || rightHand && rightHand.grabbedObject) {
                // firstGrabEvent.Invoke();
                firstGrab = true;
                if (tutorialState == 0) TriggerNextSlide();
            }
        }
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
}
