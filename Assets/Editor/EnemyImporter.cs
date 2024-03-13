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
        string assetPath = AssetDatabase.GetAssetPath(outPut);
        if (GUILayout.Button("Unpack"))
        {
            GameObject contentsRoot = PrefabUtility.LoadPrefabContents(assetPath);
            contentsRoot.AddComponent<SkinnedMeshRenderer>();
            PrefabUtility.SaveAsPrefabAsset(contentsRoot, assetPath);
            PrefabUtility.UnloadPrefabContents(contentsRoot);
        }
    }
}
