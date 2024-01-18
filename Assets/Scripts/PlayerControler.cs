using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControler : MonoBehaviour
{
    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public float dashSpeed;

    public float maxYSpeed;

    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump = true;

    bool isSprinting;

    public Transform orientation;

    [Header("Dash")]
    public float dashForce;
    public float dashUpwardForce;
    public float dashDuration;
    public float dashCD;
    private float dashCDTimer;
    private bool dashing;
    public float dashSpeedChangeFactor;
    public bool useCameraForward = true;
    public bool allowAllDirections = true;
    public bool disableGravity = false;
    public bool resetVel = true;

    [Header("Climbing")]
    public LayerMask whatIsWall;
    public float climbSpeed;
    public float maxClimbTime;
    private float climbTimer;

    private bool climbing;

    public float climbingDetectionLenght;
    public float climbingSphereCastRadius;
    public float climbingMaxWallLookAngle;
    private float wallLookAngle;

    private RaycastHit frontWallHit;
    private bool wallFront;

    public float climJumpUpForce;
    public float climJumpBackForce;
    public int climbJumps;
    int climbJumpsLeft;

    Transform lastWall;
    Vector3 lastWallNormal;
    public float minWallNormalAngleChange;

    public bool exitingWall;
    public float exitWallTime;
    float exitWallTimer;

    [Header("Action Mapping")]
    public InputActionReference jumpAction;
    public InputActionReference sprintAction;
    public InputActionReference dashAction;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckDistance;
    public LayerMask whatIsGround;
    bool grounded;

    [Header("Slope Handling")]
    public Transform slopeCheck;
    public float slopeCheckDistance;
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    bool exitingSlope;

    [Header("Animator")]
    public Animator animator;

    [Header("CameraEffects")]
    public CameraBehavior cameraBehavior;
    public float dashFov;
    public float normalFov;
    public float combatFov;
    public float topDownFov;

    [HideInInspector] public Vector2 _move;
    [HideInInspector] public Vector2 _look;

    Vector3 moveDirection;

    Rigidbody rb;

    float desiredMoveSpeed;
    float lastDesiredMoveSpeed;
    MovementState lastState;
    bool keepMomentum;

    public MovementState state;
    public enum MovementState
    {
        walking,
        sprinting,
        dashing,
        climbing,
        air
    }

    private void OnEnable()
    {
        jumpAction.action.performed += Jump;
    }
    private void OnDisable()
    {
        jumpAction.action.performed -= Jump;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        // ground check
        grounded = Physics.CheckSphere(groundCheck.position, groundCheckDistance, whatIsGround);

        SpeedControl();
        StateHandler();
        WallCheck();
        RotatePlayerToSlope();

        #region Climbing
        //Climbing
        if (state == MovementState.climbing)
        {
            if (!climbing && climbTimer > 0) StartClimbing();

            // timer
            if (climbTimer > 0) climbTimer -= Time.deltaTime;
            if (climbTimer < 0) StopClimbing();

            if (exitingWall)
            {
                if (climbing) StopClimbing();
                if (exitWallTimer > 0) exitWallTimer -= Time.deltaTime;
                if (exitWallTimer < 0) exitingWall = false;
            }

            if (wallFront && jumpAction.action.WasPressedThisFrame() && climbJumpsLeft > 0) ClimbJump();
        }

        if (climbing && !exitingWall) ClimbingMovement(); 
        #endregion

        if (dashAction.action.WasPressedThisFrame())
            Dash();
        if(sprintAction.action.WasPressedThisFrame())
            isSprinting = !isSprinting;

        // apply Drag
        if (grounded && state != MovementState.dashing)
            rb.drag = groundDrag;
        else
            rb.drag = 0;

        animator.SetFloat("Speed" ,moveDirection.magnitude);

        if(dashCDTimer > 0)
            dashCDTimer -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    void StateHandler()
    {
        // Mode - Climbing
        if(wallFront && wallLookAngle < climbingMaxWallLookAngle && _move.y >= 1 && !exitingWall)
        {
            state = MovementState.climbing;
            desiredMoveSpeed = climbSpeed;
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
            exitingWall = false;
            animator.SetBool("Sprint", true);
        }

        // Mode - Walking
        else if (grounded)
        {
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
            exitingWall = false;
            animator.SetBool("Sprint", false);
        }

        // Mode - Air
        else
        {
            state = MovementState.air;

            if (climbing || exitingWall) StopClimbing();

            if (desiredMoveSpeed < sprintSpeed)
                desiredMoveSpeed = walkSpeed;
            else
                desiredMoveSpeed = sprintSpeed;
        }

        bool desiredMoveSpeedHasChanged = desiredMoveSpeed != lastDesiredMoveSpeed;
        if(lastState == MovementState.dashing)
            keepMomentum = true;

        if(desiredMoveSpeedHasChanged)
        {
            if(keepMomentum)
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

    void MovePlayer()
    {
        if (state == MovementState.dashing || exitingWall) return;

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
            rb.AddForce(10f * moveSpeed * airMultiplier * moveDirection.normalized, ForceMode.Force);


        // turn gravity off while on slope
        rb.useGravity = !OnSlope();
    }

    void SpeedControl()
    {
        // limiting speed on slope
        if (OnSlope() && !exitingSlope)
        {
            if(rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;
        }

        // limiting speed on ground or in air
        else
        {
            Vector3 flatVel = new(rb.velocity.x, 0f, rb.velocity.z);

            // limit velocity if needed
            if(flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }

        // limit y vel
        if(maxYSpeed != 0 && rb.velocity.y > maxYSpeed)
            rb.velocity = new(rb.velocity.x, maxYSpeed, rb.velocity.z);
    }


    #region Jump
    void Jump(InputAction.CallbackContext callbackContext)
    {
        if (readyToJump && grounded)
        {
            exitingSlope = true;

            rb.velocity = new(rb.velocity.x, 0f, rb.velocity.z);

            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

            readyToJump = false;

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
    } 
    #endregion

    #region Dash
    void Dash()
    {
        if (dashCDTimer > 0) return;
        else dashCDTimer = dashCD;

        dashing = true;

        cameraBehavior.DoFov(dashFov);

        Transform forwardT;

        if (useCameraForward)
            forwardT = Camera.main.transform;
        else
            forwardT = orientation;

        Vector3 direction = GetDirection(forwardT);

        Vector3 forceToApply = direction * dashForce + orientation.up * dashUpwardForce;

        if (disableGravity)
            rb.useGravity = false;

        delayedForceToApply = forceToApply;
        Invoke(nameof(DelayedDashForce), 0.025f);

        Invoke(nameof(ResetDash), dashDuration);
    }

    private Vector3 delayedForceToApply;

    void DelayedDashForce()
    {
        if (resetVel)
            rb.velocity = Vector3.zero;

        rb.AddForce(delayedForceToApply, ForceMode.Impulse);
    }

    void ResetDash()
    {
        dashing = false;

        switch (cameraBehavior.currentStyle)
        {
            case CameraBehavior.CameraStyle.Basic:
                cameraBehavior.DoFov(normalFov);
                break;
            case CameraBehavior.CameraStyle.Combat:
                cameraBehavior.DoFov(combatFov);
                break;
            case CameraBehavior.CameraStyle.Topdown:
                cameraBehavior.DoFov(topDownFov);
                break;
            default:
                cameraBehavior.DoFov(normalFov);
                break;
        }

        if (disableGravity)
            rb.useGravity = true;
    } 
    #endregion

    #region Climbing
    void WallCheck()
    {
        wallFront = Physics.SphereCast(transform.position, climbingSphereCastRadius, orientation.forward, out frontWallHit, climbingDetectionLenght, whatIsWall);
        wallLookAngle = Vector3.Angle(orientation.forward, -frontWallHit.normal);

        bool newWall = frontWallHit.transform != lastWall || Mathf.Abs(Vector3.Angle(lastWallNormal, frontWallHit.normal)) > minWallNormalAngleChange;

        if (grounded || (wallFront && newWall))
        {
            climbTimer = maxClimbTime;
            climbJumpsLeft = climbJumps;
        }
    }

    void StartClimbing()
    {
        climbing = true;

        lastWall = frontWallHit.transform;
        lastWallNormal = frontWallHit.normal;
    }

    void ClimbingMovement()
    {
        rb.velocity = new(rb.velocity.x, climbSpeed, rb.velocity.z);
    }

    void StopClimbing()
    {
        climbing = false;
    }

    void ClimbJump()
    {
        exitingWall = true;
        exitWallTimer = exitWallTime;

        Vector3 forceToApply = transform.up * climJumpUpForce + frontWallHit.normal * climJumpBackForce;

        rb.velocity = new(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(forceToApply, ForceMode.Impulse);

        climbJumpsLeft--;
    } 
    #endregion

    // Dash
    Vector3 GetDirection(Transform forwardT)
    {
        Vector3 direction = new();

        if (allowAllDirections)
            direction = forwardT.forward * _move.y + forwardT.right * _move.x;
        else
            direction = forwardT.forward;

        if (_move.x == 0 && _move.y == 0)
            direction = forwardT.forward;

        return direction.normalized;
    }

    #region Slope
    bool OnSlope()
    {
        if (Physics.Raycast(slopeCheck.position, Vector3.down, out slopeHit, slopeCheckDistance))
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

    // Smooth Speed on Dash
    private float speedChangeFactor;
    private IEnumerator SmoothlyLerpMoveSpeed()
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(slopeCheck.position, Vector3.down * slopeCheckDistance);
    }
}
