using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
    [SerializeField] private Animator _fadeAnim;
    public static UI_Tutorial Instance { get; private set; }
    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        BlancoCombatManager.Instance.FinishedComboEvent.AddListener(DetectCombo);
        
        ResetTutorial();
    }

    private void Update()
    {
        if (!combosList[0].activeInHierarchy && GameManager.Instance.currentGameState == GameState.Tutorial) InitTutorial();
        if (combosList[0].activeInHierarchy && GameManager.Instance.currentGameState != GameState.Tutorial) ResetTutorial();
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
                        StartCoroutine(FadeToPlay());
                        DOVirtual.DelayedCall(0.5f, KillDummy);
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

    private void KillDummy()
    {
        currentEnemy.GetComponent<Enemy>().Die();
    }

    IEnumerator FadeToPlay()
    {
         yield return new WaitForSeconds(2f);
        _fadeAnim.Play("Fade");
        yield return new WaitForSeconds(.5f);
         if (GameManager.Instance.TutoVolume.enabled) GameManager.Instance.TutoVolume.enabled = false;
        yield return new WaitForSeconds(2.5f);
        ResetTutorial();
        GameManager.Instance.IsTutorialDone = true;
        GameManager.Instance.ChangeGameState(GameState.PreWave);
        StopCoroutine(FadeToPlay());
    }
    
    private void OnDestroy()
    {
        if (this == Instance)
            Instance = null;
    }
}