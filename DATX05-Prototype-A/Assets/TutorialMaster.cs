using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialMaster : MonoBehaviour
{
    [Range(0, 10)] public int maxStates = 0;
    public TutorialStateEvent triggerSlideTransitionEvent;
    public TutorialStateEvent setTutorialState;

    [System.Serializable]
    public class TutorialStateEvent : UnityEvent<int> { }

    public int tutorialState {get; private set;}


    // Start is called before the first frame update
    void Start()
    {
        if (maxStates <= 0)
            Debug.LogWarning("TutorialMaster: WARNING - Max States is not set to a positive integer!");

        if (triggerSlideTransitionEvent == null)
            triggerSlideTransitionEvent = new TutorialStateEvent();
        if (setTutorialState == null)
            setTutorialState = new TutorialStateEvent();

        tutorialState = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
