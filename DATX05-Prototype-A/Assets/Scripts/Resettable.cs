using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resettable : MonoBehaviour
{
    public Renderer[] renderers;
    public bool useResetStartingPoint = true;
    public float fadeSpeed = 5;
    [Range(0,0.5f)] public float stillnessBuffer = 0.001f;

    private Vector3 startingPosition;
    private Rigidbody rb;
    public Material[] materials {get; private set;}
    private BoxCollider startingArea;
    private OVRGrabbable_EventExtension grabbableScript;
    private AttachableTarget attachableTarget;
    private Material[] attachedMaterials = null;
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
    private Vector3 previousPosition, originalScale;

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
        grabbableScript = GetComponent<OVRGrabbable_EventExtension>();
        attachableTarget = GetComponent<AttachableTarget>();

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
    }

    // Update is called once per frame
    void Update()
    {
        if (insideHardResetVolume > 0 && !hardReset)
            StartHardReset();

        if (insideResetVolume > 0 && !grabbableScript.isGrabbed && !pending && !fadeOut && !hardReset) {
            StartPendingReset();
        } else  if (insideResetVolume <= 0 && (pending || fadeOut) && !hardReset) {
            StopPendingReset();
        }

        if (attachableTarget != null && attachableTarget.attachedObject != null && attachedMaterials == null) {
            attachedMaterials = attachableTarget.attachedObject.gameObject.GetComponent<Resettable>().materials;
        } else if (attachableTarget != null && attachableTarget.attachedObject == null && attachedMaterials != null) {
            attachedMaterials = null;
        }
        
        if (pending && !grabbableScript.isGrabbed) {
            if (rb.velocity.magnitude < stillnessBuffer) {
                StartFadeOut();
            }
            previousPosition = transform.position;

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

    private void ScaleTransform()
    {
        transform.localScale = originalScale * alpha;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag.Equals("ResetVolume")) {
            insideResetVolume++;

        } else if (other.gameObject.tag.Equals("ResetVolumeHARD")) {
            insideHardResetVolume++;

        } else if (other.gameObject.tag.Equals("RubiksResetVolume") && !grabbableScript.isGrabbed) {
            // insideRubiksResetVolume++;
            rubiksResetActive = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.tag.Equals("ResetVolume")) {
            insideResetVolume--;

        } else if (other.gameObject.tag.Equals("ResetVolumeHARD")) {
            insideHardResetVolume--;

        // } else if (other.gameObject.tag.Equals("RubiksResetVolume")) {
        //     insideRubiksResetVolume--;
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
    }

    private void StartFadeOut() {
        pending = false;
        fadeOut = true;
        grabbableScript.allowGrab = false;
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
                pos += GenerateRandomVector() * Mathf.Max(myCollider.bounds.size.x, myCollider.bounds.size.y, myCollider.bounds.size.z);
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
    }

    private void FadeInDone() {
        rb.isKinematic = false;
        fadeIn = false;
        fadeOut = false;
        grabbableScript.allowGrab = true;
        hardReset = false;
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
        var halfExtents = myCollider.bounds.size / 2;
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
        var halfExtents = myCollider.bounds.size / 2;
        int hits = Physics.OverlapBoxNonAlloc(center, halfExtents, results, transform.rotation, LayerMask.GetMask("Grabbable"));
        
        for(int i = 0; i < hits; i++) {
            if (!results[i].gameObject.Equals(gameObject))
                colliding = true;
        }

        return colliding;
    }
}
