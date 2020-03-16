using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialC : ITutorial
{
    private GameObject to;
    public float speed = 10f;
    private bool isRotating = false;
    // Start is called before the first frame update
    public override void Start()
    {
        to = new GameObject();
        to.transform.position = transform.position;
        to.transform.rotation = transform.rotation;

    }

    // Update is called once per frame
    public override void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.Any) && !isRotating)
        {
            toRotation();
        }
        if (isRotating)
        {
            RotateCube();
        }
    }

    public override void TriggerNextSlide(int nextState)
    {
        base.TriggerNextSlide(nextState);
        SpawnSlide();
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

    void SpawnSlide()
    {

    }

    void DespawnSlide()
    {

    }
}
