using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ITutorial : MonoBehaviour
{
    [Header("ITutorial Parameters")]
    [Range(1,3)] public int tutorialForPuzzle = 1;
    public bool useOnBoarding = false;
    [Space]
    public GameObject[] onBoardingSlides;
    public GameObject[] puzzle1Slides;
    public GameObject[] puzzle2Slides;
    public GameObject[] puzzle3Slides;
    public GameObject transitionSlide;
    public GameObject winningSlide;
    [Space]
    public TextAsset onBoardingTexts;
    public TextAsset puzzle1Texts;
    public TextAsset puzzle2Texts;
    public TextAsset puzzle3Texts;

    protected List<GameObject> activeSlides;

    protected TutorialMaster master;

    protected int currentState;
    protected int nextState;
    protected bool atWinState;

    // Start is called before the first frame update
    public virtual void Start()
    {
        // Checking that all slide arrays are there
        if (onBoardingSlides.Length <= 0)
            Debug.LogError("ITutorial: Tutorial does not have a single On-Boading-slide!");
        if (puzzle1Slides.Length <= 0)
            Debug.LogError("ITutorial: Tutorial does not have a single Puzzle 1 slide!");
        if (puzzle2Slides.Length <= 0)
            Debug.LogError("ITutorial: Tutorial does not have a single Puzzle 2 slide!");
        if (puzzle3Slides.Length <= 0)
            Debug.LogError("ITutorial: Tutorial does not have a single Puzzle 3 slide!");
        
        // Checking each item in every slide array
        foreach (GameObject go in onBoardingSlides) {
            if (!go)
                Debug.LogError("ITutorial: One of the given On-Boarding slide Game Objects does not exist!");
        }
        foreach (GameObject go in puzzle1Slides) {
            if (!go)
                Debug.LogError("ITutorial: One of the given Puzzle 1 slide Game Objects does not exist!");
        }
        foreach (GameObject go in puzzle2Slides) {
            if (!go)
                Debug.LogError("ITutorial: One of the given Puzzle 2 slide Game Objects does not exist!");
        }
        foreach (GameObject go in puzzle3Slides) {
            if (!go)
                Debug.LogError("ITutorial: One of the given Puzzle 3 slide Game Objects does not exist!");
        }
        
        // Checking for tutorial texts
        if (!onBoardingTexts)
            Debug.LogError("ITutorial: On-Boarding tutorial texts not found!");
        if (!puzzle1Texts)
            Debug.LogError("ITutorial: Puzzle 1 tutorial texts not found!");
        if (!puzzle2Texts)
            Debug.LogError("ITutorial: Puzzle 2 tutorial texts not found!");
        if (!puzzle3Texts)
            Debug.LogError("ITutorial: Puzzle 3 tutorial texts not found!");

        // Compile active slides
        activeSlides = CompileTutorialSlides();

        // Letting tutorial master know how many total slides there are
        master = GameMaster.instance.tutorialMaster;
        master.SetMaxStates(activeSlides.Count - 1);

        // Distribute text files into the active slides
        DistributeTexts();

        // Show the first slide!
        activeSlides[0].SetActive(true);
    }

    public virtual void Update()
    {
        // Checking for winning
        if (GameMaster.instance.hasWon && !IsTransitioning() && !atWinState) {
            TriggerWinSlide();
        }
    }

    public virtual void TriggerNextSlide(int nextState) {
        this.nextState = nextState;
    }

    public virtual void TriggerWinSlide() {
        nextState++;
    }

    public void ArrivedAtSlide(int state) {
        currentState = state;
        master.ArrivedAtSlide(state);
    }

    // Compiling the active slides for this specific tutorial
    protected List<GameObject> CompileTutorialSlides() {
        var list = new List<GameObject>();
        if (useOnBoarding)
            list.AddRange(onBoardingSlides);
        list.AddRange(GetSlides(tutorialForPuzzle));
        Debug.Log("Compiled list of slides size: "+list.Count);
        return list;
    }

    protected void DistributeTexts() {
        if (useOnBoarding)
            DistributeTexts(onBoardingTexts, onBoardingSlides);
        DistributeTexts(GetTexts(tutorialForPuzzle), GetSlides(tutorialForPuzzle));
    }
    
    protected void DistributeTexts(TextAsset texts, GameObject[] slides) {
        var textsSeparated = GetTextsFromFile(texts);
        for(int i = 0; i < slides.Length; i++) {
            if (textsSeparated.Length > i) {
                var text = textsSeparated[i].Trim(' ', '\n', '\r');
                DistributeTextToSlide(text, slides[i]);
            }
        }
    }

    protected abstract void DistributeTextToSlide(string text, GameObject slide);

    protected string[] GetTextsFromFile(TextAsset textFile) {
        return textFile.text.Split(';');
    }

    protected GameObject[] GetSlides(int puzzleNumber) {
        switch(tutorialForPuzzle) {
            case 1: return puzzle1Slides;
            case 2: return puzzle2Slides;
            case 3: return puzzle3Slides;
        }
        throw new ArgumentException("ITutorial.GetSlides() : Puzzle number can only be 1, 2 or 3!");
    }

    protected TextAsset GetTexts(int puzzleNumber) {
        switch(tutorialForPuzzle) {
            case 1: return puzzle1Texts;
            case 2: return puzzle2Texts;
            case 3: return puzzle3Texts;
        }
        throw new ArgumentException("ITutorial.GetSlides() : Puzzle number can only be 1, 2 or 3!");
    }

    public bool IsTransitioning() {
        return currentState != nextState;
    }

    // --- > ON-BOARDING tutorial events
    public void OnFirstGrab() {
        if (useOnBoarding && !IsTransitioning() && currentState == 0) {
            TriggerNextSlide(currentState + 1);
        }
    }

    public void OnSwitchHands() {
        if (useOnBoarding && !IsTransitioning() && currentState == 1) {
            TriggerNextSlide(currentState + 1);
        }
    }

    public void OnObjectReset() {
        if (useOnBoarding && !IsTransitioning() && currentState == 2) {
            TriggerNextSlide(currentState + 1);
        }
    }

    protected int GetRelativeCurrentState() {
        return useOnBoarding ? currentState - onBoardingSlides.Length : currentState;
    }

    // --- > PUZZLE 1 tutorial events
    public void OnTileAttach() {
        if (!IsTransitioning() && tutorialForPuzzle == 1 && GetRelativeCurrentState() == 0) {
            TriggerNextSlide(currentState + 1);
        }
    }

    public void OnTileDetach() {
        if (!IsTransitioning() && tutorialForPuzzle == 1 && GetRelativeCurrentState() == 1) {
            TriggerNextSlide(currentState + 1);
        }
    }

    // --- > PUZZLE 2 tutorial events
    public void OnRotateRubiks() {
        if (!IsTransitioning() && tutorialForPuzzle == 2 && GetRelativeCurrentState() == 0) {
            TriggerNextSlide(currentState + 1);
        }
    }

    public void OnResetRubiks() {
        if (!IsTransitioning() && tutorialForPuzzle == 2 && GetRelativeCurrentState() == 1) {
            TriggerNextSlide(currentState + 1);
        }
    }

}
