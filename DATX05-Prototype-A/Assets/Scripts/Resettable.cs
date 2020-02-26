using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resettable : MonoBehaviour
{
    public Renderer[] renderers;
    public GameObject resetObjectVolume;
    public float fadeSpeed = 5;
    [Range(0,0.5f)] public float stillnessBuffer = 0.001f;

    private Vector3 startingPosition;
    private Rigidbody rb;
    private Material[] material;
    private BoxCollider startingArea;
    public DistanceGrabbable_EventExtension grabbableScript;
    private Collider myCollider;

    private bool pending = false;
    private bool fadeOut = false;
    private bool fadeIn = false;

    private float alpha;
    private Vector3 previousPosition;

    // Start is called before the first frame update
    void Start()
    {
        if (resetObjectVolume) {
            startingPosition = resetObjectVolume.transform.position;
            startingArea = resetObjectVolume.GetComponent<BoxCollider>();
        } else {
            startingPosition = transform.position;
        }

        rb = GetComponent<Rigidbody>();
        myCollider = GetComponent<Collider>();
        grabbableScript = GetComponent<DistanceGrabbable_EventExtension>();

        if (renderers.Length > 0) {
            material = new Material[renderers.Length];
            for(int i = 0; i < renderers.Length; i++) {
                material[i] = renderers[i].GetComponent<Renderer>().material;
            }
        } else {
            material = new Material[1];
            material[0] = GetComponent<Renderer>().material;
        }

        if (!rb)
            Debug.LogError("Rigidbody was not found!");
        if (!material[0])
            Debug.LogError("Material was not found!");
        if (!myCollider)
            Debug.LogError("Collider was not found!");
        if (!grabbableScript)
            Debug.LogError("Grabber script was not found!");

        // Debug.Log("starting area: "+startingArea+", "+(startingArea != null)+", my collider: "+myCollider);
    }

    // Update is called once per frame
    void Update()
    {
        if (pending) {
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

            // foreach (Material m in material) {
            //     var col = m.color;
            //     col.a = alpha;
            //     m.color = col;
            // }
            UpdateMaterialAlpha();
        }
    }

    private void UpdateMaterialAlpha() {
        foreach (Material m in material) {
            var col = m.color;
            col.a = alpha;
            m.color = col;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag.Equals("ResetVolume")) {
            StartPendingReset();
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.tag.Equals("ResetVolume")) {
            StopPendingReset();
        }
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
        // grabbableScript.enabled = false;
        grabbableScript.allowGrab = false;
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
    }

    private void FadeInDone() {
        rb.isKinematic = false;
        fadeIn = false;
        // grabbableScript.enabled = true;
        grabbableScript.allowGrab = true;
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
        results = new Collider[2];
        int hits = Physics.OverlapBoxNonAlloc(transform.position, transform.localScale / 2, results, transform.rotation, LayerMask.GetMask("Grabbable"));
        return hits > 0;
    }
}
