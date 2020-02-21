using UnityEngine;

#if (UNITY_EDITOR)

[ExecuteInEditMode]
public class MAST_Grid_Component : MonoBehaviour
{
    // ---------------------------------------------------------------------------
    // Variable Declaration
    // ---------------------------------------------------------------------------
    
    // Space between grid lines
    private float xzUnitSize;
    
    // Grid extents
	private int xMin, xMax, zMin, zMax;
	
	// Grid center
	public Vector3 gridOffset = Vector3.zero;
	
	// Grid line colors
	public int gizmoMajorLines;
	public Color gizmoMinorLineColor;
    public Color gizmoMajorLineColor;
    
    void Start() { }
    
    void Update() { }
    
    // ---------------------------------------------------------------------------
	// Draw Grid
    // ---------------------------------------------------------------------------
	void OnDrawGizmos ()
	{
        // Set gizmo to use this object's position and rotation
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        
        // Draw horizontal lines
        for (int x = xMin; x < xMax+1; x++)
		{
			// Find major lines
			Gizmos.color = (x % gizmoMajorLines == 0 ? gizmoMajorLineColor : gizmoMinorLineColor);
			if (x == 0) Gizmos.color = gizmoMajorLineColor;
            
			Vector3 pos1 = new Vector3(x, 0, zMin) * xzUnitSize;
			Vector3 pos2 = new Vector3(x, 0, zMax) * xzUnitSize;
            
			Gizmos.DrawLine ((gridOffset + pos1), (gridOffset + pos2));
		}
        
		// Draw vertical lines
		for (int z = zMin; z < zMax+1; z++)
		{
			// Find major lines
			Gizmos.color = (z % gizmoMajorLines == 0 ? gizmoMajorLineColor : gizmoMinorLineColor);
			if (z == 0) Gizmos.color = gizmoMajorLineColor;
            
			Vector3 pos1 = new Vector3(xMin, 0, z) * xzUnitSize;
            Vector3 pos2 = new Vector3(xMax, 0, z) * xzUnitSize;
            
			Gizmos.DrawLine ((gridOffset + pos1), (gridOffset + pos2));
		}
    }
    
    // ---------------------------------------------------------------------------
    // Update local grid settings when changed globally
    // ---------------------------------------------------------------------------
    public void UpdateSettings()
    {
        // Update grid unit size
        xzUnitSize = Mathf.Max(0, Mathf.Abs(MAST_Settings.gui.grid.xzUnitSize));
        
        // Update grid dimensions
        xMin = 0 - MAST_Settings.gui.grid.size;
        xMax = MAST_Settings.gui.grid.size;
        zMin = 0 - MAST_Settings.gui.grid.size;
        zMax = MAST_Settings.gui.grid.size;
        
        // Update grid color
        gizmoMinorLineColor = MAST_Settings.gui.grid.minorLineColor;
        gizmoMajorLineColor = MAST_Settings.gui.grid.majorLineColor;
        gizmoMajorLines = Mathf.Max(0, Mathf.Abs(MAST_Settings.gui.grid.majorLineStep));
        
        // Scale plane to match new grid size
        transform.localScale = new Vector3(
            xzUnitSize * xMax * 2f / 10f,
            1f,
            xzUnitSize * zMax * 2f / 10f);
    }
}

#endif