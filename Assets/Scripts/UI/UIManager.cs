using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public bool startWithMenu;
    public PanelsNames startingMenu;
    [Space]
    [SerializeField] private List<GameObject> allPanels = new List<GameObject>();
    public static UIManager Instance
    {
        get {
            if (_instance == null) _instance = FindObjectOfType<UIManager>();
            return _instance;
        }
    }
    private static UIManager _instance;
    
    public string bestScore {get ; private set;}
    public string playerPseudo {get ; private set;} //pas récupéré pour le moment 
    public string currentLastScore {get ; private set;}
    public string currentBestScore {get ; private set;}

    private void Awake()
    {
        //TEMPORARY
        if (startWithMenu)
        {
            GameManager.Instance.ChangeGameState(GameState.Lobby);
            OpenOnePanel(startingMenu);
        }
        else
        {
            GameManager.Instance.ChangeGameState(GameState.PreWave);
            ClosePanels();
        }
    }

    #region Panels
    public void OpenOnePanel(PanelsNames panelName)
    {
        foreach (GameObject panel in allPanels) panel.SetActive(false);
        GetPanel(panelName).SetActive(true);
    }

    public void ClosePanels()
    {
        foreach (GameObject panel in allPanels) panel.SetActive(false);
    }

    public void OpenPopUpPanel(PanelsNames panelName)
    {
        GetPanel(panelName).SetActive(true);
    }

    public void ClosePopUpPanel(PanelsNames panelName)
    {
        GetPanel(panelName).SetActive(false);
    }

    public GameObject GetPanel(PanelsNames panelName)
    {
        foreach (GameObject panel in allPanels)
            if (panel.name == panelName.ToString())
                return panel;

        Debug.LogError("No panel named '" + panelName + "' in the list!");

        return null;
    }
    #endregion

    public void RegisterNewPseudo(string newPseudo)
    {
        playerPseudo = newPseudo;
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        ClosePanels();
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        //dont forget to open a panel !
    }
}

public enum PanelsNames
{
    MainMenu,
    Pause,
    GameOver,
    Score,
    Credits
}