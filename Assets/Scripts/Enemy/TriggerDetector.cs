using BasicEnemyStateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDetector : MonoBehaviour
{
    public EnemyBehaviour enemyBehaviour;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (enemyBehaviour.player == null)
                enemyBehaviour.player = other.transform;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (enemyBehaviour.player == null)
                enemyBehaviour.player = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (enemyBehaviour.player != null)
                enemyBehaviour.player = null;
        }
    }
}
