using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialASlide : MonoBehaviour
{
    public Renderer[] slideRenderers;

    // Start is called before the first frame update
    void Start()
    {
        // gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetAlpha(float alpha) {
        if (alpha < 0 || alpha > 1)
            throw new System.ArgumentException("Tutorial A Slides.SetAlpha() : Alpha value must be between 0 and 1, inclusive!");
        
        foreach(Renderer r in slideRenderers) {
            var mat = r.material;
            var col = mat.color;
            col.a = alpha;
            mat.color = col;
        }
    }
}
