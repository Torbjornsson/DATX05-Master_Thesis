using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialC : ITutorial
{
    public GameObject transitionSlidePrefab;
    private GameObject to, anchor, anchorSlides, rotatingCube;
    public float rotationSpeed = 10f;
    public float waitTimer = 0f;
    private bool isRotating, isTransitioning = false;
    private bool isStarted = true;
    private Queue slides = new Queue();
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        for (int i = 0; i < activeSlides.Count; i++) {
            var slideScript = activeSlides[i].GetComponent<TutorialBSlide>();
            if (slideScript) slideScript.Initiate();
        }
        
        // Show the first slide!
        activeSlides[0].SetActive(true);

        to = new GameObject();
        to.transform.position = transform.position;
        to.transform.rotation = transform.rotation;

        rotatingCube = GameObject.Find("TutorialC_Cube");
        anchor = GameObject.Find("TransitionSlideAnchor");
        anchorSlides = GameObject.Find("Slides");
    }

    // Update is called once per frame
    public override void Update()
    {
        //To check for the wincondition
        base.Update();
        
        //To spawn the initial slides after all the objects have been loaded
        if (activeSlides.Count > 0 && !isRotating && isStarted)
        {
            isStarted = false;
            SpawnSlide(activeSlides[0], false);
            rotatingCube.transform.Rotate(new Vector3(0, 90, 0), Space.Self);
            SpawnTransitionSlide(false);
            rotatingCube.transform.Rotate(new Vector3(0, 90, 0), Space.Self);
        }

        if (isRotating && waitTimer <= 0)
        {
            RotateCube();
        }

        if (waitTimer > 0f)
        {
            waitTimer -= Time.deltaTime;
        }
    }

    public override void TriggerNextSlide(int nextState)
    {
        if (!isActiveAndEnabled) return;
        base.TriggerNextSlide(nextState);

        Debug.Log("Trigger next slide: "+nextState);

        SpawnSlide(base.activeSlides[nextState]);
    }

    protected override void DistributeTextToSlide(string text, GameObject slide)
    {
        slide.GetComponent<TutorialBSlide>().SetText(text);

        if (useOnBoarding && slide.name.Contains("OnBoarding"))
        {
            string name = "CubeForPuzzle" + tutorialForPuzzle.ToString();
            var graphics = slide.transform.GetChild(1);
            var onBoardCube = graphics.Find("OnBoardCube");
            var cubeForPuzzle = graphics.Find(name).gameObject;
            cubeForPuzzle.SetActive(true);
            cubeForPuzzle.transform.position = onBoardCube.position;
            cubeForPuzzle.transform.rotation = onBoardCube.rotation;
            // cubeForPuzzle.transform.localScale = onBoardCube.localScale;
        }
    }

    public override void TriggerWinSlide()
    {
        if (!isActiveAndEnabled) return;
        base.TriggerWinSlide();
        ToRotation(90f);
    }

    void RotateCube()
    {
        if (Quaternion.Angle(rotatingCube.transform.localRotation, to.transform.localRotation) <= 0.01f)
        {
            isRotating = false;
            rotatingCube.transform.localRotation = to.transform.localRotation;
            DespawnSlide();
            if (isTransitioning)
            {
                isTransitioning = false;
                ArrivedAtSlide(nextState);
            }
            else
            {
                isTransitioning = true;
                Transitioning();
            }
        }
        else
        {
            rotatingCube.transform.localRotation = Quaternion.Lerp(rotatingCube.transform.localRotation, to.transform.localRotation, Time.deltaTime * rotationSpeed);
        }
    }

    void Transitioning()
    {
        waitTimer = 1f;
        if (currentState < activeSlides.Count - 2)
            SpawnTransitionSlide(true);

        else if (currentState == activeSlides.Count - 2)
        {
            winningSlide.GetComponent<TutorialBSlide>().enabled = false;
            winningSlide.transform.position = anchor.transform.position;
            winningSlide.transform.rotation = anchor.transform.rotation;
            winningSlide.GetComponentInChildren<SimpleHelvetica>().AddBoxCollider();
            SpawnSlide(winningSlide);
        }
    }

    void SpawnTransitionSlide(bool rotate)
    {
        GameObject tSlide = Instantiate(transitionSlidePrefab, anchor.transform.position, anchor.transform.rotation);
        tSlide.GetComponent<TutorialBSlide>().enabled = false;
        tSlide.GetComponentInChildren<SimpleHelvetica>().AddBoxCollider();
        SpawnSlide(tSlide, rotate);
    }

    void ToRotation(float angle)
    {
        to.transform.localRotation = rotatingCube.transform.localRotation;
        to.transform.Rotate(new Vector3(0, angle, 0), Space.Self);
        isRotating = true;
    }

    void SpawnSlide(GameObject slide, bool rotate)
    {
        Debug.Log("Spawning slide: " + slide.name);
        slides.Enqueue(slide);
        slide.SetActive(true);
        slide.transform.parent = rotatingCube.transform;

        if (slide.name.Contains("Puzzle") || slide.name.Contains("OnBoarding"))
        {
            slide.transform.GetChild(0).Rotate(new Vector3(0, -7, 0), Space.Self);
        }

        if (rotate)
        {
            ToRotation(90f);
        }
    }

    void SpawnSlide(GameObject slide)
    {
        SpawnSlide(slide, true);
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
