using System;
using UnityEngine;
using UnityEngine.InputSystem;


public class LookingTarget : MonoBehaviour
{
    public static event Action LookTarget;
    [SerializeField] private InputActionReference attackInput;
    [SerializeField] private InputActionReference gunInput;
    [SerializeField] private InputActionReference holdInput;

    private PlayerMouvement _playerMouvement;

    private void Awake()
    {
        _playerMouvement = GetComponent<PlayerMouvement>();
    }

    private void OnEnable()
    {
        attackInput.action.started += LookAtThem;
        gunInput.action.started += LookAtThem;
        holdInput.action.started += LookAtThem;
    }

    private void OnDisable()
    {
        attackInput.action.started -= LookAtThem;
        gunInput.action.started -= LookAtThem;
        holdInput.action.started -= LookAtThem;
    }

    private void LookAtThem(InputAction.CallbackContext context)
    {
        if(_playerMouvement.isDashing) return;
        LookTarget?.Invoke();
    }
}
