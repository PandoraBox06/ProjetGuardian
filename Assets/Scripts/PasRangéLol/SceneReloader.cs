using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneReloader : MonoBehaviour
{
    [SerializeField] private string sceneName = "Axel";
    [SerializeField] private KeyCode reloadKey;
    
    void Update()
    {
        if (Input.GetKeyDown(reloadKey))
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }
    }
}
