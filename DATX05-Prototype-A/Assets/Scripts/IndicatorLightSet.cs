using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorLightSet : MonoBehaviour
{
    public IndicatorLight[] indicatorLights;

    // Start is called before the first frame update
    void Start()
    {
        if (indicatorLights.Length != 6)
            Debug.LogError("Indicator Light Set: There was not exactly 6 indicator lights!");
        foreach(IndicatorLight l in indicatorLights) {
            if (!l) {
                Debug.LogError("Indicator Light Set: There was not exactly 6 indicator lights!");
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleLight(int lightNumber, bool active) {
        var light = indicatorLights[lightNumber];
        if (active) {
            light.TurnOn();
            GameMaster.instance.tutorialMaster.ScanningLightOn();
        }
        if (!active) light.TurnOff();
    }

    public void ToggleAllLights(bool active) {
        foreach (IndicatorLight l in indicatorLights) {
            if (active) l.TurnOn();
            if (!active) l.TurnOff();
        }
        GameMaster.instance.tutorialMaster.ScanningLightOff();
    }
}
