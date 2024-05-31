using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneReloader : MonoBehaviour
{
    [SerializeField] private string sceneName = "Axel";
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
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        if (isAButton)
        {
            PlayerPrefs.SetInt("Restart", 1);
        }
    }
}
