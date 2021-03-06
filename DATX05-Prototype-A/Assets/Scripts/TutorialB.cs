﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialB : ITutorial
{

    [Header("Tutorial B Parameters")]
    // public GameObject back;
    // [Range(0,1)] public float backAlpha = 0.8f;
    public float transitionFadeSpeed = 1;
    [Space]
    public AudioClip transitionIntroSound;
    public AudioClip transitionOutroSound;

    // private float transitionAlpha = 1;
    private float transitionProgress = 1;
    private bool outro, intro, middleIntro, middleOutro, winningOutro, winningIntro;
    private TutorialBSlide[] slideScripts;
    private TutorialBSlide transitionSlideScript;
    private TutorialBSlide winningSlideScript;
    private TutorialBSlide fadeScript;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        if (!audioSource)
            Debug.LogError("TutorialB: AudioSource not found!");
        if (!transitionIntroSound)
            Debug.LogError("TutorialB: Audio clip \"transition intro\" not found!");
        if (!transitionOutroSound)
            Debug.LogError("TutorialB: Audio clip \"Transition Outro\" not found!");
        baseTransitionPitch = audioSource.pitch;

        // Finding slide scripts, to interact directly with
        slideScripts = new TutorialBSlide[activeSlides.Count];
        for (int i = 0; i < slideScripts.Length; i++)
        {
            slideScripts[i] = activeSlides[i].GetComponent<TutorialBSlide>();
            slideScripts[i].Initiate();
        }
        transitionSlideScript = transitionSlide.GetComponent<TutorialBSlide>();
        transitionSlideScript.Initiate();
        winningSlideScript = winningSlide.GetComponent<TutorialBSlide>();
        winningSlideScript.Initiate();

        // Show the first slide!
        activeSlides[0].SetActive(true);
    }

    protected override void DistributeTextToSlide(string text, GameObject slide)
    {
        // Debug.Log("Distribute text to slide: "+text);
        slide.GetComponent<TutorialBSlide>().SetText(text);

        if (useOnBoarding && slide.name.Contains("OnBoarding"))
        {
            string name = "CubeForPuzzle" + tutorialForPuzzle.ToString();
            var graphics = slide.transform.GetChild(1);
            var onBoardCube = graphics.Find("OnBoardCube");
            var cubeForPuzzle = graphics.Find(name);
            cubeForPuzzle.gameObject.SetActive(true);
            cubeForPuzzle.position = onBoardCube.position;
            cubeForPuzzle.rotation = onBoardCube.rotation;
            // cubeForPuzzle.localScale = onBoardCube.localScale;
            cubeForPuzzle.parent = onBoardCube;
        }
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        // While transition in progress
        if (IsTransitioning())
        {
            // Outro for previous slide or transition-slide
            if (outro || middleOutro || winningOutro)
            {
                if (transitionProgress > 0)
                    transitionProgress -= Time.deltaTime * transitionFadeSpeed;

                if (transitionProgress <= 0)
                {
                    transitionProgress = 0;

                    // Trigger intro for transition-slide
                    if (outro)
                    {
                        outro = false;
                        middleIntro = true;
                        fadeScript.gameObject.SetActive(false);
                        fadeScript = transitionSlideScript;
                        fadeScript.gameObject.SetActive(true);
                        // Debug.Log("Outro done, starting middle-intro");

                        // Trigger intro for next slide
                    }
                    else if (middleOutro)
                    {
                        middleOutro = false;
                        intro = true;
                        fadeScript.gameObject.SetActive(false);
                        fadeScript = slideScripts[nextState];
                        fadeScript.gameObject.SetActive(true);
                        // Debug.Log("Middle Outro done, starting intro");
                        PlaySound(transitionIntroSound);

                        // Trigger intro for winning slide
                    }
                    else if (winningOutro)
                    {
                        winningOutro = false;
                        winningIntro = true;
                        fadeScript.gameObject.SetActive(false);
                        fadeScript = winningSlideScript;
                        fadeScript.gameObject.SetActive(true);
                        // Debug.Log("Winning Outro done, starting Winning Intro");
                    }
                }

                // fadeScript.SetAlpha(transitionAlpha);
                fadeScript.SetTransitionPosition(1 - transitionProgress);
            }
            // Intro for transition-slide or next slide
            else if (intro || middleIntro || winningIntro)
            {
                if (transitionProgress < 1)
                    transitionProgress += Time.deltaTime * transitionFadeSpeed;

                if (transitionProgress >= 1)
                {
                    transitionProgress = 1;

                    // Transition is all done!
                    if (intro)
                    {
                        intro = false;
                        ArrivedAtSlide(nextState);
                        // Debug.Log("Intro done, arrived at next state! "+nextState);

                        // Trigger outro for transition-slide
                    }
                    else if (middleIntro)
                    {
                        middleIntro = false;
                        middleOutro = true;

                        // Stop fading because at win-state
                    }
                    else if (winningIntro)
                    {
                        winningIntro = false;
                        atWinState = true;
                        // Debug.Log("Winning Intro done, THE END");
                    }
                }

                // Update alpha for whatever slide is active
                // fadeScript.SetAlpha(transitionAlpha);
                fadeScript.SetTransitionPosition(1 - transitionProgress);
            }
        }
    }

    public override void TriggerNextSlide(int nextState)
    {
        if (!isActiveAndEnabled) return;
        base.TriggerNextSlide(nextState);

        transitionProgress = 1;
        outro = true;
        intro = false;
        middleIntro = false;
        middleOutro = false;
        winningOutro = false;
        winningIntro = false;
        fadeScript = slideScripts[currentState];

        PlaySound(transitionOutroSound);

        // Debug.Log("Trigger next slide: starting outro - current: "+currentState+", next: "+nextState);
    }

    public override void TriggerWinSlide()
    {
        if (!isActiveAndEnabled) return;
        base.TriggerWinSlide();

        winningOutro = true;
        winningIntro = false;
        fadeScript = slideScripts[currentState];
        
        PlaySound(transitionOutroSound);
    }
}
