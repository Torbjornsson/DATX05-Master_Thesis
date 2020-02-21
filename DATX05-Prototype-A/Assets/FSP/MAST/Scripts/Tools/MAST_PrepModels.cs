using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if (UNITY_EDITOR)

public class MAST_PrepModels
{
    // Selected folder
    private string pathSelected;
    private string pathMeshes;
    private string pathMaterials;
    private string pathPrefabs;
    
    // Original model files
    private GameObject[] sourceModels;
    
    // ---------------------------------------------------------------------------
    #region Create Prefabs from Models
    // ---------------------------------------------------------------------------
    
    // Create Prefabs from Models
    public void CreatePrefabsFromModels()
    {
        // ------------------------------------------------------------------------
        // Get a GameObject array containing all models in the selected folder
        // ------------------------------------------------------------------------
        sourceModels = null;
        
        // Get models in the selected folder
        GetModelsInFolder();
        
        // Show error and exit if no models were found in the selected project folder
        if (sourceModels == null)
        {
            Debug.Log("No models found in the selected project folder!");
            return;
        }
        
        // ------------------------------------------------------------------------
        // Create Model, Material, and Prefab folders
        // ------------------------------------------------------------------------
        
        pathMeshes = pathSelected + "/Meshes";
        pathPrefabs = pathSelected + "/Prefabs";
        pathMaterials = pathSelected + "/Materials";
        
        // Create a new "/Models" folder, one it doesn't exist
        if (!AssetDatabase.IsValidFolder(pathMeshes))
        {
            AssetDatabase.CreateFolder(pathSelected, "Meshes");
            AssetDatabase.SaveAssets();
        }
        
        // Create a new "/Materials" folder, one it doesn't exist
        if (!AssetDatabase.IsValidFolder(pathMaterials))
        {
            AssetDatabase.CreateFolder(pathSelected, "Materials");
            AssetDatabase.SaveAssets();
        }
        
        // Create a new "/Prefabs" folder, if one doesn't exist
        if (!AssetDatabase.IsValidFolder(pathPrefabs))
        {
            AssetDatabase.CreateFolder(pathSelected, "Prefabs");
            AssetDatabase.SaveAssets();
        }
        
        // ------------------------------------------------------------------------
        // Get a unique list of all Materials used
        // ------------------------------------------------------------------------
        MeshRenderer meshRenderer;
        
        Material[] tempMats;
        
        List<Material> mats = new List<Material>();
        List<string> matNames = new List<string>();
        
        // Loop through each model
        for (int i = 0; i < sourceModels.Length; i++)
        {
            
            // Loop through each child in model
            foreach (Transform childTransform in sourceModels[i].transform)
            {
                // Get MeshRenderer
                meshRenderer = childTransform.gameObject.GetComponent<MeshRenderer>();
                
                // If MeshRenderer is found, get the Materials used in it
                if (meshRenderer)
                {
                    // Get Materials (array) in this MeshRenderer
                    tempMats = meshRenderer.sharedMaterials;
                    
                    // Loop through each Material and add unique Materials to the Material list
                    for (int t = 0; t < tempMats.Length; t++)
                    {
                        // Remove " (Instance)" from Material name
                        tempMats[t].name = tempMats[t].name.Replace(" (Instance)", "");
                        
                        // If Material name list doesn't already contain this name, add it
                        if (!matNames.Contains(tempMats[t].name))
                        {
                            mats.Add(tempMats[t]);
                            matNames.Add(tempMats[t].name);
                        }
                    }
                }
            }
        }
        
        // ------------------------------------------------------------------------
        // Save Material files for each Material found
        // ------------------------------------------------------------------------
        Material savedMat;

        // Loop through each Material and save to "/Materials" folder
        for (int i = 0; i < matNames.Count; i++)
        {
            // Load Material if it already exists
            savedMat = (Material)AssetDatabase.LoadAssetAtPath(
                pathMaterials + "/" + matNames[i] + ".mat",typeof(Material));
            
            // Create the Material if it doesn't exist
            if (savedMat == null)
            {
                // Create Material in "/Materials" folder
                AssetDatabase.CreateAsset(UnityEngine.Object.Instantiate(mats[i]),
                    pathMaterials + "/" + matNames[i] + ".mat");
                
                // Save Material and refesh AssetsDatabase, so it can be referenced again
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                
                // Get reference to saved Material
                mats[i] = (Material)AssetDatabase.LoadAssetAtPath(
                    pathMaterials + "/" + matNames[i] + ".mat",
                    typeof(Material));
                
                //Debug.Log("Saving Material: " + matNames[i]);
            }
            
            // Get reference to saved Material
            else
            {
                mats[i] = savedMat;
            }
        }
        
        // ------------------------------------------------------------------------
        // Split SubMeshes into separate Meshes and create Prefabs from them
        // ------------------------------------------------------------------------
        int matIndex;
        
        GameObject newGameObject;
        Mesh newMesh;
        
        // Loop through each model
        for (int i = 0; i < sourceModels.Length; i++)
        {
            // Split SubMeshes into child GameObjects with separate Meshes
            newGameObject = MAST_SubMeshSplitter.GetGameObjectWithSplitSubmeshes(sourceModels[i]);
            
            // Attach MAST Prefab script to the GameObject
            newGameObject.AddComponent<MAST_Prefab_Component>();
            
            // ------------------------------------------------------------------------
            // Save each SubMesh then link them with Materials to the new GameObject
            // ------------------------------------------------------------------------
            
            // Loop through each child transform and link the MeshRenderers to the saved Material files
            foreach (Transform childTransform in newGameObject.transform)
            {
                // Get MeshRenderer to gain access to the Material
                meshRenderer = childTransform.gameObject.GetComponent<MeshRenderer>();
                
                // Remove " (Instance)" from Material name
                meshRenderer.sharedMaterial.name = meshRenderer.sharedMaterial.name.Replace(" (Instance)", "");
                
                // Find array index corresponding to this Material name
                matIndex = matNames.IndexOf(meshRenderer.sharedMaterial.name);
                
                // Assign saved Material file to this MeshRenderer
                meshRenderer.sharedMaterial = mats[matIndex];
                
                newMesh = childTransform.gameObject.GetComponent<MeshFilter>().sharedMesh;
                
                // Add a MeshCollider to each model
                childTransform.gameObject.AddComponent<MeshCollider>();
                
                // Save Mesh and refesh AssetsDatabase so it can be referenced again
                AssetDatabase.CreateAsset(newMesh, pathMeshes + "/" + newMesh.name + ".asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                
                // Link active GameObject MeshRenderer to saved Mesh
                childTransform.gameObject.GetComponent<MeshFilter>().sharedMesh =
                    (Mesh)AssetDatabase.LoadAssetAtPath(
                    pathMeshes + "/" + newMesh.name + ".asset",
                    typeof(Mesh));
            }
            
            // ------------------------------------------------------------------------
            // Create a Prefab for each saved model
            // ------------------------------------------------------------------------
            
            // Make new GameObject name the same as the source model
            newGameObject.name = sourceModels[i].name;
            
            // Save a prefab to the model
            PrefabUtility.SaveAsPrefabAsset(newGameObject,
                pathPrefabs + "/" + newGameObject.name + ".prefab");
            
            // Delete Prefab from Hierarchy
            GameObject.DestroyImmediate(newGameObject);
        }
    }
    
    // Get an array of all models in the selected folder
    private void GetModelsInFolder()
    {
        // Show choose folder dialog
        pathSelected = EditorUtility.OpenFolderPanel("Choose the Folder that Contains your Models to Convert",
            Application.dataPath, "");
        
        // If a folder was chosen (Cancel was not clicked)
        if (pathSelected != "")
        {
            // Convert to project local path "Assets/..."
            pathSelected = pathSelected.Replace(Application.dataPath, "Assets");
            
            string[] GUIDOfAllGameObjectsInFolder =
                AssetDatabase.FindAssets("t:gameobject", new[] { pathSelected });
            
            // If any models were found
            if (GUIDOfAllGameObjectsInFolder.Length > 0)
            {
                // Create array to store the gameObjects
                sourceModels =
                    new GameObject[GUIDOfAllGameObjectsInFolder.Length];
                
                // Create the string outside the loop so it is not recreated every loop
                string pathToPrefab;
                
                for (int i = GUIDOfAllGameObjectsInFolder.Length - 1; i >= 0; i--)
                {
                    // Convert GUID at current index to path string
                    pathToPrefab = AssetDatabase.GUIDToAssetPath(GUIDOfAllGameObjectsInFolder[i]);
                    
                    // Get gameObject at path
                    sourceModels[i] = (GameObject)AssetDatabase.LoadAssetAtPath(pathToPrefab, typeof(GameObject));
                }
            }
        }
    }
    #endregion
    // ---------------------------------------------------------------------------
    
    // ---------------------------------------------------------------------------
    // Add MAST Scripts to Prefabs in Folder
    // ---------------------------------------------------------------------------
    public void AddMASTScriptsToPrefabsInFolder()
    {
        // Show choose folder dialog
        string chosenPath = EditorUtility.OpenFolderPanel("Choose the Folder that Contains your Prefabs",
            MAST_Interface_Data_Manager.state.lastPrefabPath, "");
        
        // If a folder was chosen "Cancel was not clicked"
        if (chosenPath != "")
        {
            // Save the path the user chose
            MAST_Interface_Data_Manager.state.lastPrefabPath = chosenPath;
            MAST_Interface_Data_Manager.Save_Changes_To_Disk();
            
            // Convert to project local path "Assets/..."
            string assetPath = chosenPath.Replace(Application.dataPath, "Assets");
            
            // Loop through each Prefab in folder
            foreach (GameObject prefab in MAST_Asset_Loader.GetPrefabsInFolder(assetPath))
            {
                // Add MAST Prefab script if not already added
                if (!prefab.GetComponent<MAST_Prefab_Component>())
                    prefab.AddComponent<MAST_Prefab_Component>();
            }
        }
    }
    
    // ---------------------------------------------------------------------------
    // Remove MAST Scripts from GameObjects
    // ---------------------------------------------------------------------------
    public void RemoveMASTScriptsFromSelectedGameObject()
    {
        // Loop through all top-level children of targetParent
        foreach (MAST_Prefab_Component prefabComponent
            in Selection.activeGameObject.transform.GetComponentsInChildren<MAST_Prefab_Component>())
        {
            // Remove the SMACK_Prefab_Component script
            GameObject.DestroyImmediate(prefabComponent);
        }
    }
}

#endif