using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ITutorial : MonoBehaviour
{
    public GameObject[] tutorialSlides;
    public GameObject transitionSlide;

    protected TutorialMaster master;

    protected int currentState;
    protected int nextState;

    // Start is called before the first frame update
    public virtual void Start()
    {
        if (tutorialSlides.Length <= 0)
            Debug.LogError("ITutorial: Tutorial does not have a single slide!");
        
        foreach (GameObject go in tutorialSlides) {
            if (!go)
                Debug.LogError("ITutorial: One of the given tutorial Game Objects does not exist!");
        }

        master = GameMaster.instance.tutorialMaster;
        master.SetMaxStates(tutorialSlides.Length - 1);
    }

    public virtual void TriggerNextSlide(int nextState) {
        this.nextState = nextState;
    }

    public void ArrivedAtSlide(int state) {
        currentState = state;
        master.ArrivedAtSlide(state);
    }
}
