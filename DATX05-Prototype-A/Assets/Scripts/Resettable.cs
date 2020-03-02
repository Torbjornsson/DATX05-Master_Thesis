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

    private bool pending = false;
    private bool fadeOut = false;
    private bool fadeIn = false;
    private bool hardReset = false;

    private float alpha;
    private Vector3 previousPosition;

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

        // Debug.Log("starting area: "+startingArea+", "+(startingArea != null)+", my collider: "+myCollider);
    }

    // Update is called once per frame
    void Update()
    {
        // if (!pending && !fadeOut && !fadeIn)
        //     Debug.Log("inside reset volume: "+insideResetVolume);

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
        
        if (pending) {
            if (rb.velocity.magnitude < stillnessBuffer) {
                StartFadeOut();
            }
            previousPosition = transform.position;
            // Debug.Log("Pending, velocity: "+rb.velocity);

        } else if (fadeOut || fadeIn) {

            if (fadeOut) {
                // Debug.Log("Fade OUT, alpha: "+alpha);
                if (alpha > 0)
                    alpha -= fadeSpeed * Time.deltaTime;
                if (alpha <= 0) {
                    alpha = 0;
                    ResetToStartingPositon();
                }

            } else if (fadeIn) {
                // Debug.Log("Fade IN, alpha: "+alpha);
                if (alpha < 1)
                    alpha += fadeSpeed * Time.deltaTime;
                if (alpha >= 1) {
                    alpha = 1;
                    FadeInDone();
                }
            }

            UpdateMaterialAlpha();
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

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag.Equals("ResetVolume")) {
            // StartPendingReset();
            // insideResetVolume = true;
            insideResetVolume++;

        } else if (other.gameObject.tag.Equals("ResetVolumeHARD")) {
            // StartPendingReset();
            // StartFadeOut();
            insideHardResetVolume++;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.tag.Equals("ResetVolume")) {
            // StopPendingReset();
            // insideResetVolume = false;
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
        // Debug.Log(gameObject.name+": Start pending");
        pending = true;
        fadeOut = false;
        fadeIn = false;
        previousPosition = transform.position;
        alpha = 1;
    }

    private void StopPendingReset() {
        // Debug.Log(gameObject.name+": STOP pending");
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
        // grabbableScript.enabled = false;
        grabbableScript.allowGrab = false;
        // Debug.Log("Start Fade");
    }

    private void ResetToStartingPositon() {
        int infCounter = 0;
        transform.position = startingPosition;

        bool outsideStartingArea = OutsideStartingArea();
        bool collidingWithAnything = CollidingWithOtherGrabbables();

        while (infCounter < 50 && (outsideStartingArea || collidingWithAnything)) {
            if (outsideStartingArea)
                transform.position = startingPosition;
            else if (collidingWithAnything) {
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
        // insideResetVolume = false;
        // insideResetVolume = 0;
    }

    private void FadeInDone() {
        rb.isKinematic = false;
        fadeIn = false;
        // grabbableScript.enabled = true;
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
        // results = new Collider[resultsSize];
        // int hits = Physics.OverlapBoxNonAlloc(startingArea.transform.position, startingArea.transform.localScale / 2, results, startingArea.transform.rotation, LayerMask.GetMask("Grabbable"));
        // for (int i = 0; i < hits; i++) {
        //     var c = results[i];
        //     Debug.Log("Is me? "+c+" vs "+myCollider);
        //     if (c.Equals(myCollider))
        //         return false;
        // }
        // return true;
        return false;
    }

    private bool CollidingWithOtherGrabbables() {
        // results = new Collider[resultsSize];
        // int hits = Physics.OverlapBoxNonAlloc(transform.position, transform.localScale / 2, results, transform.rotation, LayerMask.GetMask("Grabbable"));
        // for (int i = 0; i < hits; i++) {
        //     var c = results[i];
        //     Debug.Log("Is me? "+c.gameObject.GetInstanceID()+" vs "+gameObject.GetInstanceID());
        //     if (!c.gameObject.GetInstanceID().Equals(gameObject.GetInstanceID()))
        //         return true;
        // }
        // return false;

        // results = new Collider[2];
        // int hits = Physics.OverlapBoxNonAlloc(transform.position, transform.localScale / 2, results, transform.rotation, LayerMask.GetMask("Grabbable"));
        // return hits > 0;

        results = new Collider[10];
        int hits = Physics.OverlapBoxNonAlloc(transform.position, transform.localScale / 2, results, transform.rotation, LayerMask.GetMask("Grabbable"));
        for(int i = 0; i < hits; i++) {
            if (!results[i].gameObject.Equals(gameObject))
                return true;
        }
        return false;
    }
}
