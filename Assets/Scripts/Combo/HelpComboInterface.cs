using System.Collections.Generic;
using TMPro;
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

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI lastComboText;
    [Header("Sprites")]
    [SerializeField] private Sprite attackSprite;
    [SerializeField] private Sprite pauseSprite;
    [SerializeField] private Sprite gunSprite;
    [SerializeField] private Sprite holdSprite;
    [Header("Images")]
    [SerializeField] private List<Image> combosImages = new();
    
    private BlancoCombatManager managerInstance;
    
    private int score;
    private int index;

    private void Start()
    {
        managerInstance = BlancoCombatManager.Instance;
        
        managerInstance.InputEvent.AddListener(AddCombo);
        managerInstance.CancelEvent.AddListener(CleanCombo);
        managerInstance.FinishedComboEvent.AddListener(FinishedCombo);

        CleanCombo();
    }

    private void AddCombo()
    {
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
           case "AttackRange" :
                    combosImages[index].sprite = gunSprite;
                    combosImages[index].color = Color.white;
                break;
            case "HoldAttack" :
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

    private void FinishedCombo()
    {
        lastComboText.text = "Last combo : "+managerInstance.finishedCombo.comboName;
    }

    private void CleanCombo()
    {
        index = 0;

        foreach (Image combo in combosImages)
        {
            combo.enabled = false;
        }
    }
    
    private void OnDestroy()
    {
        if (this == Instance)
            Instance = null;
    }
}
