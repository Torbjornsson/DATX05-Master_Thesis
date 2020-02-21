﻿using System;
using UnityEngine;

#if (UNITY_EDITOR)

[Serializable]
public class MAST_GUI_ScriptableObject : ScriptableObject
{
    // Palette background enum
    public enum PaleteBGColor { Dark = 0, Gray = 1 , Light = 2 }
    
    // Toolbar position enum
    public enum ToolbarPos { Left, Right } // Later add top and bottom for horizontal palette
    
    [SerializeField] public Grid grid;
    [Serializable] public class Grid
    {
        [SerializeField] public float xzUnitSize = 1f;
        [SerializeField] public float yUnitSize = 1f;
        
        [SerializeField] public int size = 50;
        [SerializeField] public int majorLineStep = 10;
        
        [SerializeField] public int gridHeight = 0;
        
        [SerializeField] public Color fillColor = new Color32(6, 44, 58, 26);
        [SerializeField] public Color minorLineColor = new Color32(166, 209, 224, 89);
        [SerializeField] public Color majorLineColor = new Color32(225, 247, 255, 115);
    }
    
    [SerializeField] public Palette palette;
    [Serializable] public class Palette
    {
        [SerializeField] public PaleteBGColor bgColor = PaleteBGColor.Dark;
        [SerializeField] public float snapshotCameraPitch = 225f;
        [SerializeField] public float snapshotCameraYaw = 30f;
    }
    
    [SerializeField] public Toolbar toolbar;
    [Serializable] public class Toolbar
    {
        [SerializeField] public int selectedDrawToolIndex = -1;
        [SerializeField] public ToolbarPos position = ToolbarPos.Left;
        [SerializeField] public float scale = 1f;
    }
    
}

#endif