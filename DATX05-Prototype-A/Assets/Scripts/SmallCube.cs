using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallCube : MonoBehaviour
{
    public Vector3Int intPos;
    public Renderer cubeRenderer;
    public Material defaultMaterial;
    public Material highlightMaterial;
    public float highlightDelay = 0.1f;

    Vector3 floatPos;
    float delay = -1;

    // Start is called before the first frame update
    void Start()
    {
        if (!cubeRenderer)
            Debug.LogError("SmallCube: Cube Renderer was not found!");
        if (!defaultMaterial)
            Debug.LogError("SmallCube: Default Material was not found!");
        if (!highlightMaterial)
            Debug.LogError("SmallCube: Highlight Material was not found!");

        UpdatePosition();
    }

    // Update is called once per frame
    void Update()
    {
        if (delay > 0) delay -= Time.deltaTime;
        if (delay <= 0 && delay > -1) SetHighlighted(false);
    }

    public void RotateFace(Vector3 parentPosition, Vector3 axiz)
    {
        transform.RotateAround(parentPosition, Vector3.forward, 10);
        UpdatePosition();
    }

    public void UpdatePosition()
    {
        floatPos = transform.localPosition + new Vector3(0.05f, 0.05f, 0.05f);
        floatPos *= 10;        

        intPos.x = Mathf.RoundToInt(floatPos.x);
        intPos.y = Mathf.RoundToInt(floatPos.y);
        intPos.z = Mathf.RoundToInt(floatPos.z);
    }

    public void SetHighlighted(bool isHighlighted) {
        // var mat = cubeRenderer.material;
        if (!isHighlighted) {
            // mat.DisableKeyword("_EMISSION");
            cubeRenderer.material = defaultMaterial;
            delay = -1;
        } else if (isHighlighted) {
            // mat.EnableKeyword("_EMISSION");
            cubeRenderer.material = highlightMaterial;
            delay = highlightDelay;
        }
    }
}
