using System;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Sequence = DG.Tweening.Sequence;

public class HelpComboInterface : MonoBehaviour
{
    public static HelpComboInterface Instance { get; private set; }
    private bool hasFinished;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    [SerializeField] private WaveManager waveManager;
    [SerializeField] private InputActionReference closeComboInput;
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI wavemCounterText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI lastComboText;
    [Header("Sprites")]
    [SerializeField] private Sprite attackSprite;
    [SerializeField] private Sprite pauseSprite;
    [SerializeField] private Sprite gunSprite;
    [SerializeField] private Sprite holdSprite;
    [Header("Images")]
    [SerializeField] private List<Image> combosImages = new();
    [SerializeField] private GameObject lastComboBox;
    [SerializeField] private GameObject comboHelpBox;
    [SerializeField] private GameObject waveCounterBox;
    [SerializeField] private Slider inputTimingSlider;
    
    private BlancoCombatManager managerInstance;
    private Vector3 comboBoxPos;
    
    private int score;
    private int index;

    private void Start()
    {
        managerInstance = BlancoCombatManager.Instance;
        
        if (waveManager != null) waveManager.ShowNewWave.AddListener(ShowWaveNumber);
        managerInstance.InputEvent.AddListener(AddCombo);
        managerInstance.CancelEvent.AddListener(CleanCombo);
        managerInstance.FinishedComboEvent.AddListener(FinishedCombo);
        closeComboInput.action.performed += ToggleCombo;

        comboBoxPos = lastComboBox.transform.position;
        waveCounterBox.SetActive(false);

        CleanCombo();
    }

    private void Update()
    {
        inputTimingSlider.value += Time.deltaTime;
    }

    private void OnEnable()
    {
        score = 0;
    }

    private void ToggleCombo(InputAction.CallbackContext callback)
    {
        if (comboHelpBox.activeInHierarchy) comboHelpBox.SetActive(false);
        else comboHelpBox.SetActive(true);
    }

    private void ShowWaveNumber()
    {
        waveCounterBox.SetActive(true);
        wavemCounterText.text = "Wave n." + waveManager.numberOfWave;
        DOVirtual.DelayedCall(5, HideWaveNumber);
    }

    private void HideWaveNumber()
    {
        waveCounterBox.SetActive(false);
    }

    private void AddCombo()
    {
        if (hasFinished)
        {
            foreach (Image combo in combosImages)
            {
                combo.enabled = false;
            }

            hasFinished = false;
        }
        
        combosImages[index].enabled = true;
        InputAction newInput = managerInstance.actionInput;
        string inputName = newInput.name;
        
        inputTimingSlider.value = 0f;
        inputTimingSlider.maxValue = AnimationsDurations.Instance.GetDuration();

        switch (inputName)
        {
            case "Attack":
                combosImages[index].sprite = attackSprite;
                break;
            case "PauseAttack":
                combosImages[index].sprite = pauseSprite;
                break;
           case "AttackRange" :
                    combosImages[index].sprite = gunSprite;
                break;
            case "HoldAttack" :
                combosImages[index].sprite = holdSprite;
                break;
        }

        index++;
    }

    public void AddScore(int _score)
    {
        score += _score;
        scoreText.text = $"{score}";
    }

    private void FinishedCombo()
    {
        lastComboText.text = managerInstance.finishedCombo.comboName;
        
        Sequence finishSequence = DOTween.Sequence();
        finishSequence.Append(lastComboBox.transform.DOMoveY(comboBoxPos.y + 150, 0.2f));
        finishSequence.AppendInterval(0.5f);
        finishSequence.Append(lastComboBox.transform.DOMoveY(comboBoxPos.y, 0.2f));

        finishSequence.Play();
    }

    private void CleanCombo()
    {
        index = 0;

        hasFinished = true;
    }
    
    private void OnDestroy()
    {
        waveManager.ShowNewWave.RemoveAllListeners();
        managerInstance.InputEvent.RemoveAllListeners();
        managerInstance.CancelEvent.RemoveAllListeners();
        managerInstance.FinishedComboEvent.RemoveAllListeners();
        
        if (this == Instance)
            Instance = null;
    }
}
