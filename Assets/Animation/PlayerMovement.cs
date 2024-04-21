using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    float _speed;
    [Header("Movement")]
    [SerializeField] float _walkingSpeed = 7;
    [SerializeField] float _sprintingSpeed = 14;
    bool isSprinting = false;
    float maxSlopeAngle;

    [Header("References")]
    [SerializeField] Transform orientation;
    [SerializeField] CharacterController characterController;
    [SerializeField] Animator animator;
    [SerializeField] InputActionReference sprintAction;
    public bool isAttacking;
    public bool isDashing;

    [Header("Gravity")]
    public bool useGravity = true;
    [SerializeField] float gravity = -9.81f;
    Vector3 velocity;
    bool isGrounded = false;
    [SerializeField] float groundDistance = 0.4f;
    [SerializeField] LayerMask groundMask;

    //Private

    [HideInInspector] public Vector2 _move;
    [HideInInspector] public Vector2 _look;



    PlayerState state;
    enum PlayerState
    {
        attacking,
        dashing,
        walking,
        sprinting,
        air
    }
    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        sprintAction.action.performed += Sprinting;
    }
    private void OnDisable()
    {
        sprintAction.action.performed -= Sprinting;
    }


    private void Start()
    {
        maxSlopeAngle = characterController.slopeLimit;
        _speed = _walkingSpeed;
    }

    private void Update()
    {
        StateHandler();
        Move();
        RotatePlayerToSlope();
        Gravity();

        animator.SetBool("Sprint", isSprinting);
    }
    private void Move()
    {
        if (state == PlayerState.dashing) return;
        if (state == PlayerState.attacking) return;

        Vector3 moveDirection;
        moveDirection = orientation.forward * _move.y;
        moveDirection += orientation.right * _move.x;
        moveDirection.Normalize();
        moveDirection.y = 0;
        moveDirection *= _speed;
        characterController.Move(moveDirection * Time.deltaTime);

        animator.SetFloat("Speed", moveDirection.magnitude);

        if (isSprinting && moveDirection.magnitude < 0.1f)
        {
            isSprinting = false;    
        }
    }

    void Gravity()
    {
        if(!useGravity) return;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        GroundCheck();

        velocity.y += gravity * Time.deltaTime;

        characterController.Move(velocity * Time.deltaTime);
    }

    void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(transform.position, groundDistance, groundMask);
    }

    void Sprinting(InputAction.CallbackContext context)
    {
        isSprinting = !isSprinting;
    }

    void StateHandler()
    {
        if (isDashing)
        {
            state = PlayerState.dashing;
            _speed = 0;
        }
        if (isAttacking)
        {
            state = PlayerState.attacking;
            _speed = 0;
        }
        else if(isGrounded && isSprinting)
        {
            state = PlayerState.sprinting;
            _speed = _sprintingSpeed;
        }
        else if (isGrounded)
        {
            _speed = _walkingSpeed;
            state = PlayerState.walking;
        }
        else
        {
            _speed = _walkingSpeed / 2;
            state = PlayerState.air;
        }
    }


    RaycastHit slopeHit;
    bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
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
        }
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity - transform.up * Time.deltaTime);
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
