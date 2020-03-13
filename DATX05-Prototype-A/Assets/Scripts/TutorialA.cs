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
    private bool outro, intro, middleIntro, middleOutro;
    private TutorialASlide[] slideScripts;
    private TutorialASlide transitionSlideScript;
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
    }

    protected override void DistributeTextToSlide(string text, GameObject slide) {
        var textComponent = slide.GetComponentInChildren<TextMesh>();
        textComponent.text = text;
    }

    // Update is called once per frame
    void Update()
    {
        if (nextState != currentState) {
            if (outro || middleOutro) {
                if (transitionAlpha > 0)
                    transitionAlpha -= Time.deltaTime * transitionFadeSpeed;

                if (transitionAlpha <= 0) {
                    transitionAlpha = 0;
                    if (outro) {
                        outro = false;
                        middleIntro = true;
                        fadeScript.gameObject.SetActive(false);
                        fadeScript = transitionSlideScript;
                        fadeScript.gameObject.SetActive(true);
                        // Debug.Log("Outro done, starting middle-intro");
                    } else if (middleOutro) {
                        middleOutro = false;
                        intro = true;
                        fadeScript.gameObject.SetActive(false);
                        fadeScript = slideScripts[nextState];
                        fadeScript.gameObject.SetActive(true);
                        // Debug.Log("Middle Outro done, starting intro");
                    }
                }

                fadeScript.SetAlpha(transitionAlpha);
            }
            if (intro || middleIntro) {
                if (transitionAlpha < 1)
                    transitionAlpha += Time.deltaTime * transitionFadeSpeed;

                if (transitionAlpha >= 1) {
                    transitionAlpha = 1;
                    if (intro) {
                        intro = false;
                        ArrivedAtSlide(nextState);
                        // Debug.Log("Intro done, arrived at next state! "+nextState);
                    } else if (middleIntro) {
                        middleIntro = false;
                        middleOutro = true;
                    }
                }
                
                fadeScript.SetAlpha(transitionAlpha);
            }
        }
    }
    
    public override void TriggerNextSlide(int nextState) {
        base.TriggerNextSlide(nextState);

        transitionAlpha = 1;
        outro = true;
        intro = false;
        middleIntro = false;
        middleOutro = false;
        fadeScript = slideScripts[currentState];
        
        // Debug.Log("Trigger next slide: starting outro - current: "+currentState+", next: "+nextState);
    }
}
