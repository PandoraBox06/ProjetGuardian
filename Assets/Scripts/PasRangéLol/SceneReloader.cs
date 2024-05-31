using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneReloader : MonoBehaviour
{
    [SerializeField] private string sceneName = "Main Scene";
    [SerializeField] private KeyCode reloadKey;
    [SerializeField] private bool isAButton;
    void Update()
    {
        if (Input.GetKeyDown(reloadKey) && !isAButton)
        {
            ReloadScene();
        }
    }

    public void ReloadScene()
    {
        if (isAButton)
        {
            PlayerPrefs.SetInt("Restart", 1);
        }
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
