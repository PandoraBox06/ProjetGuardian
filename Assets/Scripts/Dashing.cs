using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Dashing : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform playerCam;
    Rigidbody rb;
    PlayerControlerV2 playerControler;

    [Header("Dashing")]
    public float dashForce = 20;
    public float dashUpwardForce = 0;
    public float maxDashYSpeed = 15;
    public float dashDuration = .25f;

    [Header("Cooldown")]
    public float dashCd = 1.5f;
    float dashCdTimer;

    [Header("Settings")]
    public bool useCameraForward = true;
    public bool allowAllDirections = true;
    public bool disableGravity = false;
    public bool resetVel = true;

    [Header("Camera Effects")]
    public CameraBehavior cam;
    public float dashFOV = 65;
    public float normalFOV = 50;

    [Header("Input")]
    public InputActionReference dashAction;

    Vector3 delayedForceToApply;

    private void OnEnable()
    {
        dashAction.action.performed += Dash;
    }
    private void OnDisable()
    {
        dashAction.action.performed -= Dash;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerControler = GetComponent<PlayerControlerV2>();
    }

    private void Update()
    {
        if(dashCdTimer > 0) dashCdTimer -= Time.deltaTime;
    }

    void Dash(InputAction.CallbackContext callbackContext)
    {
        // Cooldown
        if (dashCdTimer > 0) return;
        else dashCdTimer = dashCd;

        // Change OtherScripts
        playerControler.dashing = true;
        playerControler.maxYSpeed = maxDashYSpeed;

        cam.DoFov(dashFOV);

        // Settings
        Transform forwardT;

        if (useCameraForward) forwardT = playerCam;
        else forwardT = orientation;

        Vector3 direction = GetDirection(forwardT);

        // Action
        Vector3 forceToApply = direction * dashForce + orientation.up * dashUpwardForce;

        if (disableGravity)
            rb.useGravity = false;

        delayedForceToApply = forceToApply;
        Invoke(nameof(DelayedDashForce), 0.025f);

        Invoke(nameof(ResetDash), dashDuration);
    }

    void DelayedDashForce()
    {
        if (resetVel)
            rb.velocity = Vector3.zero;

        rb.AddForce(delayedForceToApply, ForceMode.Impulse);
    }

    void ResetDash()
    {
        playerControler.dashing = false;
        playerControler.maxYSpeed = 0;

        cam.DoFov(normalFOV);

        if (disableGravity)
            rb.useGravity = true;
    }

    Vector3 GetDirection(Transform forwardT)
    {
        Vector3 direction = new();

        if (allowAllDirections)
            direction = forwardT.forward * playerControler._move.y + forwardT.right * playerControler._move.x;
        else
            direction = forwardT.forward;


        if(playerControler._move.x == 0 && playerControler._move.y == 0) 
            direction = forwardT.forward;

        return direction.normalized;
    }
}
