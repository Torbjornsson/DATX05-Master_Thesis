using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialA : ITutorial
{

    [Header("Tutorial A Parameters")]
    public GameObject back;
    [Range(0,1)] public float backAlpha = 0.8f;
    public float transitionFadeSpeed = 1;

    private float transitionAlpha = 1;
    private bool outro, intro, middleIntro, middleOutro, winningOutro, winningIntro;
    private TutorialASlide[] slideScripts;
    private TutorialASlide transitionSlideScript;
    private TutorialASlide winningSlideScript;
    private TutorialASlide fadeScript;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        if (!back)
            Debug.LogError("TutorialA: Did not find Back Game Object");
        
        // Setting back alpha
        var mat = back.GetComponent<Renderer>().material;
        var col = mat.color;
        col.a = backAlpha;
        mat.color = col;

        // Finding slide scripts, to interact directly with
        slideScripts = new TutorialASlide[activeSlides.Count];
        for (int i = 0; i < slideScripts.Length; i++) {
            slideScripts[i] = activeSlides[i].GetComponent<TutorialASlide>();
        }
        transitionSlideScript = transitionSlide.GetComponent<TutorialASlide>();
        winningSlideScript = winningSlide.GetComponent<TutorialASlide>();
    }

    protected override void DistributeTextToSlide(string text, GameObject slide) {
        var textComponent = slide.GetComponentInChildren<TextMesh>();
        textComponent.text = text;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        // While transition in progress
        if (IsTransitioning()) {
            // Outro for previous slide or transition-slide
            if (outro || middleOutro || winningOutro) {
                if (transitionAlpha > 0)
                    transitionAlpha -= Time.deltaTime * transitionFadeSpeed;

                if (transitionAlpha <= 0) {
                    transitionAlpha = 0;

                    // Trigger intro for transition-slide
                    if (outro) {
                        outro = false;
                        middleIntro = true;
                        fadeScript.gameObject.SetActive(false);
                        fadeScript = transitionSlideScript;
                        fadeScript.gameObject.SetActive(true);
                        // Debug.Log("Outro done, starting middle-intro");

                    // Trigger intro for next slide
                    } else if (middleOutro) {
                        middleOutro = false;
                        intro = true;
                        fadeScript.gameObject.SetActive(false);
                        fadeScript = slideScripts[nextState];
                        fadeScript.gameObject.SetActive(true);
                        // Debug.Log("Middle Outro done, starting intro");

                    // Trigger intro for winning slide
                    } else if (winningOutro) {
                        winningOutro = false;
                        winningIntro = true;
                        fadeScript.gameObject.SetActive(false);
                        fadeScript = winningSlideScript;
                        fadeScript.gameObject.SetActive(true);
                        // Debug.Log("Winning Outro done, starting Winning Intro");
                    }
                }

                fadeScript.SetAlpha(transitionAlpha);
            }
            // Intro for transition-slide or next slide
            else if (intro || middleIntro || winningIntro) {
                if (transitionAlpha < 1)
                    transitionAlpha += Time.deltaTime * transitionFadeSpeed;

                if (transitionAlpha >= 1) {
                    transitionAlpha = 1;

                    // Transition is all done!
                    if (intro) {
                        intro = false;
                        ArrivedAtSlide(nextState);
                        // Debug.Log("Intro done, arrived at next state! "+nextState);

                    // Trigger outro for transition-slide
                    } else if (middleIntro) {
                        middleIntro = false;
                        middleOutro = true;

                    // Stop fading because at win-state
                    } else if (winningIntro) {
                        winningIntro = false;
                        atWinState = true;
                        // Debug.Log("Winning Intro done, THE END");
                    }
                }
                
                // Update alpha for whatever slide is active
                fadeScript.SetAlpha(transitionAlpha);
            }
        }
    }
    
    public override void TriggerNextSlide(int nextState)
    {
        if (!isActiveAndEnabled) return;
        base.TriggerNextSlide(nextState);

        transitionAlpha = 1;
        outro = true;
        intro = false;
        middleIntro = false;
        middleOutro = false;
        winningOutro = false;
        winningIntro = false;
        fadeScript = slideScripts[currentState];
        
        // Debug.Log("Trigger next slide: starting outro - current: "+currentState+", next: "+nextState);
    }

    public override void TriggerWinSlide() {
        if (!isActiveAndEnabled) return;
        base.TriggerWinSlide();
        
        winningOutro = true;
        winningIntro = false;
        fadeScript = slideScripts[currentState];
    }
}
