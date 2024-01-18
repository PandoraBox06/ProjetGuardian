using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraBehavior : MonoBehaviour
{
    [HideInInspector] public Vector2 _move;
    [HideInInspector] public Vector2 _look;

    [Header("References")]
    public Transform orientation;
    public Transform cameraPos;
    public Transform playerObj;
    public Rigidbody rb;

    public float rotationSpeed;

    public Transform combatLookAt;

    public GameObject thirdPersonCam;
    public GameObject combatCam;
    public GameObject topDownCam;

    public CinemachineFreeLook thirdPersonFreeLook;
    public CinemachineFreeLook combatFreeLook;
    public CinemachineFreeLook topDownFreeLook;


    public CameraStyle currentStyle;

    public InputActionReference switchCam;
    public enum CameraStyle
    {
        Basic,
        Combat,
        Topdown
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SwitchCameraStyle(CameraStyle.Basic);
    }

    public void OnMove(InputValue value)
    {
        _move = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        _look = value.Get<Vector2>();
    }


    private void Update()
    {
        if (switchCam.action.WasPressedThisFrame())
        {
            switch (currentStyle)
            {
                case CameraStyle.Basic:
                    SwitchCameraStyle(CameraStyle.Combat);
                    Debug.Log("Combat !");
                    break;
                case CameraStyle.Combat:
                    SwitchCameraStyle(CameraStyle.Basic);
                    Debug.Log("Basic !");
                    break;
                case CameraStyle.Topdown:
                    break;
            }

        }

        // rotate orientation
        Vector3 viewDir = transform.position - new Vector3(cameraPos.position.x, transform.position.y, cameraPos.position.z);
        orientation.forward = viewDir.normalized;

        // roate cameraPos object
        if (currentStyle == CameraStyle.Basic || currentStyle == CameraStyle.Topdown)
        {
            Vector3 inputDir = orientation.forward * _move.y + orientation.right * _move.x;
            if (inputDir != Vector3.zero)
                playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
        }

        else if (currentStyle == CameraStyle.Combat)
        {
            Vector3 dirToCombatLookAt = combatLookAt.position - new Vector3(cameraPos.position.x, combatLookAt.position.y, cameraPos.position.z);
            orientation.forward = dirToCombatLookAt.normalized;

            playerObj.forward = dirToCombatLookAt.normalized;
        }
    }

    private void SwitchCameraStyle(CameraStyle newStyle)
    {
        combatCam.SetActive(false);
        thirdPersonCam.SetActive(false);
        topDownCam.SetActive(false);

        if (newStyle == CameraStyle.Basic) thirdPersonCam.SetActive(true);
        if (newStyle == CameraStyle.Combat) combatCam.SetActive(true);
        if (newStyle == CameraStyle.Topdown) topDownCam.SetActive(true);

        currentStyle = newStyle;
    }

    public void DoFov(float endValue)
    {
        //Camera.main.gameObject.GetComponent<Camera>().DOFieldOfView(endValue, .25f);
        switch (currentStyle)
        {
            case CameraStyle.Basic:
                thirdPersonFreeLook.GetRig(0).GetCinemachineComponent<CinemachineOrbitalTransposer>().m_ZDamping = endValue;
                thirdPersonFreeLook.GetRig(1).GetCinemachineComponent<CinemachineOrbitalTransposer>().m_ZDamping = endValue;
                thirdPersonFreeLook.GetRig(2).GetCinemachineComponent<CinemachineOrbitalTransposer>().m_ZDamping = endValue;
                break;
            case CameraStyle.Combat:
                combatFreeLook.GetRig(0).GetCinemachineComponent<CinemachineOrbitalTransposer>().m_ZDamping = endValue;
                combatFreeLook.GetRig(1).GetCinemachineComponent<CinemachineOrbitalTransposer>().m_ZDamping = endValue;
                combatFreeLook.GetRig(2).GetCinemachineComponent<CinemachineOrbitalTransposer>().m_ZDamping = endValue;
                break;
            case CameraStyle.Topdown:
                topDownFreeLook.GetRig(0).GetCinemachineComponent<CinemachineOrbitalTransposer>().m_ZDamping = endValue;
                topDownFreeLook.GetRig(1).GetCinemachineComponent<CinemachineOrbitalTransposer>().m_ZDamping = endValue;
                topDownFreeLook.GetRig(2).GetCinemachineComponent<CinemachineOrbitalTransposer>().m_ZDamping = endValue;
                break;
            default:
                thirdPersonFreeLook.GetRig(0).GetCinemachineComponent<CinemachineOrbitalTransposer>().m_ZDamping = endValue;
                thirdPersonFreeLook.GetRig(1).GetCinemachineComponent<CinemachineOrbitalTransposer>().m_ZDamping = endValue;
                thirdPersonFreeLook.GetRig(2).GetCinemachineComponent<CinemachineOrbitalTransposer>().m_ZDamping = endValue;
                break;
        }

    }
}