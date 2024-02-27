using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "New Combo", menuName = "Combo")]
public class ComboScriptableObject : ScriptableObject
{
    public List<InputActionReference> inputList = new List<InputActionReference>();
}