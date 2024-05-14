using BasicEnemyStateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDetector : MonoBehaviour
{
    public NewEnemyBehaviour enemyBehaviour;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (enemyBehaviour.Player == null)
                enemyBehaviour.Player = other.transform;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (enemyBehaviour.Player == null)
                enemyBehaviour.Player = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (enemyBehaviour.Player != null)
                enemyBehaviour.Player = null;
        }
    }
}
