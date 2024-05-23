using System;
using System.Collections.Generic;
using UnityEngine;

public class UI_Tutorial : MonoBehaviour
{
    [SerializeField] private List<GameObject> combosList = new();
    [SerializeField] private List<GameObject> greyList = new();

    public bool isCombo1Done { get; private set; }
    public bool isCombo2Done { get; private set; }
    public bool isCombo3Done { get; private set; }

    [SerializeField] private Transform enemyPosition;
    [SerializeField] private GameObject enemyPrefab;
    private GameObject currentEnemy;
    
    public static UI_Tutorial Instance
    {
        get {
            if (_instance == null) _instance = FindObjectOfType<UI_Tutorial>();
            return _instance;
        }
    }
    private static UI_Tutorial _instance;

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
        CheckDummy();
    }

    public void DetectCombo()
    {
        if (GameManager.Instance.currentGameState != GameState.Tutorial) return;

        string _lastComboName = BlancoCombatManager.Instance.finishedCombo.comboName;
        
        CheckDummy();
        
        if (isCombo1Done)
        {
            if (isCombo2Done)
            {
                if (!isCombo3Done)
                {
                    if (_lastComboName == "Hold attack")
                    {
                        isCombo3Done = true;
                        currentEnemy.GetComponent<Enemy>().Die();
                        GameManager.Instance.ChangeGameState(GameState.PreWave);
                        ResetTutorial();
                    }
                }
            }
            else
            {
                if (_lastComboName == "Paused attack")
                {
                    isCombo2Done = true;
                    greyList[1].SetActive(true);
                    combosList[2].SetActive(true);
                }
            }
        }
        else
        {
            if (_lastComboName == "Full attack")
            {
                isCombo1Done = true;
                greyList[0].SetActive(true);
                combosList[1].SetActive(true);
            }
        }
    }

    private void CheckDummy()
    {
        if (currentEnemy == null)
        {
            currentEnemy = Instantiate(enemyPrefab, enemyPosition.position, Quaternion.identity);
        }
    }
}