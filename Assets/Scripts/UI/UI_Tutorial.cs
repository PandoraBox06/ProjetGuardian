using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_Tutorial : MonoBehaviour
{
    [SerializeField] private List<GameObject> combosList = new();
    [SerializeField] private List<GameObject> greyList = new();
    [SerializeField] private List<Image> validationList = new();
    [SerializeField] private Sprite cross;
    [SerializeField] private Sprite valid;
    public bool isCombo1Done { get; private set; }
    public bool isCombo2Done { get; private set; }
    public bool isCombo3Done { get; private set; }
    public bool isDashDone { get; private set; }
    public bool isCrossbowDone { get; private set; }

    [SerializeField] private Transform enemyPosition;
    [SerializeField] private GameObject enemyPrefab;
    private GameObject currentEnemy;
    [SerializeField] private Animator _fadeAnim;
    public int comboCounter;
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
        if (!combosList[0].activeInHierarchy && GameManager.Instance.currentGameState == GameState.Tutorial)
            InitTutorial();
        if (combosList[0].activeInHierarchy && GameManager.Instance.currentGameState != GameState.Tutorial)
            ResetTutorial();
        if (Input.GetKeyDown(KeyCode.F2))
        {
            SkipToPlay();
        }
    }

    void SkipToPlay()
    {
        isDashDone = true;
        isCrossbowDone = true;
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
        if (HelpComboInterface.Instance != null) HelpComboInterface.Instance.ShowAll();
    }

    private void ResetTutorial()
    {
        isDashDone = false;
        isCrossbowDone = false;
        isCombo1Done = false;
        isCombo2Done = false;
        isCombo3Done = false;
        
        foreach (GameObject _object in combosList)
        {
            _object.SetActive(false);
        }
        
        foreach (GameObject _object in greyList)
        {
            _object.SetActive(false);
        }
        
        foreach (Image _object in validationList)
        {
            _object.sprite = cross;
        }

        comboCounter = 0;
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

        if (isDashDone)
        {
            if (isCrossbowDone)
            {
                if (isCombo1Done)
                {
                    if (isCombo2Done)
                    {
                        if (!isCombo3Done)
                        {
                            if (_lastComboName == "Guard break")
                            {
                                if (comboCounter >= 1)
                                {
                                    validationList[9].sprite = valid;
                                    comboCounter = 0;
                                    isCombo3Done = true;
                                    greyList[4].SetActive(true);
                                    StartCoroutine(FadeToPlay());
                                    DOVirtual.DelayedCall(0.5f, KillDummy);
                                }
                                else
                                {
                                    validationList[8].sprite = valid;
                                    comboCounter ++;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (_lastComboName == "Slide")
                        {
                            if (comboCounter >= 1)
                            {
                                validationList[7].sprite = valid;
                                comboCounter = 0;
                                isCombo2Done = true;
                                greyList[3].SetActive(true);
                                combosList[4].SetActive(true);
                            }
                            else
                            {
                                validationList[6].sprite = valid;
                                comboCounter ++;
                            }
                        }
                    }
                }
                else
                {
                    if (_lastComboName == "Great slash")
                    {
                        if (comboCounter >= 1)
                        {
                            validationList[5].sprite = valid;
                            comboCounter = 0;
                            isCombo1Done = true;
                            greyList[2].SetActive(true);
                            combosList[3].SetActive(true);
                        }
                        else
                        {
                            validationList[4].sprite = valid;
                            comboCounter ++;
                        }
                    }
                }
            }
            else
            {
                if (_lastComboName == "Crossbow shot")
                {
                    if (comboCounter >= 1)
                    {
                        validationList[3].sprite = valid;
                        comboCounter = 0;
                        isCrossbowDone = true;
                        greyList[1].SetActive(true);
                        combosList[2].SetActive(true);
                    }
                    else
                    {
                        validationList[2].sprite = valid;
                        comboCounter ++;
                    }
                }
            }
        }
        else
        {
            if (_lastComboName == "Dash")
            {
                if (comboCounter >= 1)
                {
                    validationList[1].sprite = valid;
                    comboCounter = 0;
                    isDashDone = true;
                    greyList[0].SetActive(true);
                    combosList[1].SetActive(true);
                }
                else
                {
                    validationList[0].sprite = valid;
                    comboCounter ++;
                }
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