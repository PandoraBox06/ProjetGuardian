using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UI_ButtonSound : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private bool isBackButton;
    private Button thisButton;
    
    private void Awake()
    {
        thisButton = GetComponent<Button>();
        thisButton.onClick.AddListener(OnClickButton);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.Instance.PlayOneShot(AudioManager.Instance.cursorSelection, transform.position);
    }

    private void OnClickButton()
    {
        if (isBackButton) AudioManager.Instance.PlayOneShot(AudioManager.Instance.cursorBack, transform.position);
        else AudioManager.Instance.PlayOneShot(AudioManager.Instance.cursorValidate, transform.position);
    }
}
