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
        if (Input.GetKeyDown(KeyCode.F2))
        {
            SkipToPlay();
        }
    }
    
    void SkipToPlay()
    {
        isCombo1Done = true;
        isCombo2Done = true;
        isCombo3Done = true;
        greyList[2].SetActive(true);
        StartCoroutine(FadeToPlay());
        DOVirtual.DelayedCall(0.5f, KillDummy);
    }

    private void OnEnable()
    {
        HelpComboInterface.Instance.ShowOnlyTuto();
    }

    private void OnDisable()
    {
        if(HelpComboInterface.Instance != null) HelpComboInterface.Instance.ShowAll();
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
                    if (_lastComboName == "Guard break")
                    {
                        isCombo3Done = true;
                        greyList[2].SetActive(true);
                        StartCoroutine(FadeToPlay());
                        DOVirtual.DelayedCall(0.5f, KillDummy);
                    }
                }
            }
            else
            {
                if (_lastComboName == "Slide")
                {
                    isCombo2Done = true;
                    greyList[1].SetActive(true);
                    combosList[2].SetActive(true);
                }
            }
        }
        else
        {
            if (_lastComboName == "Great slash")
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
        NewWaveSound();
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
    void NewWaveSound()
    {
        if (!AudioManager.Instance.newWave.IsNull)
            AudioManager.Instance.PlayOneShot(AudioManager.Instance.newWave, transform.position);
    }
}