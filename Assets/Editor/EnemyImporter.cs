using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;


public class EnemyImporter : EditorWindow
{
    public GameObject outPut;
    public GameObject origin;
    [MenuItem("Tools/Enemy Import")]
    public static void ShowWindow()
    {
        var window = GetWindow<EnemyImporter>();
        window.titleContent = new GUIContent("Enemy Importer");
        window.Show();
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Output");
        outPut = EditorGUILayout.ObjectField(outPut, typeof(GameObject), false) as GameObject;
        EditorGUILayout.LabelField("Origin");
        origin = EditorGUILayout.ObjectField(origin, typeof(GameObject), false) as GameObject;
        string assetPathOutput = AssetDatabase.GetAssetPath(outPut);
        string assetPathOrigin = AssetDatabase.GetAssetPath(origin);
        
        if (GUILayout.Button("Unpack"))
        {
            GameObject originRoot = PrefabUtility.LoadPrefabContents(assetPathOrigin);
            GameObject outputRoot = PrefabUtility.LoadPrefabContents(assetPathOutput);
            GameObject skin = originRoot.GetComponentInChildren<SkinnedMeshRenderer>().gameObject;

            Destroy(outputRoot.GetComponentInChildren<SkinnedMeshRenderer>().gameObject);
            Instantiate(skin, outputRoot.transform, false);
            
            
            PrefabUtility.SaveAsPrefabAsset(outputRoot, assetPathOrigin);
            PrefabUtility.UnloadPrefabContents(outputRoot);
        }
    }
}
