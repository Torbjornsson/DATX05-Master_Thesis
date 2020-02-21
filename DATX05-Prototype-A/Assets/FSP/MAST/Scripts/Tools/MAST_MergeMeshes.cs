using System.Collections.Generic;
using UnityEngine;

#if (UNITY_EDITOR)

public class MAST_MergeMeshes
{
    private List<Material> mats = new List<Material>();
    private List<string> matNames = new List<string>();
    
    private List<GameObject> excludeFromMerge = new List<GameObject>();
    
    private MeshCombine[] meshCombine;
    class MeshCombine
    {
        public List<CombineInstance> combineList;
    }
    
    // ---------------------------------------------------------------------------
    // Merge all Meshes by Material
    // ---------------------------------------------------------------------------
    public GameObject MergeMeshesByMaterial(GameObject sourceParent)
    {
        // Recursively search through child tree and add any unique materials
        // This will result in 2 lists: [Materials mats] and [string matNames]
        FindUniqueMaterials(sourceParent.transform);
        
        // Prep Mesh combine array
        meshCombine = new MeshCombine[matNames.Count];
        for (int i = 0; i < matNames.Count; i++)
        {
            meshCombine[i] = new MeshCombine();
            meshCombine[i].combineList = new List<CombineInstance>();
        }
        
        // Recursively search through child tre and add each Mesh
        // to the corresponding array, sorted by Material name
        GroupMeshesByMaterial(sourceParent.transform);
        
        // Create the target parent.  This will add it to the Hierarchy as well
        GameObject targetParent = new GameObject();
        
        // Define variable used to create target GameObject
        GameObject targetChild;
        MeshFilter targetMeshFilter;
        Renderer targetRenderer;
        
        // Loop through each list of Meshes
        for (int i = 0; i < matNames.Count; i++)
        {
            // Create child GameObject for all Meshes with this Material
            targetChild = new GameObject();
            targetChild.SetActive(false);
            targetChild.transform.SetParent(targetParent.transform);
            targetChild.name = matNames[i];
            
            // Add MeshFilter and Meshes
            targetMeshFilter = targetChild.AddComponent<MeshFilter>();
            targetMeshFilter.mesh = new Mesh();
            targetMeshFilter.sharedMesh.CombineMeshes(meshCombine[i].combineList.ToArray());
            
            // Add MeshRender and Material
            targetRenderer = targetChild.AddComponent<MeshRenderer>();
            targetRenderer.sharedMaterial = mats[i];
            
            // Enable child
            targetChild.SetActive(true);
        }
        
        // Add copy of each excluded from merge GameObject to the parent GameObject
        GameObject unmergedGameObject;
        foreach (GameObject gameObject in excludeFromMerge)
        {
            unmergedGameObject = GameObject.Instantiate(gameObject);
            unmergedGameObject.name = gameObject.name;
            unmergedGameObject.transform.SetParent(targetParent.transform);
        }
        
        // Return the new target parent
        return targetParent;
    }
    
    // ---------------------------------------------------------------------------
    // Recursively search through child tree and add any unique Materials
    // ---------------------------------------------------------------------------
    // - This only works for Meshes that have only one material
    // ---------------------------------------------------------------------------
    private void FindUniqueMaterials(Transform transform)
    {
        // If prefab is not supposed to be included in the merge, exit now
        if (!IncludeInMerge(transform.gameObject))
            return;
        
        // Attempt to get reference to GameObject Renderer
        Renderer meshRenderer = transform.gameObject.GetComponent<Renderer>();
        
        // If a Renderer was found
        if (meshRenderer != null)
        {
            meshRenderer.sharedMaterial.name = meshRenderer.sharedMaterial.name.Replace(" (Instance)", "");
            
            // If material name list doesn't already contain this name, add it
            if (!matNames.Contains(meshRenderer.sharedMaterial.name))
            {
                mats.Add(meshRenderer.sharedMaterial);
                matNames.Add(meshRenderer.sharedMaterial.name);
            }
        }
        
        // Recursively run this method for each child transform
        foreach (Transform childTransform in transform)
        {
            FindUniqueMaterials(childTransform);
        }
    }
    
    // ---------------------------------------------------------------------------
    // Recursively search through child tree and add Meshes to corresponding arrays
    // ---------------------------------------------------------------------------
    // - This only works for Meshes that have only one material
    // ---------------------------------------------------------------------------
    private void GroupMeshesByMaterial(Transform transform)
    {
        // If GameObject is not supposed to be included in the merge
        if (!IncludeInMerge(transform.gameObject))
        {
            // Add the GameObject to the exclude from merge list
            excludeFromMerge.Add(transform.gameObject);
            
            // Exit before merging
            return;
        }
        
        // Attempt to get reference to GameObject Renderer
        Renderer meshRenderer = transform.gameObject.GetComponent<Renderer>();
        
        // If a Renderer was found
        if (meshRenderer != null)
        {
            meshRenderer.sharedMaterial.name = meshRenderer.sharedMaterial.name.Replace(" (Instance)", "");
            
            // Find array index corresponding to this Material name
            int matIndex = matNames.IndexOf(meshRenderer.sharedMaterial.name);
            
            // Get MeshFilter of current GameObject 
            MeshFilter tempMeshFilter = transform.gameObject.GetComponent<MeshFilter>();
            
            // Create a CombineInstance using this Mesh
            CombineInstance tempCombine = new CombineInstance();
            tempCombine.mesh = tempMeshFilter.sharedMesh;
            tempCombine.transform = tempMeshFilter.transform.localToWorldMatrix;
            
            // Add CombineInstance to List
            meshCombine[matIndex].combineList.Add(tempCombine);
        }
        
        // Recursively run this method for each child transform
        foreach (Transform childTransform in transform)
        {
            GroupMeshesByMaterial(childTransform);
        }
    }
    
    private bool IncludeInMerge(GameObject go)
    {
        // If prefab is not supposed to be included in the merge, don't include its material name
        MAST_Prefab_Component prefabComponent = go.GetComponent<MAST_Prefab_Component>();
        if (prefabComponent != null)
            return prefabComponent.includeInMerge;
        
        // If no MAST prefab component was attached, include it in the merge
        return true;
    }
}

#endif