using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseEvent : MonoBehaviour
{
    public void PauseIt()
    {
        CombatManager.instance.PauseAction();
    }
}
