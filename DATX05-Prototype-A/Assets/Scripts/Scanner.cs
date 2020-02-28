using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scanner : MonoBehaviour
{
    public GameObject scannerLight;
    public Renderer[] scannerBarRenderers;
    public Renderer scannerBack;
    public bool useParticles = true;
    public bool useSpotlight = true;
    [Space]
    public int totalPasses = 1;
    public float scanningIntroSpeed = 1f;
    public float scanningMoveSpeed = 1f;
    public float delayBetweenPasses = 0.2f;
    [Space]
    public float scannerBackIdleGlowSpeed = 0.1f;
    public float scannerBackFlashGlowSpeed = 1.5f;
    public float scannerBackIdleMaxIntensity = 0.5f;
    public float scannerBackFlashMaxIntensity = 2;

    private ParticleSystem scannerParticles;
    private Light scannerPointLight;
    private float scannerLightIntensity;
    private Material[] scannerBarMaterials;
    private float scannerBarAlpha = 0;

    private Material scannerBackMaterial;
    private Color scannerBackEmissionColor;
    private float scannerBackIntensity = 0;
    private bool scannerBackFlash = false;

    private Vector3 lightStartPosition;
    private Vector3 lightEndPosition;
    private Vector3 lightTargetPosition;
    private Vector3 lightPreviousPosition;
    private int passesCount = 0;
    private bool backwards = false;
    private bool intro = false;
    private bool outro = false;
    private bool scanning = false;
    private float lerpTime;
    private float delay;

    // Start is called before the first frame update
    void Start()
    {
        if (!scannerLight)
            Debug.LogError("Scanner: Scanner light was not found!");
        if (!scannerBack)
            Debug.LogError("Scanner: Scanner back was not found!");

        lightStartPosition = scannerLight.transform.position;
        lightEndPosition = scannerLight.transform.position;
        lightEndPosition.x *= -1;

        scannerParticles = scannerLight.GetComponentInChildren<ParticleSystem>();
        scannerParticles.Stop();

        scannerPointLight = scannerLight.GetComponentInChildren<Light>();
        scannerLightIntensity = scannerPointLight.intensity;
        scannerPointLight.intensity = 0;
        scannerPointLight.enabled = useSpotlight;

        scannerBarMaterials = new Material[scannerBarRenderers.Length];
        for (int i = 0; i < scannerBarRenderers.Length; i++) {
            scannerBarMaterials[i] = scannerBarRenderers[i].material;
        }
        // scannerBarMaterial = scannerLight.GetComponentInChildren<MeshRenderer>().material;
        scannerBarAlpha = 0;
        UpdateScannerBarAlpha();

        scannerBackMaterial = scannerBack.material;
        scannerBackEmissionColor = scannerBackMaterial.GetColor("_EmissionColor");
    }

    // Update is called once per frame
    void Update()
    {

        if (!scannerBackFlash) {
            scannerBackIntensity += scannerBackIdleGlowSpeed * Time.deltaTime;
            if ((scannerBackIntensity > scannerBackIdleMaxIntensity && scannerBackIdleGlowSpeed > 0)
                    || (scannerBackIntensity < 0 && scannerBackIdleGlowSpeed < 0))
                scannerBackIdleGlowSpeed *= -1;
            scannerBackMaterial.SetColor("_EmissionColor", scannerBackIntensity * scannerBackEmissionColor);
            // Debug.Log("IDLE - Scanner back intensity: "+scannerBackIntensity);

        } else {
            scannerBackIntensity += scannerBackFlashGlowSpeed * Time.deltaTime;
            if (scannerBackIntensity > scannerBackFlashMaxIntensity && scannerBackFlashGlowSpeed > 0)
                scannerBackFlashGlowSpeed *= -1;
            if (scannerBackIntensity > 0)
                scannerBackMaterial.SetColor("_EmissionColor", scannerBackIntensity * scannerBackEmissionColor);
            else
                scannerBackFlash = false;
            // Debug.Log("FLASH - Scanner back intensity: "+scannerBackIntensity);
        }


        if (!scanning) return;

        if (intro) {
            if (scannerBarAlpha < 1)
                scannerBarAlpha += scanningIntroSpeed * Time.deltaTime;
            if (scannerBarAlpha > 1)
                scannerBarAlpha = 1;
            UpdateScannerBarAlpha();

            if (useSpotlight) {
                if (scannerPointLight.intensity < 1)
                    scannerPointLight.intensity += scanningIntroSpeed * Time.deltaTime;
                if (scannerPointLight.intensity > 1)
                    scannerPointLight.intensity = 1;
            }
            
            if (scannerBarAlpha >= 1 && (!useSpotlight || scannerPointLight.intensity >= 1)) {
                intro = false;
                lightPreviousPosition = scannerLight.transform.position;
                lightTargetPosition = lightEndPosition;
                lerpTime = 0;
                delay = delayBetweenPasses;
            }
            return;
        }

        if (outro) {
            if (scannerBarAlpha > 0)
                scannerBarAlpha -= scanningIntroSpeed * Time.deltaTime;
            if (scannerBarAlpha < 0)
                scannerBarAlpha = 0;
            UpdateScannerBarAlpha();

            if (useSpotlight) {
                if (scannerPointLight.intensity > 0)
                    scannerPointLight.intensity -= scanningIntroSpeed * Time.deltaTime;
                if (scannerPointLight.intensity < 0)
                    scannerPointLight.intensity = 0;
            }
            
            if (scannerBarAlpha <= 0 && (!useSpotlight || scannerPointLight.intensity <= 0)) {
                outro = false;
                scanning = false;
            }
            return;
        }

        if (passesCount < totalPasses) {

            // var vectorToTravel = (lightTargetPosition - scannerLight.transform.position);
            // if (vectorToTravel.magnitude > Mathf.Epsilon) {
            //     scannerLight.transform.position += vectorToTravel.normalized * Time.deltaTime;
            // }

            if (delay < delayBetweenPasses) {
                delay += Time.deltaTime;
                return;
            }

            lerpTime += Time.deltaTime * scanningMoveSpeed;
            scannerLight.transform.position = Vector3.Lerp(lightPreviousPosition, lightTargetPosition, lerpTime);

            if (lerpTime >= 1) {
                scannerLight.transform.position = lightTargetPosition;
                var temp = lightPreviousPosition;
                lightPreviousPosition = lightTargetPosition;
                lightTargetPosition = temp;
                lerpTime = 0;
                passesCount++;
                delay = 0;
            }

            if (passesCount >= totalPasses) {
                outro = true;
                if (useParticles) scannerParticles.Stop();
            }
        }
    }

    public void StartScanner() {
        passesCount = 0;
        backwards = false;
        intro = true;
        outro = false;
        scanning = true;
        if (useParticles) scannerParticles.Play();
        if (useSpotlight) scannerPointLight.intensity = 0;
        scannerLight.transform.position = lightStartPosition;
        scannerBarAlpha = 0;
        UpdateScannerBarAlpha();
        scannerBackFlashGlowSpeed = Mathf.Abs(scannerBackFlashGlowSpeed);
        scannerBackFlash = true;
    }

    private void UpdateScannerBarAlpha() {
        // var col = scannerBarMaterial.color;
        // col.a = scannerBarAlpha;
        // scannerBarMaterial.color = col;
        foreach (Material m in scannerBarMaterials) {
            var col = m.color;
            col.a = scannerBarAlpha;
            m.color = col;
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag.Equals("PuzzleCube") && !scanning) {
            Debug.Log("Found puzzle cube!");
            StartScanner();
        }
    }
}
