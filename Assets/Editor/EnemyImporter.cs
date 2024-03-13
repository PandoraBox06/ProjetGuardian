using UnityEditor;
using UnityEngine;


public class EnemyImporter : EditorWindow
{
    private GameObject prefabWithSkinnedMesh;
    private GameObject prefabWithNewSkinnedMesh;
    
    [MenuItem("Tools/Enemy Import")]
    public static void ShowWindow()
    {
        var window = GetWindow<EnemyImporter>();
        window.titleContent = new GUIContent("Enemy Importer");
        window.Show();
    }
    
    private void OnGUI()
    {
        EditorGUILayout.LabelField("Prefab with Skinned Mesh", EditorStyles.boldLabel);
        prefabWithSkinnedMesh = (GameObject)EditorGUILayout.ObjectField(prefabWithSkinnedMesh, typeof(GameObject), false);
        EditorGUILayout.LabelField("Prefab with New Skinned Mesh", EditorStyles.boldLabel);
        prefabWithNewSkinnedMesh = (GameObject)EditorGUILayout.ObjectField(prefabWithNewSkinnedMesh, typeof(GameObject), false);
        
        if (GUILayout.Button("Replace Skinned Mesh"))
        {
            ReplaceSkinnedMesh();
        }
    }


    private void ReplaceSkinnedMesh()
    {
        if (prefabWithSkinnedMesh == null || prefabWithNewSkinnedMesh == null)
        {
            Debug.LogError("Both prefabs must be assigned.");
            return;
        }
        
        SkinnedMeshRenderer skinnedMeshRenderer = prefabWithSkinnedMesh.GetComponentInChildren<SkinnedMeshRenderer>();

        if (skinnedMeshRenderer == null)
        {
            Debug.LogError("Prefab with Skinned Mesh must have a SkinnedMeshRenderer component.");
            return;
        }

        SkinnedMeshRenderer newSkinnedMeshRenderer = prefabWithNewSkinnedMesh.GetComponentInChildren<SkinnedMeshRenderer>();

        if (newSkinnedMeshRenderer == null)
        {
            Debug.LogError("Prefab with New Skinned Mesh must have a SkinnedMeshRenderer component.");
            return;
        }

        SkinnedMeshRenderer newSkinnedMeshRendererClone = Instantiate(newSkinnedMeshRenderer);
        newSkinnedMeshRendererClone.transform.localPosition = Vector3.zero;
        newSkinnedMeshRendererClone.transform.localRotation = Quaternion.identity;
        newSkinnedMeshRendererClone.transform.localScale = Vector3.one;

        for (int i = 0; i < newSkinnedMeshRendererClone.bones.Length; i++)
        {
            Transform bone = newSkinnedMeshRendererClone.bones[i];
            Transform newBone = prefabWithNewSkinnedMesh.transform.Find(bone.name);

            if (newBone != null)
            {
                newSkinnedMeshRendererClone.bones[i] = newBone;
            }
            else
            {
                Debug.LogWarning("Bone not found: " + bone.name);
            }
        }

        skinnedMeshRenderer.sharedMesh = newSkinnedMeshRendererClone.sharedMesh;
        // skinnedMeshRenderer.materials = newSkinnedMeshRendererClone.materials;
        DestroyImmediate(newSkinnedMeshRendererClone.gameObject);
        Debug.Log("Skinned Mesh replaced successfully.");
    }
}
