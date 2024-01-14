using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(CharacterController))]
public class PlayerControler : MonoBehaviour
{
    [Header("Player Speed")]
    [SerializeField] private float movementSpeed;
    [SerializeField] private float accelerationSpeed;
    [SerializeField] private float rotationSpeed;
    private float movementX;
    private float movementY;

    private Transform cameraObject;
    private CharacterController characterController;

    [Header("Gravity & Jump")]
    public float gravity = -9.81f;
    public LayerMask groundMask;
    public float groundDistance = 0.4f;
    public Transform groundCheck;
    [SerializeField] float jumpforce;
    bool isGrounded, jump;
    Vector3 velocity;
    private Animator m_Animator;
    [Header("InputRef")]
    [SerializeField] InputActionReference jumpAction;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cameraObject = Camera.main.transform;
        m_Animator = GetComponentInChildren<Animator>();
        jumpAction.action.performed += OnJump;
    }

    private void Update()
    {
        Move();
        Gravity();
    }

    Vector3 speed;
    private void Move()
    {
        Vector3 moveDirection;
        moveDirection = cameraObject.forward * movementY;
        moveDirection = moveDirection + cameraObject.right * movementX;
        moveDirection.Normalize();
        moveDirection.y = 0;
        moveDirection *= accelerationSpeed;
        if(moveDirection.magnitude > 0.1f)
        {
            speed += moveDirection * Time.deltaTime;
            speed = Vector3.ClampMagnitude(speed, movementSpeed);

            characterController.Move(speed * Time.deltaTime);
        }
        else
            speed = Vector3.zero;

        Debug.Log($"Speed Magnitude = {speed.magnitude}");
        if(speed.magnitude >= movementSpeed)
        {
            m_Animator.SetBool("Surf", true);
        }
        else
            m_Animator.SetBool("Surf", false);
        m_Animator.SetFloat("Speed", moveDirection.magnitude);
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

    void Gravity()
    {
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        GroundCheck();

        velocity.y += gravity * Time.deltaTime;

        characterController.Move(velocity * Time.deltaTime);
    }

    void OnJump(InputAction.CallbackContext callbackContext)
    {
        if (isGrounded)
        {
            jump = true;
            velocity.y = Mathf.Sqrt(jumpforce * -2f * gravity);

            velocity.y += gravity * Time.deltaTime;
            characterController.Move(velocity * Time.deltaTime);
        }
    }

    private void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
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
