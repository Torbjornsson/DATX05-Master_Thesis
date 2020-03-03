﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactSoundPlayer : MonoBehaviour
{
    public AudioSource soundSource;
    public AudioClip[] soundClipsNormalImpact;
    public AudioClip[] soundClipsHardImpact;
    public float normalImpactThreshold = 0.1f;
    public float hardImpactThreshold = 0.5f;

    private float debugTime = 0;

    private Rigidbody rb;
    private Vector3 previousVelocity;
    private Vector3 previousAngular;
    private OVRGrabbable_EventExtension grabbableScript;

    // Start is called before the first frame update
    void Start()
    {
        if (!soundSource)
            Debug.LogError("Impact Sound Player: Audio Source was not found!");
        if (soundClipsNormalImpact.Length <= 0)
            Debug.LogError("Impact Sound Player: No Audio Clips was found!");
        
        rb = GetComponent<Rigidbody>();
        grabbableScript = GetComponent<OVRGrabbable_EventExtension>();

        if (!rb)
            Debug.LogError("Impact Sound Player: Rigidbody was found!");
        if (!grabbableScript)
            Debug.LogError("Impact Sound Player: OVR Grabbable was found!");

        previousVelocity = Vector3.zero;
        previousAngular = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        // debugTime += Time.deltaTime;
        // if (debugTime > 5) {
        //     PlaySound();
        //     debugTime = 0;
        // }

        previousVelocity = rb.velocity;
        previousAngular = rb.angularVelocity;
    }

    void OnCollisionEnter(Collision col) {
        if (grabbableScript.isGrabbed) return;

        var vel = previousVelocity.magnitude + previousAngular.magnitude / 3;
        if (vel > hardImpactThreshold)
            PlaySoundHard();
        else if (vel > normalImpactThreshold)
            PlaySound();

        Debug.Log("Previous total velocity: "+vel);
    }

    public void PlaySound() {
        PlaySound(soundClipsNormalImpact);
    }

    public void PlaySoundHard() {
        if (soundClipsHardImpact.Length > 0)
            PlaySound(soundClipsHardImpact);
        else {
            Debug.LogError("Impact Sound Player: No Audio Clips (hard) was found!");
            PlaySound(soundClipsNormalImpact);
        }
    }

    public void PlaySound(AudioClip[] soundClips) {
        soundSource.clip = soundClips[Random.Range(0, soundClips.Length)];
        soundSource.Play();
    }
}