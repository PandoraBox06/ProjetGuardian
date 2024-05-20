using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.Timeline;

[CreateAssetMenu(fileName = "New Combo", menuName = "Combo")]
public class ComboScriptableObject : ScriptableObject
{
    [CanBeNull] public string comboName;
    public List<InputActionReference> inputList = new List<InputActionReference>();
    [CanBeNull] public string lastAnimation;
}