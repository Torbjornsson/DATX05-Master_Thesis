using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialC : ITutorial
{
    private GameObject to;
    public float speed = 10f;
    private bool isRotating = false;
    private Queue slides = new Queue();
    int i = 0;
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        to = new GameObject();
        to.transform.position = transform.position;
        to.transform.rotation = transform.rotation;

        SpawnSlide(base.GetSlides(tutorialForPuzzle)[0]);
        toRotation();
    }

    // Update is called once per frame
    public override void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.Any) && !isRotating)
        {
            // if (i < 3)
            // {
            //     TriggerNextSlide(i);
            //     i++;
            // }
        }
        if (isRotating)
        {
            RotateCube();
        }
    }

    public override void TriggerNextSlide(int nextState)
    {
        base.TriggerNextSlide(nextState);
        SpawnSlide(base.GetSlides(tutorialForPuzzle)[nextState]);
        toRotation();
    }

    protected override void DistributeTextToSlide(string text, GameObject slide)
    {

    }

    public override void TriggerWinSlide()
    {
        base.TriggerWinSlide();
        
    }

    void RotateCube()
    {
        if (Quaternion.Angle(transform.localRotation, to.transform.localRotation) <= 0.01f)
        {
            isRotating = false;
            transform.localRotation = to.transform.localRotation;
            DespawnSlide();
        }
        else
        {
            transform.localRotation = Quaternion.Lerp(transform.localRotation, to.transform.localRotation, Time.deltaTime * speed);
        }
    }

    void toRotation()
    {
        to.transform.localRotation = transform.localRotation;
        to.transform.Rotate(new Vector3(0,90,0), Space.Self);
        isRotating = true;
    }

    void SpawnSlide(GameObject slide)
    {
        slides.Enqueue(slide);
        slide.active = true;
        slide.transform.parent = transform;
    }

    void DespawnSlide()
    {
        if (slides.Count > 3)
        {
            GameObject slide = (GameObject)slides.Dequeue();
            slide.active = false;
            slide.transform.parent = GameObject.Find("Slides").transform;
        }
            
    }
}
