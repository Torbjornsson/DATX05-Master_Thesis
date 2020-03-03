﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinningSlot : MonoBehaviour
{
    public bool cubeCanFallIn = true;
    public bool canAttachTileWhenCubeInSlot = true;
    public bool active = true;
    [Space]
    public AudioClip win;
    public AudioClip fail;
    public AudioSource soundSource;

    private GameObject puzzleCubeCloseToSlot = null;
    private OVRGrabbable_EventExtension puzzleCubeGrabbable = null;
    private AttachableTarget puzzleCubeAttachableTarget = null;
    private bool puzzleCubeInSlot = false;
    private bool cubeWasGrabbedLastFrame = false;
    private bool cubeHadAttachedTileLastFrame = false;

    private bool winHasBeenTriggered = false;

    // Start is called before the first frame update
    void Start()
    {
        if (!soundSource)
            Debug.LogError("Winning Slot: Audio Source was not found!");
        if (!win)
            Debug.LogError("Winning Slot: Win Audio Clip was not found!");
        if (!fail)
            Debug.LogError("Winning Slot: Fail Audio Clip was not found!");
    }

    // Update is called once per frame
    void Update()
    {
        var justReleasedCube = puzzleCubeCloseToSlot != null
            && cubeWasGrabbedLastFrame && !puzzleCubeGrabbable.isGrabbed;
        var justAttachedTile = puzzleCubeAttachableTarget != null
            && !cubeHadAttachedTileLastFrame && puzzleCubeAttachableTarget.attachedObject != null;

        // When cube ends up in slot, snap it into position, and check win or fail
        if (active && puzzleCubeCloseToSlot != null && !puzzleCubeInSlot
                && !puzzleCubeGrabbable.isGrabbed && (cubeCanFallIn || justReleasedCube)) {
            var pos = transform.position;
            pos.y += 0.07f; // There are probably better ways of doing it...
            puzzleCubeCloseToSlot.transform.position = pos;
            puzzleCubeCloseToSlot.transform.rotation = transform.rotation;
            puzzleCubeCloseToSlot.GetComponent<Rigidbody>().isKinematic = true;
            puzzleCubeInSlot = true;

            if (puzzleCubeAttachableTarget != null && !canAttachTileWhenCubeInSlot) {
                puzzleCubeAttachableTarget.allowAttaching = false;
                Debug.Log("Disable attaching!");
            }
            
            CheckIfWinOrFail();
        }

        // When grabbing the cube out of the slot, release it
        if (puzzleCubeCloseToSlot != null && puzzleCubeInSlot && puzzleCubeGrabbable.isGrabbed) {
            puzzleCubeInSlot = false;

            if (puzzleCubeAttachableTarget != null) puzzleCubeAttachableTarget.allowAttaching = true;
        }

        // When cube is in slot, and a tile was just added, check win or fail gain
        if (justAttachedTile && puzzleCubeInSlot && !winHasBeenTriggered) {
            CheckIfWinOrFail();
        }

        cubeWasGrabbedLastFrame = puzzleCubeCloseToSlot != null && puzzleCubeGrabbable.isGrabbed;
        cubeHadAttachedTileLastFrame = puzzleCubeAttachableTarget != null && puzzleCubeAttachableTarget.attachedObject != null;
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag.Equals("PuzzleCube")) {
            puzzleCubeCloseToSlot = other.gameObject;

            puzzleCubeGrabbable = puzzleCubeCloseToSlot.GetComponent<OVRGrabbable_EventExtension>();
            puzzleCubeAttachableTarget = puzzleCubeCloseToSlot.GetComponent<AttachableTarget>();
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.gameObject.tag.Equals("PuzzleCube") && other.gameObject.Equals(puzzleCubeCloseToSlot)) {
            puzzleCubeCloseToSlot = null;
            puzzleCubeInSlot = false;

            if (puzzleCubeAttachableTarget != null) puzzleCubeAttachableTarget.allowAttaching = true;

            puzzleCubeGrabbable = null;
            puzzleCubeAttachableTarget = null;
        }
    }

    public void CheckIfWinOrFail() {
        if (GameMaster.instance.goalCriteriaSatisfied) {
            TriggerWin();
        } else {
            TriggerFail();
        }
    }

    public void TriggerWin() {
        puzzleCubeGrabbable.allowGrab = false;

        // Disable attached tile as well
        // var target = puzzleCubeCloseToSlot.gameObject.GetComponent<AttachableTarget>();
        if (puzzleCubeAttachableTarget != null) {
            puzzleCubeAttachableTarget.attachedObject.GetComponent<OVRGrabbable_EventExtension>().allowGrab = false;
        }

        winHasBeenTriggered = true;
        PlayWinSound();
        Debug.Log("WON THE GAME!!!!");
    }

    public void TriggerFail() {
        PlayFailSound();
        Debug.Log("Lost the game...");
    }

    public void PlayWinSound() {
        soundSource.clip = win;
        soundSource.Play();
    }

    public void PlayFailSound() {
        soundSource.clip = fail;
        soundSource.Play();
    }
}
