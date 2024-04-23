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

    [SerializeField] private ParticleSystem dashVFX;
    
    // Update is called once per frame
    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!isDashing && dashInput.action.WasPerformedThisFrame() && Time.time > timer)
        {
            // Start dashing
            StartDash();
            dashVFX.Play();
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
                _characterController.Move(dashDirection * ((dashDistance / dashDuration) * Time.deltaTime));
                // transform.position += dashDirection * (dashDistance / dashDuration) * Time.deltaTime;
            }
        }
    }

    void StartDash()
    {
        // Get camera forward direction as dash direction
        dashDirection = transform.forward;

        // Ignore Y component to avoid dashing upwards
        dashDirection.y = 0;

        // Normalize dash direction to avoid faster dashes when looking up or down
        dashDirection.Normalize();

        // Start dashing
        isDashing = true;
        dashTimer = 0f;

        // Disable player controls during dash (optional)
        // Add your own code to disable controls
        // _playerMouvement.isDashing = true;
        // Add any visual effects or animations for dash (optional)
        // Add your own code for visual effects
    }

    void StopDash()
    {
        // Stop dashing
        isDashing = false;

        // Add any cooldown logic here if needed
        // Add your own cooldown logic
        timer = Time.time + dashCooldown;
        // Re-enable player controls after dash (optional)
        // Add your own code to re-enable controls
        // _playerMouvement.isDashing = false;
        // Add any cleanup or resetting logic after dash (optional)
        // Add your own cleanup/reset logic
    }
    
    public void DashPlayerSound()
    {
        if (!AudioManager.Instance.dash.IsNull)
            AudioManager.Instance.PlayOneShot(AudioManager.Instance.dash, transform.position);
    }
}