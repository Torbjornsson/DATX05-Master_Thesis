using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubiksBoxScript : MonoBehaviour
{
    public GameObject hint;

    public float hintDelay = 0.4f;
    
    private float delay = 0;

    // Start is called before the first frame update
    void Start()
    {
        ShowHint(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (delay > 0) delay -= Time.deltaTime;
        if (delay <= 0 && hint.activeSelf) ShowHint(false);
    }

    public void ShowHint(bool show) {
        hint.SetActive(show);
        delay = hintDelay;
    }
}
