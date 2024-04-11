using UnityEngine;

public class AoELaunching : MonoBehaviour
{
    [SerializeField] private float timer;
    [SerializeField] private GameObject projectile;
    [SerializeField] private Vector3 coords;

    private void Start()
    {
        Destroy(gameObject, timer);
    }

    private void OnDestroy()
    {
        Instantiate(projectile, coords, Quaternion.identity);
    }
}
