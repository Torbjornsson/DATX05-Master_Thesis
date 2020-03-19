using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialC : ITutorial
{
    public GameObject transitionSlidePrefab;
    private GameObject to, anchor, anchorSlides;
    public float speed = 10f;
    private bool isRotating, isTransitioning = false;
    private bool isStarted = true;
    private Queue slides = new Queue();
    int i = 0;
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        to = new GameObject();
        to.transform.position = transform.position;
        to.transform.rotation = transform.rotation;

        anchor = GameObject.Find("TransitionSlideAnchor");
        anchorSlides = GameObject.Find("Slides");

    }

    // Update is called once per frame
    public override void Update()
    {
        //To spawn the initial slides after all the objects have been loaded
        if (activeSlides.Count > 0 && !isRotating && isStarted)
        {
            isStarted = false;
            SpawnSlide(activeSlides[0]);
            ToRotation(90f);
        }

        if (isRotating)
        {
            RotateCube();
        }
    }

    public override void TriggerNextSlide(int nextState)
    {
        base.TriggerNextSlide(nextState);

        SpawnSlide(base.activeSlides[nextState]);
        ToRotation(90f);
    }

    protected override void DistributeTextToSlide(string text, GameObject slide)
    {
        slide.GetComponent<TutorialBSlide>().SetText(text);
    }

    public override void TriggerWinSlide()
    {
        base.TriggerWinSlide();
        SpawnSlide(winningSlide);
        atWinState = true;
        ToRotation(90f);
    }

    void RotateCube()
    {
        if (Quaternion.Angle(transform.localRotation, to.transform.localRotation) <= 0.01f)
        {
            isRotating = false;
            transform.localRotation = to.transform.localRotation;
            DespawnSlide();
            if (isTransitioning)
            {
                isTransitioning = false;
                ArrivedAtSlide(nextState);
                Debug.Log(IsTransitioning());
            }
            else
            {
                isTransitioning = true;
                if (currentState < GetSlides(tutorialForPuzzle).Length - 2)
                    SpawnTransitionSlide();
                else if (currentState == GetSlides(tutorialForPuzzle).Length - 2)
                {
                    //atWinState = true;
                    winningSlide.GetComponent<TutorialBSlide>().enabled = false;
                    winningSlide.transform.position = anchor.transform.position;
                    winningSlide.transform.rotation = anchor.transform.rotation;
                    SpawnSlide(winningSlide);
                    ToRotation(90f);
                }
            }
        }
        else
        {
            transform.localRotation = Quaternion.Lerp(transform.localRotation, to.transform.localRotation, Time.deltaTime * speed);
        }
    }

    void SpawnTransitionSlide()
    {
        GameObject tSlide = Instantiate(transitionSlidePrefab, anchor.transform.position, anchor.transform.rotation);
        tSlide.GetComponent<TutorialBSlide>().enabled = false;
        SpawnSlide(tSlide);
        ToRotation(90f);
    }
    void ToRotation(float angle)
    {
        to.transform.localRotation = transform.localRotation;
        to.transform.Rotate(new Vector3(0, angle, 0), Space.Self);
        isRotating = true;
    }

    void SpawnSlide(GameObject slide)
    {
        Debug.Log("Spawning slide: " + slide.name);
        slides.Enqueue(slide);
        slide.SetActive(true);
        slide.transform.parent = transform;
    }

    void DespawnSlide()
    {
        if (slides.Count > 3)
        {
            GameObject slide = (GameObject)slides.Dequeue();
            Debug.Log("Despawning slide: " + slide.name);
            slide.SetActive(false);
            slide.transform.parent = anchorSlides.transform;
        }
    }
}
