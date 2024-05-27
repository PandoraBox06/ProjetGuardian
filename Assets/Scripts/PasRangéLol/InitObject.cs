using System;
using UnityEngine;


public class InitObject : MonoBehaviour
{
    [SerializeField] private GameObject _toInit;
    [SerializeField] private GameState _stateToActivate;

    private void Start()
    {
        _toInit.SetActive(false);
    }

    private void Update()
    {
        if (GameManager.Instance.currentGameState == _stateToActivate && !_toInit.activeInHierarchy)
        {
            _toInit.SetActive(true);
        }
    }
}
