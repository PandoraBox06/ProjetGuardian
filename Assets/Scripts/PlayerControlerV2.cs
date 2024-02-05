using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(Animator))]
public class PlayerControlerV2 : MonoBehaviour
{
    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed = 7;
    public float sprintSpeed = 12;
    public float groundDrag = 5;

    bool isSprinting;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckDistance = .2f;
    public LayerMask whatIsGround;
    bool grounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle = 40;
    RaycastHit slopeHit;
    private bool exitingSlope;

    [Header("Jump")]
    public float jumpForce = 12;
    public float jumpCooldown = .25f;
    public float airMultiplier = .4f;
    bool readyToJump = true;

    [Header("Action Mapping")]
    public InputActionReference jumpAction;
    public InputActionReference sprintAction;
    //public InputActionReference dashAction;

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
        walking,
        sprinting,
        air
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void OnEnable()
    {
        jumpAction.action.performed += Jump;
    }
    private void OnDisable()
    {
        jumpAction.action.performed -= Jump;
    }

    private void Update()
    {
        grounded = Physics.CheckSphere(groundCheck.position, groundCheckDistance, whatIsGround);

        SpeedControl();
        StateHandler();

        if(grounded)
            rb.drag = groundDrag;
        else 
            rb.drag = 0;

        if (sprintAction.action.WasPressedThisFrame())
            isSprinting = !isSprinting;

        animator.SetFloat("Speed", moveDirection.magnitude);
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    void StateHandler()
    {
        // Mode - Sprinting
        if(grounded  && isSprinting)
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }
        // Mode - Walking
        else if (grounded)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }
        // Mode - Air
        else
        {
            state = MovementState.air;
        }
    }

    #region Movement

    void MovePlayer()
    {
        //calculate movement direction
        moveDirection = orientation.forward * _move.y + orientation.right * _move.x;

        // on slope
        if (OnSlope() && !exitingSlope)
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
        if (OnSlope() && !exitingSlope)
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
    } 
    #endregion

    #region Jump
    void Jump(InputAction.CallbackContext callbackContext)
    {
        if (readyToJump && grounded)
        {
            exitingSlope = true;
            readyToJump = false;

            rb.velocity = new(rb.velocity.x, 0f, rb.velocity.z);

            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);


            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    void ResetJump()
    {
        readyToJump = true;
        exitingSlope = false;
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
