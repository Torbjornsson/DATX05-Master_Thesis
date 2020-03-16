using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubiksBoxScript : MonoBehaviour
{
    public GameObject hint;
    public BoxCollider myCollider;

    public float hintDelay = 0.4f;
    
    private float delay = 0;
    
    // private RotateRubiks.Symbol activeSymbol;

    // Start is called before the first frame update
    void Start()
    {
        ShowHint(false);
        // activeSymbol = RotateRubiks.Symbol.None;

        myCollider = GetComponent<BoxCollider>();
        if (!myCollider)
            Debug.LogError(gameObject.name+": Collider was not found!");
    }

    // Update is called once per frame
    void Update()
    {
        if (delay > 0) delay -= Time.deltaTime;
        if (delay <= 0 && hint.activeSelf) ShowHint(false);
    }

    public void ShowHint(bool show) {
        ShowHint(show, Vector2Int.up);
    }

    public void ShowHint(bool show, Vector2Int localDir) {
        hint.SetActive(show);
        delay = hintDelay;

        if (localDir.x != 0) {
            if (localDir.x > 0)
                hint.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, -90));
            else
                hint.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 90));
        } else {
            if (localDir.y > 0)
                hint.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
            else
                hint.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 180));
        }
    }

    public RotateRubiks.Symbol GetActiveSymbol() {
        var symbol = RotateRubiks.Symbol.None;

        var results = new Collider[10];
        var center = transform.position;
        var halfExtents = myCollider.bounds.size / 4; // Delar på 4 istället för 2 för att kompensera att rubiks-kuben är skala 0.7
        int hits = Physics.OverlapBoxNonAlloc(center, halfExtents, results, transform.rotation, LayerMask.GetMask("RubiksFace"));

        if (hits > 0)
        {
            symbol = RotateRubiks.GetSymbol(results[0].tag);
            // if (hits > 1)
            //     Debug.Log("Many hits! Second: "+RotateRubiks.GetSymbol(results[1].tag)+", collider size: "+myCollider.bounds.size);
        }
        return symbol;
    }
}
