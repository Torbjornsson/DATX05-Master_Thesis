using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WinningSlot : MonoBehaviour
{
    public bool cubeCanFallIn = true;
    public bool canAttachTileWhenCubeInSlot = true;
    public bool active = true;
    [Space]
    public bool neverendingWinFade = false;
    public Renderer winRenderer;
    public Renderer failRenderer;
    public bool useVLight = true;
    public VLight winLight;
    public VLight failLight;
    public float fadeSpeed = 1;
    public float fadeWait = 0.5f;
    [Space]
    public ParticleSystem[] confettiEmitter;
    [Space]
    public AudioClip winSound;
    [Range (0,1)] public float winVolume = 0.5f;
    public AudioClip failSound;
    [Range (0,1)] public float failVolume = 0.25f;
    public AudioSource soundSource;
    [Space]
    public UnityEvent winEvent;
    public UnityEvent failEvent;

    private GameObject puzzleCubeCloseToSlot = null;
    private OVRGrabbable_EventExtension puzzleCubeGrabbable = null;
    private AttachableTarget puzzleCubeAttachableTarget = null;
    private bool puzzleCubeInSlot = false;
    private bool cubeWasGrabbedLastFrame = false;
    private bool cubeHadAttachedTileLastFrame = false;

    private bool winHasBeenTriggered = false;

    private bool fading = false;
    private Renderer currentFade = null;
    private VLight currentLight = null;
    private float fadeAlpha = 0;
    private bool fadeIn = true;
    private float fadeTime = 0;
    private float fadeLightMax;

    // Start is called before the first frame update
    void Start()
    {
        if (confettiEmitter.Length == 0 || !confettiEmitter[0])
            Debug.LogError(gameObject.name + ": Confetti Particle System was not found!");
        if (!soundSource)
            Debug.LogError(gameObject.name + ": Audio Source was not found!");
        if (!winSound)
            Debug.LogError(gameObject.name + ": Win Audio Clip was not found!");
        if (!failSound)
            Debug.LogError(gameObject.name + ": Fail Audio Clip was not found!");
            
        if (!winRenderer)
            Debug.LogError(gameObject.name + ": Win Renderer was not found!");
        if (!failRenderer)
            Debug.LogError(gameObject.name + ": Fail Renderer was not found!");

        if (winLight || failLight)
            fadeLightMax = winLight ? winLight.lightMultiplier : failLight.lightMultiplier;
        
        if (winEvent == null)
            winEvent = new UnityEvent();
        if (failEvent == null)
            failEvent = new UnityEvent();
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
                && !puzzleCubeGrabbable.isGrabbed && (cubeCanFallIn || justReleasedCube))
        {
            Debug.Log("Attaching cube to slot and checking win/fail");

            SnapCube(puzzleCubeCloseToSlot);
            puzzleCubeInSlot = true;

            if (puzzleCubeAttachableTarget != null && !canAttachTileWhenCubeInSlot)
            {
                puzzleCubeAttachableTarget.allowAttaching = false;
                Debug.Log("Disable attaching!");
            }

            CheckIfWinOrFail();
        }

        // When grabbing the cube out of the slot, release it
        if (puzzleCubeCloseToSlot != null && puzzleCubeInSlot && puzzleCubeGrabbable.isGrabbed)
        {
            Debug.Log("Detaching cube from slot");

            puzzleCubeInSlot = false;

            if (puzzleCubeAttachableTarget != null) puzzleCubeAttachableTarget.allowAttaching = true;
        }

        // When cube is in slot, and a tile was just added, check win or fail gain
        if (justAttachedTile && puzzleCubeInSlot && !winHasBeenTriggered)
        {
            CheckIfWinOrFail();
        }

        cubeWasGrabbedLastFrame = puzzleCubeCloseToSlot != null && puzzleCubeGrabbable.isGrabbed;
        cubeHadAttachedTileLastFrame = puzzleCubeAttachableTarget != null && puzzleCubeAttachableTarget.attachedObject != null;

        // Fading slot-back when win or fail
        if (fading) {
            if (fadeIn) {
                if (fadeAlpha < 1) fadeAlpha += fadeSpeed * Time.deltaTime;

                if (fadeAlpha >= 1) {
                    fadeAlpha = 1;

                    if (fadeTime > 0) fadeTime -= Time.deltaTime;
                    if (fadeTime <= 0) {
                        fadeTime = 0;
                        fadeIn = false;
                    }
                }
            } else {
                if (fadeAlpha > 0) fadeAlpha -= fadeSpeed * Time.deltaTime;

                if (fadeAlpha <= 0) {
                    fadeAlpha = 0;

                    if (neverendingWinFade && winHasBeenTriggered) {
                        if (fadeTime < fadeWait) fadeTime += Time.deltaTime;
                        if (fadeTime >= fadeWait) {
                            fadeTime = fadeWait;
                            fadeIn = true;
                        }
                        
                    } else {
                        fading = false;
                        currentFade.gameObject.SetActive(false);
                        if (useVLight && currentLight) currentLight.gameObject.SetActive(false);
                    }
                }
            }

            var col = currentFade.material.color;
            col.a = fadeAlpha;
            currentFade.material.color = col;

            if (useVLight && currentLight) currentLight.lightMultiplier = fadeLightMax * fadeAlpha;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("PuzzleCube"))
        {
            puzzleCubeCloseToSlot = other.gameObject;

            puzzleCubeGrabbable = puzzleCubeCloseToSlot.GetComponent<OVRGrabbable_EventExtension>();
            puzzleCubeAttachableTarget = puzzleCubeCloseToSlot.GetComponent<AttachableTarget>();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag.Equals("PuzzleCube") && other.gameObject.Equals(puzzleCubeCloseToSlot))
        {
            puzzleCubeCloseToSlot = null;
            puzzleCubeInSlot = false;

            if (puzzleCubeAttachableTarget != null) puzzleCubeAttachableTarget.allowAttaching = true;

            puzzleCubeGrabbable = null;
            puzzleCubeAttachableTarget = null;
        }
    }

    public void CheckIfWinOrFail()
    {
        if (GameMaster.instance.goalCriteriaSatisfied && GameMaster.instance.LastSlideReached)
        {
            TriggerWin();
        }
        else
        {
            TriggerFail();
        }
    }

    public void TriggerWin()
    {
        puzzleCubeGrabbable.allowGrab = false;

        // Disable attached tile as well
        if (puzzleCubeAttachableTarget != null)
        {
            puzzleCubeAttachableTarget.attachedObject.GetComponent<OVRGrabbable_EventExtension>().allowGrab = false;
        }

        winHasBeenTriggered = true;
        PlayWinSound();
        Debug.Log("WON THE GAME!!!!");

        winEvent.Invoke();
        GameMaster.instance.PuzzleWon();
        
        TriggerFade(true);

        // Confetti!
        Invoke("ThrowConfetti", 5.7f);
    }

    public void ThrowConfetti()
    {
        foreach (var confetti in confettiEmitter)
        {
            confetti.Play();
        }
    }

    public void TriggerFail()
    {
        PlayFailSound();
        failEvent.Invoke();
        TriggerFade(false);
        Debug.Log("Lost the game...");
    }

    public void TriggerFade(bool win) {
        fading = true;
        currentFade = win ? winRenderer : failRenderer;
        currentLight = win ? winLight : failLight;
        fadeIn = true;
        fadeTime = fadeWait;
        fadeAlpha = 0;
        
        currentFade.gameObject.SetActive(true);
        var col = currentFade.material.color;
        col.a = fadeAlpha;
        currentFade.material.color = col;

        if (useVLight && currentLight) {
            currentLight.gameObject.SetActive(true);
            currentLight.lightMultiplier = 0;
        }
    }

    public void PlayWinSound()
    {
        soundSource.clip = winSound;
        soundSource.volume = winVolume;
        soundSource.Play();
    }

    public void PlayFailSound()
    {
        soundSource.clip = failSound;
        soundSource.volume = failVolume;
        soundSource.Play();
    }

    private void SnapCube(GameObject cube)
    {
        var pos = transform.position;
        pos.y += 0.07f; // There are probably better ways of doing it...
        cube.transform.position = pos;

        var cubeRotation = cube.transform.localRotation;
        var slotRotation = transform.localRotation;

        Vector3 rotationOffset = RotationOffset(slotRotation, cubeRotation);

        cube.transform.rotation = transform.rotation;
        cube.transform.Rotate(rotationOffset, Space.Self);
        cube.GetComponent<Rigidbody>().isKinematic = true;
    }

    private Vector3 RotationOffset(Quaternion slotRotation, Quaternion cubeRotation)
    {
        Vector3 rotationOffset = Vector3.zero;

        var diff = cubeRotation.eulerAngles - slotRotation.eulerAngles;

        rotationOffset.x = RotationOffsetInAxis(diff.x);
        rotationOffset.y = RotationOffsetInAxis(diff.y);
        rotationOffset.z = RotationOffsetInAxis(diff.z);

        return rotationOffset;
    }

    private float RotationOffsetInAxis(float axisDiff)
    {
        return (float)(System.Math.Round(axisDiff / 90f, System.MidpointRounding.AwayFromZero) * 90f);
    }
}