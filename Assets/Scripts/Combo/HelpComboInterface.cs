using System;
using TMPro;
using UnityEngine;

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
    
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI lastComboText;
    private int score;

    private void Start()
    {
        BlancoCombatManager.Instance.InputEvent.AddListener(AddCombo);
        BlancoCombatManager.Instance.CancelEvent.AddListener(CleanCombo);
        BlancoCombatManager.Instance.FinishedComboEvent.AddListener(FinishedCombo);
    }

    private void AddCombo()
    {
        string newInput = BlancoCombatManager.Instance.actionInput.name.ToString();
        string inputType = BlancoCombatManager.Instance.actionType.ToString();
        inputType = inputType.Substring(0, 1);
        string inputTypeWithBrackets = $" ({inputType})";
        
        if (comboText.text == "")
        {
            comboText.text = newInput + inputTypeWithBrackets;
        }
        else
        {
            comboText.text = comboText.text + " + " + newInput + inputTypeWithBrackets;
        }
    }

    public void AddScore(int _score)
    {
        score += _score;
        scoreText.text = $"Score : {score}";
    }

    private void FinishedCombo()
    {
        lastComboText.text = "Last Combo : "+BlancoCombatManager.Instance.finishedCombo.comboName;
    }

    private void CleanCombo()
    {
        comboText.text = "";
    }
    
    private void OnDestroy()
    {
        if (this == Instance)
            Instance = null;
    }
}
