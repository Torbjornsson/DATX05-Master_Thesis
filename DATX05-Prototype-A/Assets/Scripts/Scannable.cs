﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scannable : MonoBehaviour
{
    public enum CubeFace {
        Front, Back, Left, Right, Up, Down
    }

    public struct ScanningInfo {
        CubeFace face;
        int times;
        public ScanningInfo(CubeFace f, int t) {
            face = f;
            times = t;
        }
    }

    public IndicatorLightSet indicatorLights;

    [Space]
    public Collider ScannerTarget_Front;
    public Collider ScannerTarget_Back;
    public Collider ScannerTarget_Left;
    public Collider ScannerTarget_Right;
    public Collider ScannerTarget_Up;
    public Collider ScannerTarget_Down;

    [Space]
    public CubeFace[] scanningOrder;
    public int[] scanningCountPerFace;

    private Dictionary<Collider, CubeFace> colliderToFace;
    private ScanningInfo[] scanningOrderAndCount;

    private int scanningOrderProgress = 0;
    private int scanningCountPerFaceProgress = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (!indicatorLights)
            Debug.LogError("Scannable: Indicator Light Set was not found!");

        if (!ScannerTarget_Front || !ScannerTarget_Back || !ScannerTarget_Left
                || !ScannerTarget_Right || !ScannerTarget_Up || !ScannerTarget_Down)
            Debug.LogError("Scannable: Not all six Scanner Targets were found!");

        colliderToFace = new Dictionary<Collider, CubeFace>();
        colliderToFace.Add(ScannerTarget_Front, CubeFace.Front);
        colliderToFace.Add(ScannerTarget_Back, CubeFace.Back);
        colliderToFace.Add(ScannerTarget_Left, CubeFace.Left);
        colliderToFace.Add(ScannerTarget_Right, CubeFace.Right);
        colliderToFace.Add(ScannerTarget_Up, CubeFace.Up);
        colliderToFace.Add(ScannerTarget_Down, CubeFace.Down);

        var cubeFaces = 6;
        if (scanningOrder.Length != cubeFaces || scanningCountPerFace.Length != cubeFaces)
            Debug.LogError("Scannable: Scanning order or Scanning count per face, was not exactly 6");

        scanningOrderAndCount = new ScanningInfo[cubeFaces];
        for(int i = 0; i < cubeFaces; i++) {
            scanningOrderAndCount[i] = new ScanningInfo(scanningOrder[i], scanningCountPerFace[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ScannedTarget(Collider target) {
        Debug.Log("Just scanned target: "+target.gameObject.name);
    }
}
