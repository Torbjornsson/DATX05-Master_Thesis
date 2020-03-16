using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBSlide : MonoBehaviour
{
    public GameObject[] slideContents;
    public SimpleHelvetica textScript;
    public int lineSpacing = 30;

    // Start is called before the first frame update
    void Start()
    {
        // gameObject.SetActive(false);
        textScript.LineSpacing = lineSpacing;
        // SetText("Testing text with several new rows:\nBla bla bla\n\nBla bla bla\n\nBla bla bla");
        UpdateTextPosition();
        // textScript.
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
        // Debug.Log("Text height: "+textScript.height);
        var trans = textScript.gameObject.transform;
        var localPos = trans.localPosition;
        localPos.y = textScript.height / 2 - (textScript.LineSpacing / 2) * textScript.transform.localScale.y;
        trans.localPosition = localPos;
    }
}
