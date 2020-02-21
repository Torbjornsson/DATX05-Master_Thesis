using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resettable : MonoBehaviour
{
    public GameObject resetObjectVolume;
    public float fadeSpeed = 5;
    [Range(0,0.5f)] public float stillnessBuffer = 0.001f;

    private Vector3 startingPosition;
    private Rigidbody rb;
    private Material material;
    private BoxCollider startingArea;
    private Collider myCollider;

    private bool pending = false;
    private bool fadeOut = false;
    private bool fadeIn = false;

    private float alpha;
    private Vector3 previousPosition;
    // private bool collidingWithAnything;
    // private bool outsideStartingArea;

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
        material = GetComponent<Renderer>().material;
        myCollider = GetComponent<Collider>();

        if (!rb)
            Debug.LogError("Rigidbody was not found!");
        if (!material)
            Debug.LogError("Material was not found!");
        if (!myCollider)
            Debug.LogError("Collider was not found!");

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
            var col = material.color;
            col.a = alpha;
            material.color = col;
        }

        // collidingWithAnything = false;
        // outsideStartingArea = startingArea != null;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag.Equals("ResetVolume")) {
            StartPendingReset();
        }
    }

    // private void OnTriggerStay(Collider other) {
    //     if (other.Equals(startingArea)) {
    //         outsideStartingArea = false;
    //     }
    // }

    // private void OnCollisionStay(Collision collisionInfo) {
    //     if (collisionInfo.collider.gameObject.tag.Equals("Grabbable")) {
    //         collidingWithAnything = true;
    //     }
    // }

    private void StartPendingReset() {
        pending = true;
        fadeOut = false;
        fadeIn = false;
        previousPosition = transform.position;
        alpha = 1;
    }

    private void StartFadeOut() {
        pending = false;
        fadeOut = true;
    }

    private void ResetToStartingPositon() {
        int infCounter = 0;
        transform.position = startingPosition;
        // bool outsideStartingArea = startingArea != null && !startingArea.bounds.Intersects(myCollider.bounds);
        // bool outsideStartingArea = startingArea != null && Physics;

        bool outsideStartingArea = OutsideStartingArea();
        bool collidingWithAnything = CollidingWithOtherGrabbables();

        while (infCounter < 50 && (outsideStartingArea || collidingWithAnything)) {
            // Debug.Log("Count: "+infCounter+", outside starting area: "+outsideStartingArea+", colliding with anything: "+collidingWithAnything);
            if (outsideStartingArea)
                transform.position = startingPosition;
            else if (collidingWithAnything) {
                var pos = transform.position;
                pos += GenerateRandomVector() * transform.localScale.x;
                transform.position = pos;
            }
            // outsideStartingArea = startingArea != null && !startingArea.bounds.Intersects(myCollider.bounds);
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
