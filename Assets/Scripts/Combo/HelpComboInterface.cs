using System;
using TMPro;
using UnityEngine;

public class HelpComboInterface : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private TextMeshProUGUI lastInputText1;
    [SerializeField] private TextMeshProUGUI lastInputText2;
    [SerializeField] private TextMeshProUGUI lastInputText3;

    private void Start()
    {
        BlancoCombatManager.Instance.InputEvent.AddListener(AddCombo);
        BlancoCombatManager.Instance.CancelEvent.AddListener(CleanCombo);
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

        lastInputText3.text = lastInputText2.text;
        lastInputText2.text = lastInputText1.text;
        lastInputText1.text = newInput;
    }

    private void CleanCombo()
    {
        comboText.text = "";
    }
}
