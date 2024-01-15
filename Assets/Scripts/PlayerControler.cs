using UnityEngine;
using UnityEngine.InputSystem;




public class PlayerControler : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;

    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump = true;

    public Transform orientation;

    [Header("Action Mapping")]
    public InputActionReference jumpAction;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckDistance;
    public LayerMask whatIsGround;
    bool grounded;

    [Header("Animator")]
    public Animator animator;



    [HideInInspector] public Vector2 _move;
    [HideInInspector] public Vector2 _look;

    Vector3 moveDirection;

    Rigidbody rb;

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

        // limit Speed
        SpeedControl();

        // apply Drag
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;

        animator.SetFloat("Speed" ,moveDirection.magnitude);
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        //calculate movement direction
        moveDirection = orientation.forward * _move.y + orientation.right * _move.x;

        // on ground
        if(grounded)
            rb.AddForce(10f * moveSpeed * moveDirection.normalized, ForceMode.Force);

        // in air
        else if (!grounded)
            rb.AddForce(10f * moveSpeed * airMultiplier * moveDirection.normalized, ForceMode.Force);

    }

    void SpeedControl()
    {
        Vector3 flatVel = new(rb.velocity.x, 0f, rb.velocity.z);

        // limit velocity if needed
        if(flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    void Jump(InputAction.CallbackContext callbackContext)
    {
        if(readyToJump && grounded)
        {
            rb.velocity = new(rb.velocity.x, 0f, rb.velocity.z);

            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

            readyToJump = false;

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    void ResetJump()
    {
        readyToJump = true;
    }

    public void OnMove(InputValue value)
    {
        _move = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        _look = value.Get<Vector2>();
    }
}
