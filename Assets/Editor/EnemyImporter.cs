using UnityEditor;
using UnityEngine;


public class EnemyImporter : EditorWindow
{
    public GameObject outPut;
    public GameObject origin;
    [MenuItem("Tools/Enemy Import")]
    public static void ShowWindow()
    {
        var window = GetWindow<EnemyImporter>();
        window.titleContent = new GUIContent("Alphonse: Grand Frere...");
        window.Show();
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("From");
        origin = EditorGUILayout.ObjectField(origin, typeof(GameObject), false) as GameObject;
        EditorGUILayout.LabelField("To");
        outPut = EditorGUILayout.ObjectField(outPut, typeof(GameObject), false) as GameObject;
        string assetPathOutput = AssetDatabase.GetAssetPath(outPut);
        
        if (GUILayout.Button("Unpack"))
        {
            var newOrigin = PrefabUtility.InstantiatePrefab(origin) as GameObject;
            PrefabUtility.SaveAsPrefabAsset(newOrigin, "Assets/Prefab/newPrefab.prefab");
            DestroyImmediate(newOrigin);
            GameObject originRoot = PrefabUtility.LoadPrefabContents("Assets/Prefab/newPrefab.prefab");
            GameObject outputRoot = PrefabUtility.LoadPrefabContents(assetPathOutput);
            GameObject skin = originRoot.GetComponentInChildren<SkinnedMeshRenderer>().gameObject;
            var mask = originRoot.GetComponentInChildren<Animator>().avatar;
            
            DestroyImmediate(outputRoot.GetComponentInChildren<SkinnedMeshRenderer>().gameObject);
            outputRoot.GetComponentInChildren<Animator>().avatar = mask;
            Instantiate(skin, outputRoot.transform, false);
            
            PrefabUtility.SaveAsPrefabAsset(outputRoot, assetPathOutput);
            PrefabUtility.UnloadPrefabContents(outputRoot);
            AssetDatabase.DeleteAsset("Assets/Prefab/newPrefab.prefab");
        }
    }
}
