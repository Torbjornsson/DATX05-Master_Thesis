﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorLight : MonoBehaviour
{
    public Renderer lightRenderer;
    public float emissionOffIntensity = -10;
    public float emissionOnIntensity = 0;
    public Color albedoOffColor;
    public Color albedoOnColor;
    public float fadeSpeed = 1;

    private Material lightMaterial;
    private Color emissionColor;
    private float emissionIntensity;
    private float abledoColorSpeedRatio;
    private float emissionIntensitySpeedRatio;

    private bool isOn = false;
    private bool fading = false;

    // private float debugDelay = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (!lightRenderer) Debug.LogError("Indicator Light: Light Material was not found!");

        lightMaterial = lightRenderer.material;
        lightMaterial.color = lightMaterial.color;
        emissionColor = lightMaterial.GetColor("_EmissionColor");
        lightMaterial.SetColor("_EmissionColor", emissionColor * emissionOffIntensity);
        emissionIntensity = emissionOffIntensity;

        abledoColorSpeedRatio = Mathf.Abs(albedoOnColor.r - albedoOffColor.r);
        emissionIntensitySpeedRatio = Mathf.Abs(emissionOnIntensity - emissionOffIntensity);
    }

    // Update is called once per frame
    void Update()
    {
        // debugDelay += Time.deltaTime;
        // if (debugDelay > 5 && !isOn) TurnOn();
        // if (debugDelay > 10 && isOn) {
        //     TurnOff();
        //     debugDelay = 0;
        // }

        if (isOn && fading) {
            UpdateGraphics(albedoOnColor, emissionOnIntensity);
        }
    
        if (!isOn && fading) {   
            UpdateGraphics(albedoOffColor, emissionOffIntensity);
        }
    }

    public void UpdateGraphics(Color albedoTargetColor, float emissionTargetIntensity) {
        bool[] done = new bool[4];

        var albedoFadeStep = Time.deltaTime * abledoColorSpeedRatio * fadeSpeed;
        var col = lightMaterial.color;
        
        if (col.r < albedoTargetColor.r - albedoFadeStep)
            col.r += albedoFadeStep;
        else if (col.r > albedoTargetColor.r + albedoFadeStep)
            col.r -= albedoFadeStep;
        else {
            col.r = albedoTargetColor.r;
            done[0] = true;
        }

        if (col.g < albedoTargetColor.g - albedoFadeStep)
            col.g += albedoFadeStep;
        else if (col.g > albedoTargetColor.g + albedoFadeStep)
            col.g -= albedoFadeStep;
        else {
            col.g = albedoTargetColor.g;
            done[1] = true;
        }

        if (col.b < albedoTargetColor.b - albedoFadeStep)
            col.b += albedoFadeStep;
        else if (col.b > albedoTargetColor.b + albedoFadeStep)
            col.b -= albedoFadeStep;
        else {
            col.b = albedoTargetColor.b;
            done[2] = true;
        }

        lightMaterial.color = col;
        // Debug.Log("Color: (r:"+col.r+",g:"+col.g+",b:"+col.b+")");


        var emissionFadeStep = Time.deltaTime * emissionIntensitySpeedRatio * fadeSpeed;

        if (emissionIntensity < emissionTargetIntensity - emissionFadeStep)
            emissionIntensity += emissionFadeStep;
        else if (emissionIntensity > emissionTargetIntensity + emissionFadeStep)
            emissionIntensity -= emissionFadeStep;
        else {
            emissionIntensity = emissionTargetIntensity;
            done[3] = true;
        }

        lightMaterial.SetColor("_EmissionColor", emissionColor * emissionIntensity);
        // Debug.Log("Emission intensity: "+emissionIntensity);

        if (done[0] && done[1] && done[2] && done[3]) fading = false;
    }

    public void TurnOn() {
        isOn = true;
        fading = true;
        // Debug.Log("Turned on light!");
    }

    public void TurnOff() {
        isOn = false;
        fading = true;
        // Debug.Log("Turned off light!");
    }
}
