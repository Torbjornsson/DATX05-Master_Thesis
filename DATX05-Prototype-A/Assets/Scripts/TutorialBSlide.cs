using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBSlide : MonoBehaviour
{
    public GameObject[] slideContents;
    public SimpleHelvetica textScript;

    // Start is called before the first frame update
    void Start()
    {
        // gameObject.SetActive(false);
        UpdateTextPosition();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetText(string text) {
        textScript.Text = text;
        textScript.GenerateText();
        UpdateTextPosition();
    }

    public void UpdateTextPosition() {

    }
}
