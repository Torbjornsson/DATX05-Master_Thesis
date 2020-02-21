using UnityEngine;
using UnityEditor;

#if (UNITY_EDITOR)

public class MAST_Tools_Window : EditorWindow
{  
    [SerializeField] private MAST_MergeMeshes MergeMeshesClass;
    private MAST_MergeMeshes MergeMeshes
    {
        get
        {
            // Initialize MergeMeshes Class if needed and return MergeMeshesClass
            if(MergeMeshesClass == null)
                MergeMeshesClass = new MAST_MergeMeshes();
            
            return MergeMeshesClass;
        }
    }
    
    [SerializeField] private MAST_PrepModels PrepModelsClass;
    private MAST_PrepModels PrepModels
    {
        get
        {
            // Initialize Hotkeys Class if needed and return HotKeysClass
            if(PrepModelsClass == null)
                PrepModelsClass = new MAST_PrepModels();
            
            return PrepModelsClass;
        }
    }
    
    [SerializeField] private Vector2 scrollPos;
    
    void OnFocus() {}
    
    void OnDestroy() {}
    
    // ---------------------------------------------------------------------------
    #region Preferences Interface
    // ---------------------------------------------------------------------------
    void OnGUI()
    {
        // Verical scroll view for palette items
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        
        // ----------------------------------
        // Create Prefabs from Models
        // ----------------------------------
        EditorGUILayout.LabelField("Strip materials and meshes from all models in the selected folder and generate prefabs from them to be used in MAST (A MAST script will also be added to each prefab).", EditorStyles.wordWrappedLabel);
        
        if (GUILayout.Button(new GUIContent("Create Prefabs from Models",
            "Create Prefabs from all models in the selected folder.")))
        {
            PrepModels.CreatePrefabsFromModels();
        }
        
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        
        // ----------------------------------
        // Add MAST Script to Prefabs
        // ----------------------------------
        EditorGUILayout.LabelField("If you generated your own prefabs, this will add a MAST script to each prefab.  The script is used to describe the type of object to the MAST editor.", EditorStyles.wordWrappedLabel);
        
        if (GUILayout.Button(new GUIContent("Add MAST Script to Prefabs",
            "Create Prefabs from all models in the selected folder.")))
        {
            PrepModels.AddMASTScriptsToPrefabsInFolder();
        }
        
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        
        // ----------------------------------
        // Remove MAST Components Button
        // ----------------------------------
        EditorGUILayout.LabelField("Remove all MAST Components that were attached to the children of the selected GameObject during placement.", EditorStyles.wordWrappedLabel);
        
        if (GUILayout.Button(new GUIContent("Remove MAST Components",
            "Remove any MAST Component code attached to gameobjects during placement")))
        {
            if (EditorUtility.DisplayDialog("Are you sure?",
                "This will remove all MAST components attached to '" + Selection.activeGameObject.name + "'",
                "Remove MAST Components", "Cancel"))
            {
                PrepModels.RemoveMASTScriptsFromSelectedGameObject();
            }
        }
        
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        
        // ----------------------------------
        // Merge Meshes by Material Button
        // ----------------------------------
        EditorGUILayout.LabelField("Merge all meshes by material in the selected GameObject, and place them in a new GameObject.", EditorStyles.wordWrappedLabel);
        
        if (GUILayout.Button(new GUIContent("Merge Meshes by Material",
            "Merge all meshes by material name, resulting in one mesh for each material")))
        {
            if (EditorUtility.DisplayDialog("Are you sure?",
                "This will merge all meshes by material in '" + Selection.activeGameObject.name +
                "' and save them to a new GameObject.  The original GameObject will not be affected.",
                "Merge Meshes by Material", "Cancel"))
            {
                
                GameObject targetParent = MergeMeshes.MergeMeshesByMaterial(Selection.activeGameObject);
                targetParent.name = Selection.activeGameObject.name + "_Merged";
            }
        }
        
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        
        EditorGUILayout.EndScrollView();
    }
    #endregion
    // ---------------------------------------------------------------------------
}

#endif