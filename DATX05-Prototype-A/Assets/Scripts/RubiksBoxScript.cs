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
}
