using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class DashController : MonoBehaviour
{
    [SerializeField] private InputActionReference dashInput;
    private CharacterController _characterController;
    private Animator animator;
    public float dashDistance = 5f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 2f;
    private float timer;
    private bool isDashing = false;
    private Vector3 dashDirection;
    private float dashTimer = 0f;

    [SerializeField] private PlayerMouvement _playerMouvement;
    [SerializeField] private ParticleSystem dashVFX;
    [SerializeField] private LayerMask _PassThroughMask;
    public static event Action<bool> OnDash; 
    
    // Update is called once per frame
    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (GameManager.Instance.currentGameState == GameState.Cutscene) return;
        if (Time.time > timer && !isDashing && dashInput.action.WasPerformedThisFrame())
        {
            // Start dashing
            StartDash();
            dashVFX.Play();
            DashPlayerSound();
        }

        if (isDashing)
        {
            // Update dash timer
            dashTimer += Time.deltaTime;

            animator.Play("Dash");
            
            
            // Check if dash duration is over
            if (dashTimer >= dashDuration)
            {
                StopDash();
            }
            else
            {
                // Move the player in dash direction
                _characterController.excludeLayers = _PassThroughMask;
                _characterController.Move(dashDirection * ((dashDistance / dashDuration) * Time.deltaTime));
            }
        }
    }

    void StartDash()
    {
        _playerMouvement.isDashing = true;
        // Get camera forward direction as dash direction
        dashDirection = transform.forward;

        // Ignore Y component to avoid dashing upwards
        dashDirection.y = 0;

        // Normalize dash direction to avoid faster dashes when looking up or down
        dashDirection.Normalize();

        // Start dashing
        isDashing = true;
        dashTimer = 0f;
        OnDash?.Invoke(isDashing);
        
        //
    }

    void StopDash()
    {
        // Stop dashing
        isDashing = false;
        OnDash?.Invoke(isDashing);
        _characterController.excludeLayers = 0;
        // Add any cooldown logic here if needed
        // Add your own cooldown logic
        timer = Time.time + dashCooldown;
        
        //
        _characterController.Move(Vector3.zero);
        _playerMouvement.isDashing = false;
    }
    
    public void DashPlayerSound()
    {
        if (!AudioManager.Instance.dash.IsNull)
            AudioManager.Instance.PlayOneShot(AudioManager.Instance.dash, transform.position);
    }
}