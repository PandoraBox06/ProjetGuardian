using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerControlerV2 : MonoBehaviour
{
    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed = 7;
    public float sprintSpeed = 12;
    public float dashSpeed = 20;
    public float dashSpeedChangeFactor = 50;
    [HideInInspector] public float maxYSpeed;
    public float groundDragWalk = 7;
    public float groundDragSprint = 12;

    bool isSprinting;

    float desiredMoveSpeed;
    float lastDesiredMoveSpeed;
    MovementState lastState;
    bool keepMomentum;

    public bool isAttacking;
    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckDistance = .2f;
    public LayerMask whatIsGround;
    bool grounded;
    public float airMultiplier = .4f;

    [Header("Slope Handling")]
    public float maxSlopeAngle = 40;
    RaycastHit slopeHit;

    [Header("Action Mapping")]
    public InputActionReference sprintAction;

    [Header("References")]
    public Transform orientation;
    public Animator animator;

    //Private Variables
    [HideInInspector] public Vector2 _move;
    [HideInInspector] public Vector2 _look;
    Vector3 moveDirection;
    Rigidbody rb;

    public MovementState state;

    public enum MovementState
    {
        attacking,
        walking,
        sprinting,
        dashing,
        air
    }

    public bool dashing;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        grounded = Physics.CheckSphere(groundCheck.position, groundCheckDistance, whatIsGround);

        SpeedControl();
        StateHandler();

        switch (state)
        {
            case MovementState.attacking:
                rb.drag = groundDragWalk;
                break;
            case MovementState.walking:
                rb.drag = groundDragWalk;
                break;
            case MovementState.sprinting:
                rb.drag = groundDragSprint;
                break;
            case MovementState.dashing:
                rb.drag = 0;
                break;
            case MovementState.air:
                rb.drag = 0;
                break;
        }

        if (sprintAction.action.WasPressedThisFrame())
            isSprinting = !isSprinting;
        if(isSprinting && moveDirection.magnitude <= 0.1f)
            isSprinting = false;

        animator.SetFloat("Speed", moveDirection.magnitude);
        animator.SetBool("Sprint", isSprinting);
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    void StateHandler()
    {
        if (isAttacking)
        {
            state = MovementState.attacking;
            desiredMoveSpeed = 0;
        }
        // Mode - Dashing
        else if (dashing)
        {
            state = MovementState.dashing;
            desiredMoveSpeed = dashSpeed;
            speedChangeFactor = dashSpeedChangeFactor;
        }
        // Mode - Sprinting
        else if(grounded && isSprinting)
        {
            state = MovementState.sprinting;
            desiredMoveSpeed = sprintSpeed;
        }
        // Mode - Walking
        else if (grounded)
        {
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
        }
        // Mode - Air
        else
        {
            state = MovementState.air;

            if (desiredMoveSpeed < sprintSpeed)
                desiredMoveSpeed = walkSpeed;
            else
                desiredMoveSpeed = sprintSpeed;
        }

        bool desiredMOveSpeedHasChange = desiredMoveSpeed != lastDesiredMoveSpeed;
        if (lastState == MovementState.dashing) keepMomentum = true;

        if(desiredMOveSpeedHasChange)
        {
            if (keepMomentum)
            {
                StopAllCoroutines();
                StartCoroutine(SmoothlyLerpMoveSpeed());
            }
            else
            {
                StopAllCoroutines();
                moveSpeed = desiredMoveSpeed;
            }
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;
        lastState = state;
    }

    float speedChangeFactor;
    IEnumerator SmoothlyLerpMoveSpeed()
    {
        // smoothly lerp movementSpeed to desired value
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        float boostFactor = speedChangeFactor;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);

            time += Time.deltaTime * boostFactor;

            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
        speedChangeFactor = 1f;
        keepMomentum = false;
    }

    #region Movement
    void MovePlayer()
    {
        if (state == MovementState.dashing) return;
        if (state == MovementState.attacking) return;

        //calculate movement direction
        moveDirection = orientation.forward * _move.y + orientation.right * _move.x;

        // on slope
        if (OnSlope())
        {
            rb.AddForce(20f * moveSpeed * GetSlopeMoveDirection(), ForceMode.Force);

            if (rb.velocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }
        // on ground
        else if (grounded)
            rb.AddForce(10f * moveSpeed * moveDirection.normalized, ForceMode.Force);
        // in air
        else if (!grounded)
            rb.AddForce(10f * airMultiplier * moveSpeed * moveDirection.normalized, ForceMode.Force);

        // turn gravity off while on slope
        rb.useGravity = !OnSlope();
    }

    void SpeedControl()
    {
        // limiting speed on slope
        if (OnSlope())
        {
            if (rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;
        }

        // limiting speed on ground or in air
        else
        {
            Vector3 flatVel = new(rb.velocity.x, 0f, rb.velocity.z);

            // limit velocity if needed
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }

        // limit y vel
        if(maxYSpeed != 0 && rb.velocity.y > maxYSpeed)
            rb.velocity = new(rb.velocity.x, maxYSpeed, rb.velocity.z);
    } 
    #endregion

    #region Slope
    bool OnSlope()
    {
        if (Physics.Raycast(groundCheck.position, Vector3.down, out slopeHit))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

    void RotatePlayerToSlope()
    {
        if (OnSlope())
        {
            RaycastHit hit;
            if (Physics.SphereCast(transform.position, 0.5f, -transform.up, out hit, 5))
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(Vector3.Cross(transform.right, hit.normal), hit.normal), Time.deltaTime * 5.0f);
            }
            rb.AddForce(-9.61f * Time.deltaTime * -transform.up);
        }
    }
    #endregion

    #region InputDetection
    public void OnMove(InputValue value)
    {
        _move = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        _look = value.Get<Vector2>();
    }
    #endregion
}
