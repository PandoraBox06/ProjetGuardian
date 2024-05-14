using UnityEngine;

public class AoEDamage : MonoBehaviour
{
    [SerializeField] private float fallingSpeed;
    [SerializeField] private Rigidbody rb;
    
    private void Update()
    {
        rb.velocity = Vector3.down * fallingSpeed;
    }
}
