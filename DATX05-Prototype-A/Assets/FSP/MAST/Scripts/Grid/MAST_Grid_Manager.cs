using System;
using UnityEngine;
using UnityEditor;

#if (UNITY_EDITOR)

[Serializable]
public static class MAST_Grid_Manager
{
    // ---------------------------------------------------------------------------
    #region Variable Declaration
    // ---------------------------------------------------------------------------
    
    // Grid Appearance
    [SerializeField] public static bool gridExists = false;
    
    // Grid in Scene
    [SerializeField] private static GameObject gridGameObject;
    [SerializeField] private static Material gridMaterial;
    [SerializeField] private static MAST_Grid_Component gridComponent;
    [SerializeField] private static GameObject gridParent; // hidden in inspector with child grid left visible so it still draws gizmolines
    
    #endregion
    // ---------------------------------------------------------------------------
    
    // ---------------------------------------------------------------------------
    // Initialize
    // ---------------------------------------------------------------------------
    public static void Initialize()
    {
        
    }
    
    // ---------------------------------------------------------------------------
    #region Grid Location
    // ---------------------------------------------------------------------------
    public static void MoveGridUp()
    {
        if (gridExists)
        {
            // Move Grid Up
            MAST_Settings.gui.grid.gridHeight += 1;
            MoveGridToNewHeight();
        }
    }
    
    public static void MoveGridDown()
    {
        if (gridExists)
        {
            // Move Grid Up
            MAST_Settings.gui.grid.gridHeight -= 1;
            MoveGridToNewHeight();
        }
    }
    
    private static void MoveGridToNewHeight()
    {
        // Calculate new grid height
        float gridY = MAST_Settings.gui.grid.gridHeight * MAST_Settings.gui.grid.yUnitSize + MAST_Const.grid.yOffsetToAvoidTearing;
        gridGameObject.transform.position =
            new Vector3(gridGameObject.transform.position.x, gridY, gridGameObject.transform.position.z);
    }
    #endregion
    // ---------------------------------------------------------------------------
    
    // ---------------------------------------------------------------------------
    #region Create/Destroy Grid
    // ---------------------------------------------------------------------------
    
    // Return if grid reference exists
    public static bool DoesGridExist()
    {
        return gridExists;
    }
    
    // Change grid visibility
    public static void ChangeGridVisibility()
    {
        if (gridExists)
            CreateGrid();
        else
            DestroyGrid();
    }
    
    // Destroy any existing grid
    public static void DestroyGrid()
    {
        // Find existing grid(s) and delete them - even if disabled
        foreach (GameObject go in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
        {
            if (go.name == MAST_Const.grid.defaultParentName)
            {
                GameObject.DestroyImmediate(go);
            }
        }
        
        // Remove locked layer
        Tools.lockedLayers &= ~(1 << MAST_Const.grid.gridLayer);
        
        gridExists = false;
    }
    
    // Create grid
    public static void CreateGrid()
    {
        CreateLinkToGrid();
        
        // Lock the layer the grid is on
        Tools.lockedLayers = 1 << MAST_Const.grid.gridLayer;
        
        gridExists = true;
    }
    
    // Create link to any grid that exists, or create a new grid
    private static void CreateLinkToGrid()
    {
        gridGameObject = GameObject.Find(MAST_Const.grid.defaultParentName);
        
        // If grid exists already, get a reference to its component
        if (gridGameObject != null)
        {
            gridComponent = gridGameObject.GetComponent<MAST_Grid_Component>();
            
            // If the gridcomponent isn't found, then this grid gameobject is dead
            if (gridComponent == null)
            {
                // Destroy existing grid and create a new one
                DestroyGrid();
                CreateNewGrid();
            }
            
            UpdateGridSettings();
        }
        
        // If no grid exists, create a new grid
        else
        {
            CreateNewGrid();
        }
    }
    
    // ---------------------------------------------------------------------------
    // Create a New Grid in the Hierarchy from the Grid Prefab
    // ---------------------------------------------------------------------------
    static void CreateNewGrid()
    {
        // Create new Grid GameObject
        gridGameObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
        gridGameObject.transform.position = new Vector3(0f, 0f, 0f);
        gridGameObject.name = MAST_Const.grid.defaultName;
        gridGameObject.layer = 4;
        
        // Configure Grid GameObject MeshRenderer
        MeshRenderer gridMeshRenderer = gridGameObject.GetComponent<MeshRenderer>();
        gridMeshRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
        gridMeshRenderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
        gridMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        gridMeshRenderer.receiveShadows = false;
        
        // Configure Grid GameObject Material
        if (gridMaterial == null)
        {
            gridMaterial = MAST_Asset_Loader.GetGridMaterial();
        }
        gridMaterial.SetColor("_Color", MAST_Settings.gui.grid.fillColor);
        gridMeshRenderer.material = gridMaterial;
        
        //Create parent object, used to make the grid unselectable while it still displays it's gizmos
        gridParent = new GameObject(MAST_Const.grid.defaultParentName);
        gridGameObject.transform.SetParent(gridParent.transform);
        
        // Add MAST_Grid_Component script to grid and pass grid preferences to it
        gridComponent = gridGameObject.AddComponent<MAST_Grid_Component>();
        UpdateGridSettings();
        
        // Return the grid to its last saved height
        MoveGridToNewHeight();
        
        // Hide the grid in the hierarchy
        gridParent.hideFlags = HideFlags.HideInHierarchy;
    }
    #endregion
    // ---------------------------------------------------------------------------
    
    // ---------------------------------------------------------------------------
    #region Grid Settings
    // ---------------------------------------------------------------------------
    public static void UpdateGridSettings()
    {
        if (gridGameObject != null)
        {
            // Update grid component
            gridComponent.UpdateSettings();
            
            // Update grid color
            gridMaterial.SetColor("_Color", MAST_Settings.gui.grid.fillColor);
            MeshRenderer gridMeshRenderer = gridGameObject.GetComponent<MeshRenderer>();
            gridMeshRenderer.material = gridMaterial;
        }
    }
    #endregion
    // ---------------------------------------------------------------------------
}

#endif