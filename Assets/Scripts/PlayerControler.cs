using UnityEngine;
using UnityEngine.InputSystem;
// ReSharper disable All


[RequireComponent(typeof(CharacterController))]
public class PlayerControler : MonoBehaviour
{
    [Header("Player Speed")]
    [SerializeField] private float movementSpeed;
    [SerializeField] private float slidingSpeed;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float dashCooldown;
    private float movementX;
    private float movementY;
    Vector3 moveDirection;
    private Transform cameraObject;
    private CharacterController characterController;
    private float timer;
    [Header("Gravity & Jump")]
    public float gravity = -9.81f;
    public LayerMask groundMask;
    public float groundDistance = 0.4f;
    public Transform groundCheck;
    [SerializeField] float jumpforce;
    bool isGrounded, jump, isDashing;
    Vector3 velocity;
    private Animator m_Animator;
    private bool isSliding;
    
    [Header("InputRef")]
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference slidingAction;
    [SerializeField] private InputActionReference dashAction;

    
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cameraObject = Camera.main.transform;
        m_Animator = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        jumpAction.action.performed += Jump;
        slidingAction.action.performed += Sliding;
        dashAction.action.performed += Dash;
    }

    private void OnDisable()
    {
        jumpAction.action.performed -= Jump;
        slidingAction.action.performed -= Sliding;
        dashAction.action.performed -= Dash;
    }

    private void Update()
    {
        Move();
        Gravity();
        ResetDash();
    }
    
    private void Move()
    {
        moveDirection = cameraObject.forward * movementY;
        moveDirection = moveDirection + cameraObject.right * movementX;
        moveDirection.Normalize();
        moveDirection.y = 0;
        Vector3 moveDir;
        moveDir = isSliding ? moveDirection *= movementSpeed : moveDirection *= slidingSpeed;
        characterController.Move(moveDir * Time.deltaTime);
        m_Animator.SetFloat("Speed", moveDir.magnitude);
        m_Animator.SetBool("Surf", isSliding);
    }

    #region Rota
    //void Rotation()
    //{
    //    Vector3 targetDirection;

    //    targetDirection = cameraObject.forward * movementY;
    //    targetDirection = targetDirection + cameraObject.right * movementX;
    //    targetDirection.Normalize();
    //    targetDirection.y = 0;

    //    if (targetDirection == Vector3.zero)
    //        targetDirection = transform.forward;

    //    Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
    //    Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

    //    transform.rotation = playerRotation;
    //} 
    #endregion

    private void Gravity()
    {
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        GroundCheck();

        velocity.y += gravity * Time.deltaTime;

        characterController.Move(velocity * Time.deltaTime);
    }

    private void Jump(InputAction.CallbackContext callbackContext)
    {
        if (!isGrounded) return;
        if (isSliding) return;
        jump = true;
        velocity.y = Mathf.Sqrt(jumpforce * -2f * gravity);

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    private void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    private void Sliding(InputAction.CallbackContext callbackContext)
    {
        isSliding = !isSliding;
    }

    private void Dash(InputAction.CallbackContext callbackContext)
    {
        if(isDashing) return;
        if (!isSliding)
        {
            characterController.Move(moveDirection * dashSpeed * Time.deltaTime);
            isDashing = true;
            timer = Time.time + dashCooldown;
        }
        
    }

    private void ResetDash()
    {
        if (Time.time > timer)
        {
            isDashing = false;
        }
    }
    
    void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        Physics.Raycast(groundCheck.position, Vector3.down, out RaycastHit hit, 1f, groundMask);

        if (hit.transform != null && hit.transform.CompareTag("Slope") && !jump)
        {
            velocity.y = -10f;
        }

        if(isGrounded && jump)
        {
            jump = false;
        }
    }
}
