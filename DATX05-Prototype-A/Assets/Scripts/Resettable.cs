using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resettable : MonoBehaviour
{
    public Renderer[] renderers;
    public bool useResetStartingPoint = true;
    public float fadeSpeed = 5;
    [Range(0,0.5f)] public float stillnessBuffer = 0.001f;
    [Space]
    public AudioSource resetSoundSource;
    public AudioClip resetOutSound;
    public AudioClip resetInSound;
    public float soundSize = 1;
    public float pitchSpan = 0.1f;

    private Vector3 startingPosition;
    private Rigidbody rb;
    public Material[] materials {get; private set;}
    private BoxCollider startingArea;
    private OVRGrabbable_EventExtension grabbableScript;
    private AttachableTarget attachableTargetScript;
    private Attachable attachableScript;
    private Material[] attachedMaterials = null;
    private Resettable attachedResettable = null;
    private Collider myCollider;

    private int insideResetVolume = 0;
    private int insideHardResetVolume = 0;
    // private int insideRubiksResetVolume = 0;

    private bool rubiksResetActive = false;
    private RotateRubiks rubiksScript = null;

    private bool pending = false;
    private bool fadeOut = false;
    private bool fadeIn = false;
    private bool hardReset = false;

    private float alpha;
    private Vector3 previousPosition, originalScale, originalColliderBounds;

    private float startingPitch;

    // Start is called before the first frame update
    void Start()
    {
        GameObject resetObjectVolume = GameObject.FindGameObjectWithTag("ResetStartingPoint");
        if (useResetStartingPoint && resetObjectVolume) {
            startingPosition = resetObjectVolume.transform.position;
            startingArea = resetObjectVolume.GetComponent<BoxCollider>();
        } else {
            startingPosition = transform.position;
        }

        rb = GetComponent<Rigidbody>();
        myCollider = GetComponent<Collider>();
        originalColliderBounds = myCollider.bounds.size;

        grabbableScript = GetComponent<OVRGrabbable_EventExtension>();
        attachableTargetScript = GetComponent<AttachableTarget>();
        attachableScript = GetComponent<Attachable>();

        rubiksScript = GetComponent<RotateRubiks>();

        if (renderers.Length > 0) {
            materials = new Material[renderers.Length];
            for(int i = 0; i < renderers.Length; i++) {
                materials[i] = renderers[i].material;
            }
        } else {
            materials = new Material[1];
            materials[0] = GetComponentInChildren<Renderer>().material;
        }

        if (!rb)
            Debug.LogError(gameObject.name+": Rigidbody was not found!");
        if (!materials[0])
            Debug.LogError(gameObject.name+": Material was not found!");
        if (!myCollider)
            Debug.LogError(gameObject.name+": Collider was not found!");
        if (!grabbableScript)
            Debug.LogError(gameObject.name+": Grabber script was not found!");

        originalScale = transform.localScale;
        
        if (!resetSoundSource)
            Debug.LogError(gameObject.name+": Reset sound Audio Source was not found!");
        startingPitch = resetSoundSource.pitch;
        if (!resetOutSound)
            Debug.LogError(gameObject.name+": Reset OUT sound Audio Clip was not found!");
        if (!resetInSound)
            Debug.LogError(gameObject.name+": Reset IN sound Audio Clip was not found!");
    }

    // Update is called once per frame
    void Update()
    {
        // Hard reset goes directly without pending
        if (insideHardResetVolume > 0 && !hardReset)
            StartHardReset();

        // No resetting at all when attached to other object
        if (attachableScript != null && attachableScript.attachedTo != null) return;

        // Start or stop pending
        if (insideResetVolume > 0 && !grabbableScript.isGrabbed && !pending && !fadeOut && !hardReset) {
            StartPendingReset();
        } else  if (insideResetVolume <= 0 && (pending || fadeOut) && !hardReset && alpha == 1) {
            StopPendingReset();
        }

        // Making sure attached materials are accounted for (MAY BE OBSOLETE!)
        if (attachableTargetScript != null && attachableTargetScript.attachedObject != null && attachedMaterials == null) {
            attachedMaterials = attachableTargetScript.attachedObject.gameObject.GetComponent<Resettable>().materials;
        } else if (attachableTargetScript != null && attachableTargetScript.attachedObject == null && attachedMaterials != null) {
            attachedMaterials = null;
        }

        // Making sure attached resettable is accounted for
        if (attachableTargetScript != null && attachableTargetScript.attachedObject && attachedResettable == null) {
            attachedResettable = attachableTargetScript.attachedObject.GetComponent<Resettable>();
        } else if ((attachableTargetScript == null || !attachableTargetScript.attachedObject) && attachedResettable != null) {
            attachedResettable = null;
        }
        
        // When pending reset
        if (pending && !grabbableScript.isGrabbed) {
            Debug.Log("Pending... stillness: "+rb.velocity.magnitude+" vs stillness-buffer: "+stillnessBuffer);
            if (rb.velocity.magnitude < stillnessBuffer) {
                StartFadeOut();
            }
            previousPosition = transform.position;

        // When fade in or fade out is active
        } else if (fadeOut || fadeIn) {

            if (fadeOut) {
                if (alpha > 0)
                    alpha -= fadeSpeed * Time.deltaTime;
                if (alpha <= 0) {
                    alpha = 0;
                    ResetToStartingPositon();
                }

            } else if (fadeIn) {
                if (alpha < 1)
                    alpha += fadeSpeed * Time.deltaTime;
                if (alpha >= 1) {
                    alpha = 1;
                    FadeInDone();
                }
            }

            UpdateMaterialAlpha();
            ScaleTransform();
        }

        // Resetting cube if getting stuck
        if (rb.isKinematic && !fadeIn && !grabbableScript.isGrabbed && !GameMaster.instance.hasWon)
        {
            alpha = 1;
            FadeInDone();
            UpdateMaterialAlpha();
            ScaleTransform();
        }
    }

    private void UpdateMaterialAlpha() {
        foreach (Material m in materials) {
            var col = m.color;
            col.a = alpha;
            m.color = col;
        }
        if (attachedMaterials != null) {
            foreach (Material m in attachedMaterials) {
                var col = m.color;
                col.a = alpha;
                m.color = col;
            }
        }
    }

    private void ScaleTransform() {
        ScaleTransform(alpha);
    }
    private void ScaleTransform(float a)
    {
        transform.localScale = originalScale * a;
        if (attachedResettable != null) attachedResettable.ScaleTransform(a);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag.Equals("ResetVolume")) {
            insideResetVolume++;

        } else if (other.gameObject.tag.Equals("ResetVolumeHARD")) {
            insideHardResetVolume++;

        } else if (other.gameObject.tag.Equals("RubiksResetVolume") && !grabbableScript.isGrabbed) {
            rubiksResetActive = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.tag.Equals("ResetVolume")) {
            insideResetVolume--;

        } else if (other.gameObject.tag.Equals("ResetVolumeHARD")) {
            insideHardResetVolume--;
        }
    }

    private void StartHardReset() {
        if (grabbableScript.isGrabbed)
            grabbableScript.grabbedBy.ForceRelease(grabbableScript);
        StartPendingReset();
        StartFadeOut();
        hardReset = true;
    }

    private void StartPendingReset() {
        pending = true;
        fadeOut = false;
        fadeIn = false;
        previousPosition = transform.position;
        alpha = 1;
        Debug.Log("Pending: START");
    }

    private void StopPendingReset() {
        if (pending) {
            pending = false;
        }
        if (fadeOut) {
            fadeOut = false;
            alpha = 1;
            UpdateMaterialAlpha();
        }
        Debug.Log("Pending: STOP");
    }

    private void StartFadeOut() {
        pending = false;
        fadeOut = true;
        grabbableScript.allowGrab = false;
        PlaySound(resetOutSound);
        Debug.Log("Fade-out: START");
    }

    private void ResetToStartingPositon() {
        int infCounter = 0;
        transform.position = startingPosition;

        bool outsideStartingArea = OutsideStartingArea();
        bool collidingWithAnything = CollidingWithOtherGrabbables();

        while (infCounter < 50 && (outsideStartingArea || collidingWithAnything)) {
            if (outsideStartingArea) {
                transform.position = startingPosition;
            } else if (collidingWithAnything) {
                var pos = transform.position;
                pos += GenerateRandomVector() * Mathf.Max(originalColliderBounds.x, originalColliderBounds.y, originalColliderBounds.z);
                transform.position = pos;
            }
            outsideStartingArea = OutsideStartingArea();
            collidingWithAnything = CollidingWithOtherGrabbables();
            infCounter++;
        }

        rb.velocity = Vector3.zero;
        pending = false;
        fadeOut = false;
        fadeIn = true;
        rb.isKinematic = true;

        GameMaster.instance.tutorialMaster.ObjectResetted();

        if (rubiksScript != null && rubiksResetActive) {
            rubiksScript.ResetCube();
            rubiksResetActive = false;
        }

        PlaySound(resetInSound);
        Debug.Log("Resetting to start position!");
    }

    private void FadeInDone() {
        rb.isKinematic = false;
        fadeIn = false;
        fadeOut = false;
        grabbableScript.allowGrab = true;
        hardReset = false;
        Debug.Log("Fade-in: DONE");
    }

    public void SetResetPosition(Vector3 position) {
        startingPosition = position;
    }

    private Vector3 GenerateRandomVector() {
        float min = -1;
        float max = 1;
        var v = new Vector3(Random.Range(min, max), Random.Range(0, max), Random.Range(min, max));
        return v.normalized;
    }

    private static int resultsSize = 20;
    Collider[] results;
    private bool OutsideStartingArea() {
        bool colliding = false;
        results = new Collider[resultsSize];
        
        var center = transform.position;
        var halfExtents = originalColliderBounds / 2;
        int hits = Physics.OverlapBoxNonAlloc(center, halfExtents, results, transform.rotation);
        
        for(int i = 0; i < hits; i++) {
            if (results[i].tag.Equals("ResetStartingPoint"))
                colliding = true;
        }
        
        return !colliding;
    }

    private bool CollidingWithOtherGrabbables() {
        bool colliding = false;
        results = new Collider[resultsSize];
        
        var center = transform.position;
        var halfExtents = originalColliderBounds / 2;
        int hits = Physics.OverlapBoxNonAlloc(center, halfExtents, results, transform.rotation, LayerMask.GetMask("Grabbable"));
        
        for(int i = 0; i < hits; i++) {
            if (!results[i].gameObject.Equals(gameObject))
                colliding = true;
        }

        return colliding;
    }

    public void PlaySound(AudioClip sound) {
        resetSoundSource.clip = sound;
        var size = (1 + (1 - soundSize));
        resetSoundSource.pitch = (startingPitch - pitchSpan/2 + Random.Range(0, pitchSpan)) * size;
        resetSoundSource.Play();
    }
}
