using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HelpComboInterface : MonoBehaviour
{
    public static HelpComboInterface Instance { get; private set; }

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Sprite attackSprite;
    [SerializeField] private Sprite pauseSprite;
    [SerializeField] private Sprite gunSprite;
    [SerializeField] private Sprite holdSprite;
    [SerializeField] private List<Image> combosImages = new();
    [SerializeField] private Animator animator;
    
    private Fuckall managerInstance;
    private bool ComboIsOver;
    
    private int score;
    private int index;

    private void Start()
    {
        managerInstance = Fuckall.Instance;
        
        managerInstance.InputEvent.AddListener(AddComboInput);
        managerInstance.CancelEvent.AddListener(CleanCombo);

        CleanCombo();
    }

    private void AddComboInput()
    {
        if (ComboIsOver)
        {
            foreach (Image combo in combosImages)
            {
                combo.enabled = false;
            }

            ComboIsOver = false;
        }
        
        combosImages[index].enabled = true;
        InputAction newInput = managerInstance.actionInput;
        string inputName = newInput.name;

        switch (inputName)
        {
            case "Attack":
                combosImages[index].sprite = attackSprite;
                combosImages[index].color = Color.white;
                break;
            case "PauseAttack":
                combosImages[index].sprite = pauseSprite;
                combosImages[index].color = Color.black;
                break;
            case "AttackRange":
                combosImages[index].sprite = gunSprite;
                combosImages[index].color = Color.white;
                break;
            case "HoldAttack":
                combosImages[index].sprite = holdSprite;
                combosImages[index].color = Color.white;
                break;
        }

        index++;
    }

    public void AddScore(int _score)
    {
        score += _score;
        scoreText.text = $"Score : {score}";
    }

    private void CleanCombo()
    {
        index = 0;

        ComboIsOver = true;
        foreach (Image combo in combosImages)
        {
            combo.color = Color.red;
        }
    }
    
    private void OnDestroy()
    {
        if (this == Instance)
            Instance = null;
    }
}
