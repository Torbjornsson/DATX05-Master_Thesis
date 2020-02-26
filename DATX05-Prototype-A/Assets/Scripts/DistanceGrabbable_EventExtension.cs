using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using OculusSampleFramework;

public class DistanceGrabbable_EventExtension : DistanceGrabbable
{
    [Space]
    public GrabEvent grabStartEvent;
    public GrabEvent grabEndEvent;

    [System.Serializable]
    public class GrabEvent : UnityEvent<OVRGrabbable> { }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        if (grabStartEvent == null)
            grabStartEvent = new GrabEvent();
        if (grabEndEvent == null)
            grabEndEvent = new GrabEvent();
    }

    override public void GrabBegin(OVRGrabber hand, Collider grabPoint)
    {
        base.GrabBegin(hand, grabPoint);
        grabStartEvent.Invoke(this);
    }

    override public void GrabEnd(Vector3 linearVelocity, Vector3 angularVelocity)
    {
        base.GrabEnd(linearVelocity, angularVelocity);
        grabEndEvent.Invoke(this);
    }
}
