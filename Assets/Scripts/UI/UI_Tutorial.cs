using System;
using System.Collections.Generic;
using UnityEngine;

public class UI_Tutorial : MonoBehaviour
{
    [SerializeField] private List<GameObject> combosList = new();
    [SerializeField] private List<GameObject> greyList = new();

    private bool isCombo1Done;
    private bool isCombo2Done;
    private bool isCombo3Done;

    private void Start()
    {
        BlancoCombatManager.Instance.FinishedComboEvent.AddListener(DetectCombo);
        
        ResetTutorial();
    }

    private void Update()
    {
        if (!combosList[0].activeInHierarchy && GameManager.Instance.currentGameState == GameState.Tutorial) InitTutorial();
    }

    private void ResetTutorial()
    {
        isCombo1Done = false;
        isCombo2Done = false;
        isCombo3Done = false;

        combosList[0].SetActive(false);
        combosList[1].SetActive(false);
        combosList[2].SetActive(false);
        
        greyList[0].SetActive(false);
        greyList[1].SetActive(false);
        greyList[2].SetActive(false);
    }

    private void InitTutorial()
    {
        combosList[0].SetActive(true);
    }

    public void DetectCombo()
    {
        if (GameManager.Instance.currentGameState != GameState.Tutorial) return;

        string _lastComboName = BlancoCombatManager.Instance.finishedCombo.comboName;
        
        if (isCombo1Done)
        {
            if (isCombo2Done)
            {
                if (isCombo3Done)
                {
                    GameManager.Instance.ChangeGameState(GameState.PreWave);
                    ResetTutorial();
                }
                else
                {
                    if (!combosList[2].activeInHierarchy) combosList[2].SetActive(true);
                    if (!combosList[1].activeInHierarchy) greyList[1].SetActive(true);

                    if (_lastComboName == "Hold attack")
                    {
                        isCombo3Done = true;
                        DetectCombo();
                    }
                }
            }
            else
            {
                if (!combosList[1].activeInHierarchy) combosList[1].SetActive(true);
                if (!combosList[0].activeInHierarchy) greyList[0].SetActive(true);

                if (_lastComboName == "Paused attack")
                {
                    isCombo2Done = true;
                    DetectCombo();
                }
            }
        }
        else
        {
            if (!combosList[0].activeInHierarchy) combosList[0].SetActive(true);

            if (_lastComboName == "Full attack")
            {
                isCombo1Done = true;
                DetectCombo();
            }
        }
    }
}